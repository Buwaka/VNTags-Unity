using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility;

namespace VNTags
{
    public class VNTagDeserializer
    {
        private static readonly Dictionary<string, VNTag> TagLibrary = InitTagLibrary();

        /// <summary>
        ///     is the text more than just an empty line
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsSignificant(string text)
        {
            return !string.IsNullOrEmpty(text) && (text.Trim(' ').Length > 0);
        }

        /// <summary>
        ///     The primary function to process pure text into VNTags,
        ///     note: adds a confirm and EoL tag to the end of every line
        ///     todo: make it possible to decide if and what gets added at the end.
        /// </summary>
        /// <param name="text">The whole markdown based script</param>
        /// <returns>a queue containing all the tags for this script</returns>
        [NotNull]
        public static VNTagQueue Parse(string text)
        {
            var      tagQueue = new VNTagQueue();
            string[] lines    = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (ushort lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (!IsSignificant(line))
                {
                    continue;
                }

                string processedLine = TextProcessors.TextProcessors.PreProcessRawScript(line);
                processedLine = TextProcessors.TextProcessors.PostProcessRawScript(processedLine);

                bool hasDialogue = false;
                foreach (VNTag tag in ParseLine(processedLine, (ushort)(lineIndex + 1)))
                {
                    if (tag is DialogueTag)
                    {
                        hasDialogue = true;
                    }
                    tagQueue.AddLast(tag);
                }

                if (hasDialogue)
                {
                    tagQueue.AddLast(ScriptableObject.CreateInstance<ConfirmTag>());
                    tagQueue.AddLast(ScriptableObject.CreateInstance<EndOfLineTag>());
                }
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
        public static IList<VNTag> ParseLine(string line, ushort lineNumber)
        {
            var tags = new List<VNTag>();

            if (string.IsNullOrEmpty(line))
            {
                return tags;
            }

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
                    string characterName = line.Substring(start, index - start);
                    var    cTag          = ScriptableObject.CreateInstance<CharacterTag>();
                    cTag._init(VNTagID.GenerateID(lineNumber, (ushort)tags.Count), line);
                    if (cTag.Deserialize(new VNTagDeserializationContext(lineNumber, line, (ushort)tags.Count), characterName))
                    {
                        tags.Add(cTag);
                    }

                    start = index + 1;
                    continue;
                }

                // start of a tag
                if (c == '{')
                {
                    // process dialogue before tag
                    if (index > start)
                    {
                        string rawDialogue = line.Substring(start, index - start);
                        var    dialogue    = ScriptableObject.CreateInstance<DialogueTag>();
                        dialogue._init(VNTagID.GenerateID(lineNumber, (ushort)tags.Count), line);
                        if (dialogue.Deserialize(new VNTagDeserializationContext(lineNumber, line, (ushort)tags.Count), rawDialogue))
                        {
                            tags.Add(dialogue);
                        }
                    }

                    // look for the closing bracket
                    int endBracketIndex = StringUtils.FindClosingBracket(line, index, '{', '}');

                    // if not closing bracket is found
                    if (endBracketIndex == -1)
                    {
                        Debug.LogError("VNTagParser: ParseLine: did not find closing bracket for bracket at position " + index + ", for line '" + line + "'");
                        start = index + 1;
                    }
                    else
                    {
                        // closing bracket is found, and tag will be parsed
                        string tagString = line.Substring(index + 1, endBracketIndex - 1 - index);
                        VNTag  tag       = ParseTag(tagString, new VNTagDeserializationContext(lineNumber, line, (ushort)tags.Count));
                        tags.Add(tag);
                        start = endBracketIndex + 1;
                        index = endBracketIndex;
                    }
                }
            }

            // process the dialogue after all the last tag, or if no tag is present
            if (start < line.Length)
            {
                var dialogue = ScriptableObject.CreateInstance<DialogueTag>();
                dialogue._init(VNTagID.GenerateID(lineNumber, (ushort)tags.Count), line);
                if (dialogue.Deserialize(new VNTagDeserializationContext(lineNumber, line, (ushort)tags.Count), line.Substring(start)))
                {
                    tags.Add(dialogue);
                }
            }

            return tags;
        }

        private static Dictionary<string, VNTag> InitTagLibrary()
        {
            var Out = new Dictionary<string, VNTag>(StringComparer.OrdinalIgnoreCase);

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(VNTag).IsAssignableFrom(type)
                 && type.IsClass                   // Check if the type implements or inherits the interface
                 && !type.IsAbstract               // Ensure it's a class (not an interface itself or a struct)
                 && !type.IsGenericTypeDefinition) // Exclude abstract classes
                    // Exclude open generic types (e.g., IMyGenericInterface<>)
                {
                    var tag = (VNTag)ScriptableObject.CreateInstance(type);
                    Out.Add(tag.GetTagName(), tag);
                }
            }

            return Out;
        }

        private static string ExtractToken(string token)
        {
            for (int i = 0; i < token.Length; i++)
            {
                char c = token[i];

                switch (c)
                {
                    case '\\':
                        continue;
                    case '"':
                    {
                        int closingQuoteIndex = token.LastIndexOf('\"');

                        // if not closing bracket is found or it is from before the current quote
                        if ((closingQuoteIndex == -1) || (closingQuoteIndex <= i))
                        {
                            Debug.LogError("VNTagParser: ExtractToken: did not find closing bracket for bracket at position "
                                         + i
                                         + ", for token '"
                                         + token
                                         + "'");
                            continue;
                        }

                        return token.Substring(i + 1, closingQuoteIndex - i - 1);
                    }
                }
            }
            // Debug.LogWarning("VNTagParser: ExtractToken: could not properly extract token, forwarding the whole token, '" + token + "'");

            return token;
        }

        /// <summary>
        ///     parse string representation of a tag,
        ///     note: do not include the curly brackets {}
        /// </summary>
        /// <param name="line"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static VNTag ParseTag(string line, VNTagDeserializationContext context)
        {
            var    tokens   = new List<string>();
            string workLine = line;

            while (workLine.ContainsUnEscaped(';'))
            {
                int    semicolonIndex = workLine.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                string token          = ExtractToken(workLine.Substring(0, semicolonIndex));
                tokens.Add(token);
                if (semicolonIndex + 1 >= workLine.Length)
                {
                    workLine = null;
                    break;
                }

                workLine = workLine.Substring(semicolonIndex + 1);
            }

            if (workLine != null)
            {
                tokens.Add(ExtractToken(workLine));
            }

            if (tokens.Count > 0)
            {
                string tagID = tokens[0];

                if (!TagLibrary.ContainsKey(tagID))
                {
                    Debug.LogError("VNTagParser: ParseTag: did not find any tag class with ID '" + tagID + "'");
                    return null;
                }

                VNTag tagType = TagLibrary[tagID];

                var newInst = (VNTag)ScriptableObject.CreateInstance(tagType.GetType());
                newInst._init(VNTagID.GenerateID(context.LineNumber, context.TagNumber), line);
                bool result = newInst.Deserialize(context, tokens.Skip(1).ToArray());
                return result ? newInst : null;
            }

            return null;
        }
    }
}