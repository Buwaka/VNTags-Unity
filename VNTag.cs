using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using Object = System.Object;

namespace VNTags
{
    // todo honestly this should be reworked, is way too game dependant
    public struct VNTagContext
    {
        /// <summary>
        /// ID being just an unique identifier to differentiate tags from each other
        /// </summary>
        public uint       ID{ get;                private set; }
        public TMP_Text   CharacterNameBox { get; private set; }
        public TMP_Text   TextBox          { get; private set; }
        public GameObject DialogueWindow   { get; private set; }

        public VNTagContext(VNTagContext other)
        {
            ID               = other.ID;
            CharacterNameBox = other.CharacterNameBox;
            TextBox          = other.TextBox;
            DialogueWindow   = other.DialogueWindow;
        }

        public VNTagContext(VNTagContext other, uint id)
        {
            ID               = id;
            CharacterNameBox = other.CharacterNameBox;
            TextBox          = other.TextBox;
            DialogueWindow   = other.DialogueWindow;
        }

        public VNTagContext(TMP_Text characterNameBox, TMP_Text textBox, GameObject dialogueWindow) : this()
        {
            CharacterNameBox = characterNameBox;
            TextBox          = textBox;
            DialogueWindow   = dialogueWindow;
        }

        public VNTagContext Instantiate(uint id)
        {
            return new VNTagContext(this, ID);
        }
    }

    public struct VNTagDeserializationContext
    {
        public readonly int    LineNumber;
        public readonly string FullLine;

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
        public ICollection<VNTag> Tags;

        public VNTagSerializationContext(ICollection<VNTag> tags)
        {
            Tags = tags;
        }

        public VNCharacterData GetMainCharacter()
        {
            foreach (VNTag tag in Tags)
            {
                if (tag is CharacterTag cTag)
                {
                    return cTag.Character;
                }
            }

            return null;
        }
    }


    public abstract class VNTag
    {
        public uint   ID     { get; private set; }
        public string RawTag { get; private set; }

        public void _init(uint id, string raw)
        {
            ID     = id;
            RawTag = raw;
        }
        
        /// <summary>
        ///     Get the tag ID to search for when parsing, case insensitive
        /// </summary>
        /// <returns></returns>
        public abstract string GetTagName();

        public void BaseExecute(VNTagContext context, out bool isFinished)
        {
            var instContext = context.Instantiate(ID);
            Execute(instContext, out isFinished);
        }

        protected abstract void Execute(VNTagContext context, out bool isFinished);

        protected static bool ExecuteHelper(bool? result)
        {
            if (!result.HasValue || result.Value)
            {
                return true;
            }

            return false;
        }

        public abstract void   Deserialize(VNTagDeserializationContext context, params string[] parameters);
        public abstract string Serialize(VNTagSerializationContext     context);

        protected static string SerializeHelper(string tagID, params Object[] parameters)
        {
            var formattedParameters = parameters.Select(p =>
            {
                switch (p)
                {
                    case string s:
                        return $"\"{s}\"";
                    case double d:
                        return Convert.ToDecimal(d).ToString(CultureInfo.InvariantCulture);
                    case float f:
                        return Convert.ToDecimal(f).ToString(CultureInfo.InvariantCulture);
                    case bool b:
                        return b ? "true" : "false";
                    default:
                        return p.ToString();
                }
            });

            return "{" + tagID + ";" + string.Join(";", formattedParameters) + "}";
        }
    }
}