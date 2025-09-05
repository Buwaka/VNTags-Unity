using System;
using System.Collections.Generic;
using UnityEngine;
using VNTags.Utility;

namespace VNTags.TextProcessors
{
    [CreateAssetMenu(fileName = "CharacterNameColor", menuName = "ScriptableObjects/TextProcessors/CharacterNameColor")]
    public class CharacterNameColor : BaseTextProcessor
    {
        // private static readonly List<(string SearchWord, string OpeningTag, string ClosingTag)> _characterNames = new();
        private static readonly Dictionary<string, (string OpeningTag, string ClosingTag)>      wordDict        = new (StringComparer.OrdinalIgnoreCase);


        public static void AddCharacter(string name, Color color)
        {
            string openingTag = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
            string closingTag = "</color>";
            wordDict.Add(name, new ValueTuple<string, string>(openingTag, closingTag) );
        }

        public static void AddCharacter(VNCharacterData character, Color color)
        {
            AddCharacter(character.Name, color);

            foreach (string alias in character.Alias)
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
            foreach (VNCharacterData character in characters)
            {
                AddCharacter(character, character.Color);
            }
        }

        public override string PostProcessDialogue(string text)
        {
            return text.EncloseBatch(wordDict);
        }
    }
}