#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using VNTags.Tags;
using Object = System.Object;

namespace VNTags
{
    public struct VNTagContext
    {
        /// <summary>
        ///     ID being just a unique identifier to differentiate tags from each other
        /// </summary>
        public uint ID { get; }
        /// <summary>
        /// The tag attached to this context
        /// </summary>
        public VNTag Tag { get; }

        /// <summary>
        /// The character currently talking
        /// </summary>
        public VNCharacterData CurrentCharacter { get; private set; }

        /// <summary>
        /// This dictionary will be accessible from every tag, could potentially use it as another way to communicate between tags
        /// </summary>
        public static readonly Dictionary<string, Object> DataFields = new();

        public VNTagContext(uint id, VNTag tag)
        {
            ID               = id;
            Tag              = tag;
            CurrentCharacter = null;
        }

        public VNTagContext(VNTagContext other)
        {
            ID               = other.ID;
            Tag              = other.Tag;
            CurrentCharacter = null;
        }

        private VNTagContext(VNTagContext other, uint id , VNTag tag)
        {
            ID               = id;
            Tag              = tag;
            CurrentCharacter = null;
        }

        public VNTagContext Instantiate(uint id, VNTag tag)
        {
            return new VNTagContext(this, id, tag);
        }

        public void SetMainCharacter(VNCharacterData currentCharacter)
        {
            CurrentCharacter = currentCharacter;
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

    public struct VNTagParameter
    {
        public string    Name         { get; }
        public TypeCode  Type         { get; }
        public string    Description  { get; }
        public object?   DefaultValue { get; }
        public bool      Optional     { get; }
        public Type?     EnumType     { get; }
        public string[]? Options      { get; }

        public VNTagParameter(string name, TypeCode type, string description, object? defaultValue = null, bool optional = false, Type? enumType = null, string[]? options = null)
        {
            Name        = name;
            Type        = type;
            Description = description;

            DefaultValue = defaultValue;
            Optional     = optional;
            EnumType     = enumType;
            Options      = options;
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

        /// <summary>
        /// This will be used to generate tags in the editor
        /// </summary>
        /// <param name="currentParameters">the current parameters, these are often going to be null, This can be useful for type safe tag creation</param>
        /// <returns></returns>
        public abstract VNTagParameter[] GetParameters(IList<Object> currentParameters);

        /// <summary>
        /// Whether this tag should show up as an option in the editor
        /// </summary>
        /// <returns></returns>
        public virtual bool EditorVisibility()
        {
            return true;
        }
        
        public void BaseExecute(VNTagContext context, out bool isFinished)
        {
            VNTagContext instContext = context.Instantiate(ID, this);
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

        /// <summary>
        /// string -> tag
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns>whether the deserialization succeeded</returns>
        public abstract bool Deserialize(VNTagDeserializationContext context, params string[] parameters);
        
        /// <summary>
        /// tag -> string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string Serialize(VNTagSerializationContext     context);

        /// <summary>
        /// formats each parameter into a string representation,
        /// filters out null values!!!
        /// </summary>
        /// <param name="tagID"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected static string SerializeHelper(string tagID, params Object[] parameters)
        {
            var formattedParameters = parameters.Where((p) => p != null).Select(p =>
            {
                switch (p)
                {
                    case Enum e:
                        return $"\"{e.ToString()}\"";
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