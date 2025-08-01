using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = System.Object;

namespace VNTags
{
    // todo honestly this should be reworked, is way too game dependant
    public struct VNTagContext
    {
        public TMP_Text   CharacterNameBox;
        public TMP_Text   TextBox;
        public GameObject DialogueWindow;
    }

    public struct VNTagDeserializationContext
    {
        public int    LineNumber;
        public string FullLine;

        public VNTagDeserializationContext(int num, string line)
        {
            LineNumber = num;
            FullLine   = line;
        }

        public override string ToString()
        {
            return LineNumber + ": " + FullLine;
        }
    }

    public struct VNTagSerializationContext
    {
        public ICollection<IVNTag> Tags;

        public VNTagSerializationContext(ICollection<IVNTag> tags)
        {
            Tags = tags;
        }

        public VNCharacterData GetMainCharacter()
        {
            foreach (IVNTag tag in Tags)
            {
                if (tag is CharacterTag cTag)
                {
                    return cTag.Character;
                }
            }

            return null;
        }
    }


    public interface IVNTag
    {
        /// <summary>
        ///     Get the tag ID to search for when parsing, case insensitive
        /// </summary>
        /// <returns></returns>
        string GetTagID();

        void Execute(VNTagContext context, out bool isFinished);

        protected internal static bool ExecuteHelper(bool? result)
        {
            if (!result.HasValue || result.Value)
            {
                return true;
            }

            return false;
        }

        void Deserialize(VNTagDeserializationContext context, params string[] parameters);

        string Serialize(VNTagSerializationContext context);

        protected internal static string SerializeHelper(string tagID, params Object[] parameters)
        {
            // todo put string values in quotes
            return "{" + tagID + ";" + string.Join(";", parameters) + "}";
        }
    }
}