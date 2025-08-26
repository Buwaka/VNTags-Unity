using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Scripting;

namespace VNTags.TextProcessors
{
    [CreateAssetMenu(fileName = "CharacterNameColor", menuName = "ScriptableObjects/CharacterNameColor")]
    public class CharacterNameColor : BaseTextProcessor
    {
        private static SortedDictionary<string, string> _characterNames = new SortedDictionary<string, string>();


        public static void AddCharacter(string name, Color color)
        {
            _characterNames.Add(name, ColorUtility.ToHtmlStringRGBA(color));
        }
        
        public static void AddCharacter(VNCharacterData character, Color color)
        {
            AddCharacter(character.Name, color);

            foreach (var alias in character.Alias)
            {
                AddCharacter(alias, color);
            }
        }
        
        public static void AddCharacter(VNCharacterData character)
        {
            AddCharacter(character, character.Color);
        }

        public static void AddCharacter(IEnumerable<VNCharacterData> characters)
        {
            foreach (var character in characters)
            {
                AddCharacter(character, character.Color);
            }
        }
        
        public override string PostProcessDialogue(string text)
        {
            return EncloseNames(text);
        }
        
        public string EncloseNames(string text)
        {
            var sb = new System.Text.StringBuilder(text);

            foreach (var character in _characterNames)
            {
                var pattern = "\\b" + Regex.Escape(character.Key) + "\\b";
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);

                // Iterate over matches in reverse to avoid messing up the indices
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    
                    // the actual enclosing part
                    sb.Insert(match.Index + match.Length, "</color>");
                    sb.Insert(match.Index,                $"<color=#{character.Value}>");
                }
            }
            return sb.ToString();
        }
    }
}