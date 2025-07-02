using System;
using System.Collections.Generic;
using UnityEngine;


namespace VNTags
{
    // based on markdown
    public class VNTagParser
    {
        
        public static bool isSignificant(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Trim(' ').Length > 0;
        }

        /// <summary>
        /// The primary function to process pure text into VNTags
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
                    cTag.Init(CharacterName, context);
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
                        dialogue.Init(RawDialogue, context);
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
                        var tag = ParseTag(tagString);
                        tags.Add(tag);
                        start = endBracketIndex + 1;
                    }
                    continue;
                }
            }

            // process the dialogue after all the last tag, or if no tag is present
            if (start < line.Length)
            {
                DialogueTag dialogue = new DialogueTag();
                dialogue.Init(line.Substring(start), context);
                tags.Add(dialogue);
            }

            return tags;
        }

        public static IVNTag ParseTag(string line)
        {
            //todo
            return null;
        }

    }
}