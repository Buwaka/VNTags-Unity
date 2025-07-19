using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace VNTags
{

    public struct VNTagContext
    {
        public TMPro.TMP_Text CharacterNameBox;
        public TMPro.TMP_Text TextBox;
        public GameObject DialogueWindow;
    }

    public struct VNTagDeserializationContext
    {
        public int LineNumber;
        public string FullLine;

        public VNTagDeserializationContext(int num, string line)
        {
            LineNumber = num;
            FullLine = line;
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

        public VNCharacter GetMainCharacter()
        {
            foreach (var tag in Tags)
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
        /// Get the tag ID to search for when parsing, case insensitive
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
            else
            {
                return false;
            }
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
