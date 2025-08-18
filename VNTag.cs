using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VNTags.Tags;
using Object = System.Object;

namespace VNTags
{
    public struct VNTagContext
    {
        /// <summary>
        ///     ID being just an unique identifier to differentiate tags from each other
        /// </summary>
        public uint ID { get; }

        public static readonly Dictionary<string, Object> Fields = new();

        public VNTagContext(uint id)
        {
            ID = id;
        }

        public VNTagContext(VNTagContext other)
        {
            ID = other.ID;
        }

        public VNTagContext(VNTagContext other, uint id)
        {
            ID = id;
        }

        public VNTagContext Instantiate(uint id)
        {
            return new VNTagContext(this, id);
        }
    }

    public struct VNTagDeserializationContext
    {
        public readonly ushort LineNumber;
        public readonly string FullLine;
        public readonly ushort TagNumber;

        public VNTagDeserializationContext(ushort num, string line, ushort tagNumber)
        {
            LineNumber = num;
            FullLine   = line;
            TagNumber  = tagNumber;
        }

        public override string ToString()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            return LineNumber + ": " + FullLine + ", #" + TagNumber;
#else
            return "Linenumber: " + LineNumber + ", #" + TagNumber;
#endif
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
            VNTagContext instContext = context.Instantiate(ID);
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