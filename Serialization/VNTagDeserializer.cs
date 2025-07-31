using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VNTags
{
    // based on markdown
    public class VNTagDeserializer
    {
        private static readonly Dictionary<string, IVNTag> TagLibrary = InitTagLibrary();

        /// <summary>
        ///     is the text more than just an empty line
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool isSignificant(string text)
        {
            return !string.IsNullOrEmpty(text) && (text.Trim(' ').Length > 0);
        }

        /// <summary>
        ///     The primary function to process pure text into VNTags,
        ///     note: adds a confirm and EoL tag to the end of every line
        /// </summary>
        /// <param name="text">The whole markdown based script</param>
        /// <returns>a queue containing all the tags for this script</returns>
        public static LinkedList<IVNTag> Parse(string text)
        {
            var tagQueue = new LinkedList<IVNTag>();
            string[] lines = text.Split(
                                        new[] { "\r\n", "\r", "\n" },
                                        StringSplitOptions.None
                                       );

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (!isSignificant(line))
                {
                    continue;
                }

                foreach (IVNTag tag in ParseLine(line, lineIndex + 1))
                {
                    tagQueue.AddLast(tag);
                }

                tagQueue.AddLast(new ConfirmTag());
                tagQueue.AddLast(new EndOfLineTag());
            }

            return tagQueue;
        }


        /// <summary>
        ///     sub function of Parse(string) where each character will be evaluated,
        ///     splitting the line into individual tags
        /// </summary>
        /// <param name="line">A single line of the script</param>
        /// <param name="lineNumber">
        ///     Line number in source script, mainly for editor and debugging purposes,
        ///     potentially for choices later on
        /// </param>
        /// <returns>a collection of VNTags</returns>
        public static ICollection<IVNTag> ParseLine(string line, int lineNumber)
        {
            var context = new VNTagDeserializationContext(lineNumber, line);

            var tags = new List<IVNTag>();

            int start = 0;
            for (int index = 0; index < line.Length; index++)
            {
                char c = line[index];

                // \ is an escape character
                if (c == '\\')
                {
                    index++;
                    continue;
                }

                // end of a character name
                if (c == ';')
                {
                    string CharacterName = line.Substring(start, index - start);
                    var    cTag          = new CharacterTag();
                    cTag.Deserialize(context, CharacterName);
                    tags.Add(cTag);
                    start = index + 1;
                    continue;
                }

                // start of a tag
                if (c == '{')
                {
                    // process dialogue before tag
                    if (index > start)
                    {
                        string RawDialogue = line.Substring(start, index);
                        var    dialogue    = new DialogueTag();
                        dialogue.Deserialize(context, RawDialogue);
                        tags.Add(dialogue);
                    }

                    // look for the closing bracket
                    int endBracketIndex = line.IndexOf("}", index, StringComparison.Ordinal);

                    // if not closing bracket is found
                    if (endBracketIndex == -1)
                    {
                        Debug.LogError("VNTagParser: ParseLine: did not find closing bracket for bracket at position "
                                     + index
                                     + ", for line '"
                                     + line
                                     + "'");
                        start = index + 1;
                    }
                    else
                    {
                        // closing bracket is found, and tag will be parsed
                        string tagString = line.Substring(index + 1, endBracketIndex - 1 - index);
                        IVNTag tag       = ParseTag(tagString, context);
                        tags.Add(tag);
                        start = endBracketIndex + 1;
                        index = endBracketIndex;
                    }
                }
            }

            // process the dialogue after all the last tag, or if no tag is present
            if (start < line.Length)
            {
                var dialogue = new DialogueTag();
                dialogue.Deserialize(context, line.Substring(start));
                tags.Add(dialogue);
            }

            return tags;
        }

        private static Dictionary<string, IVNTag> InitTagLibrary()
        {
            var Out = new Dictionary<string, IVNTag>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IVNTag).IsAssignableFrom(type)
                 && // Check if the type implements or inherits the interface
                    type.IsClass
                 && // Ensure it's a class (not an interface itself or a struct)
                    !type.IsAbstract
&&                                                 // Exclude abstract classes
                    !type.IsGenericTypeDefinition) // Exclude open generic types (e.g., IMyGenericInterface<>)
                {
                    var tag = (IVNTag)Activator.CreateInstance(type);
                    Out.Add(tag.GetTagID(), tag);
                }
            }

            return Out;
        }


        /// <summary>
        ///     parse string representation of a tag,
        ///     note: do not include the curly brackets {}
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IVNTag ParseTag(string line, VNTagDeserializationContext context)
        {
            var tokens = new List<string>();

            int start = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if ((c == '"') && (i + 1 < line.Length))
                {
                    int closingQuoteIndex = line.IndexOf("\"", i + 1, StringComparison.OrdinalIgnoreCase);

                    // if not closing bracket is found
                    if (closingQuoteIndex == -1)
                    {
                        Debug.LogError("VNTagParser: ParseTag: did not find closing bracket for bracket at position "
                                     + i
                                     + ", for line '"
                                     + line
                                     + "'");
                        continue;
                    }

                    tokens.Add(line.Substring(start, closingQuoteIndex - start));
                    i     = closingQuoteIndex;
                    start = i + 1;
                }
                else if (c == ';')
                {
                    tokens.Add(line.Substring(start, i - start));
                    start = i + 1;
                }
            }

            if (start < line.Length)
            {
                tokens.Add(line.Substring(start));
            }

            if (tokens.Count > 0)
            {
                string tagID = tokens[0];

                if (!TagLibrary.ContainsKey(tagID))
                {
                    Debug.LogError("VNTagParser: ParseTag: did not find any tag class with ID '" + tagID + "'");
                    return null;
                }

                IVNTag tagType = TagLibrary[tagID];

                var newInst = (IVNTag)Activator.CreateInstance(tagType.GetType());
                newInst.Deserialize(context, tokens.Skip(1).ToArray());
                return newInst;
            }

            return null;
        }
    }
}