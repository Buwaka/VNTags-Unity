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
        
        /// <summary>
        /// is the text more than just an empty line
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool isSignificant(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Trim(' ').Length > 0;
        }

        /// <summary>
        /// The primary function to process pure text into VNTags,
        /// note: adds a confirm and EoL tag to the end of every line
        /// </summary>
        /// <param name="text">The whole markdown based script</param>
        /// <returns>a queue containing all the tags for this script</returns>
        public static Queue<IVNTag> Parse(string text)
        {
            Queue<IVNTag> tagQueue = new Queue<IVNTag>();
            var lines = text.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            
            for(var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                if (!isSignificant(line))
                {
                    continue;
                }
                
                foreach (var tag in ParseLine(line, lineIndex + 1))
                {
                    tagQueue.Enqueue(tag);
                }
                tagQueue.Enqueue(new ConfirmTag());
                tagQueue.Enqueue(new EndOfLineTag());
            }
            
            return tagQueue;
        }


        /// <summary>
        /// sub function of Parse(string) where each character will be evaluated,
        /// splitting the line into individual tags
        /// </summary>
        /// <param name="line">A single line of the script</param>
        /// <param name="lineNumber">Line number in source script, mainly for editor and debugging purposes,
        /// potentially for choices later on</param>
        /// <returns>a collection of VNTags</returns>
        public static ICollection<IVNTag> ParseLine(string line, int lineNumber)
        {
            VNTagLineContext context = new VNTagLineContext(lineNumber, line);
            
            List<IVNTag> tags = new List<IVNTag>();

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
                    var CharacterName = line.Substring(start, index - start);
                    CharacterTag cTag = new CharacterTag();
                    cTag.Deserialize(context, CharacterName);
                    tags.Add(cTag);
                    start = index + 1;
                    continue;
                }

                // start of a tag
                if (c == '{')
                {
                    // process dialogue before tag
                    if (index > 0)
                    {
                        var RawDialogue = line.Substring(start, index - 1);
                        DialogueTag dialogue = new DialogueTag();
                        dialogue.Deserialize(context, RawDialogue);
                        tags.Add(dialogue);
                    }
                    
                    // look for the closing bracket
                    var endBracketIndex = line.IndexOf("}", index, StringComparison.Ordinal);

                    // if not closing bracket is found
                    if (endBracketIndex == -1)
                    {
                        Debug.LogError("VNTagParser: ParseLine: did not find closing bracket for bracket at position " + index + ", for line '" + line + "'");
                        start = index + 1;
                    }
                    else
                    {
                        // closing bracket is found, and tag will be parsed
                        var tagString = line.Substring(index + 1, endBracketIndex - index - 1);
                        var tag = ParseTag(tagString, context);
                        tags.Add(tag);
                        start = endBracketIndex + 1;
                        index = endBracketIndex;
                    }
                    continue;
                }
            }

            // process the dialogue after all the last tag, or if no tag is present
            if (start < line.Length)
            {
                DialogueTag dialogue = new DialogueTag();
                dialogue.Deserialize(context, line.Substring(start));
                tags.Add(dialogue);
            }

            return tags;
        }

        private static Dictionary<string, IVNTag> TagLibrary = InitTagLibrary();

        private static Dictionary<string, IVNTag> InitTagLibrary()
        {
            Dictionary<string, IVNTag> Out = new Dictionary<string, IVNTag>();
            
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IVNTag).IsAssignableFrom(type) && // Check if the type implements or inherits the interface
                    type.IsClass && // Ensure it's a class (not an interface itself or a struct)
                    !type.IsAbstract && // Exclude abstract classes
                    !type.IsGenericTypeDefinition) // Exclude open generic types (e.g., IMyGenericInterface<>)
                {
                    var tag = (IVNTag) Activator.CreateInstance(type);
                    Out.Add(tag.GetTagID(), tag);
                }
            }

            return Out;
        }
        
        
        /// <summary>
        /// parse string representation of a tag,
        /// note: do not include the curly brackets {}
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IVNTag ParseTag(string line, VNTagLineContext context)
        {
            List<string> tokens = new List<string>();

            int start = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"' && (i + 1) < line.Length)
                {
                    var closingQuoteIndex = line.IndexOf("\"", i + 1, StringComparison.OrdinalIgnoreCase);
                    
                    // if not closing bracket is found
                    if (closingQuoteIndex == -1)
                    {
                        Debug.LogError("VNTagParser: ParseTag: did not find closing bracket for bracket at position " + i + ", for line '" + line + "'");
                        continue;
                    }
                    tokens.Add(line.Substring(start, closingQuoteIndex - start - 1));
                    i = closingQuoteIndex;
                }
                else if (c == ';')
                {
                    tokens.Add(line.Substring(start, i - start - 1));
                }
            }
            
            if (start < line.Length)
            {
                tokens.Add(line.Substring(start));
            }

            if (tokens.Count > 0)
            {
                var tagID = tokens[0];

                if (!TagLibrary.ContainsKey(tagID))
                {
                    Debug.LogError("VNTagParser: ParseTag: did not find any tag class with ID '" + tagID + "'");
                    return null;
                }

                var tagType = TagLibrary[tagID];

               var newInst = (IVNTag)  Activator.CreateInstance(tagType.GetType());
               newInst.Deserialize(context, tokens.Skip(1).ToArray());
               return newInst;
            }

            return null;
        }

    }
}