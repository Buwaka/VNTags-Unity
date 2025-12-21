#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility.Interfaces;
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
        ///     The tag attached to this context
        /// </summary>
        public VNTag Tag { get; }

        /// <summary>
        ///     The character currently talking
        /// </summary>
        public VNCharacterData CurrentCharacter { get; private set; }

        public bool Instant;

        /// <summary>
        ///     This dictionary will be accessible from every tag, could potentially use it as another way to communicate between
        ///     tags
        /// </summary>
        public static readonly Dictionary<string, Object> DataFields = new();

        public VNTagContext(uint id, VNTag tag)
        {
            ID               = id;
            Tag              = tag;
            CurrentCharacter = null!;
            Instant          = false;
        }

        public VNTagContext(VNTagContext other)
        {
            ID               = other.ID;
            Tag              = other.Tag;
            Instant          = other.Instant;
            CurrentCharacter = null!;
        }

        private VNTagContext(VNTagContext other, uint id, VNTag tag) : this(other)
        {
            ID               = id;
            Tag              = tag;
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
            FullLine = line;
            TagNumber = tagNumber;
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
        private readonly ICollection<VNTag> Tags;

        public VNTagSerializationContext(ICollection<VNTag> tags)
        {
            Tags = tags;
        }

        public VNCharacterData GetMainCharacter()
        {
            if (Tags == null || Tags.Count <= 0) return null!;

            foreach (var tag in Tags)
                if (tag is CharacterTag cTag)
                    return cTag.Character;

            return null!;
        }
    }

    public struct VNTagParameter
    {
        public int Number { get; }
        public string Name { get; }
        public TypeCode Type { get; }
        public string Description { get; }
        public bool Optional { get; }
        public Type? EnumType { get; }
        public string[]? Options { get; }

        public GUID ID;


        public VNTagParameter
        (int number, string name, TypeCode type, string description, bool optional = false, Type? enumType = null,
            string[]? options = null)
        {
            Number = number;
            Name = name;
            Type = type;
            Description = description;
            Optional = optional;
            EnumType = enumType;
            Options = options;
            ID = GUID.Generate();
        }

        public bool Equals(VNTagParameter other)
        {
            return Number == other.Number
                   && Name == other.Name
                   && Type == other.Type
                   && Description == other.Description
                   && Optional == other.Optional
                   && Equals(EnumType, other.EnumType)
                   && (Options == other.Options ||
                       (Options != null && other.Options != null && Options.SequenceEqual(other.Options)));
        }

        public override bool Equals(object? obj)
        {
            return obj is VNTagParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Name, (int)Type, Description, Optional, EnumType, Options);
        }

        public static bool operator ==(VNTagParameter left, VNTagParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VNTagParameter left, VNTagParameter right)
        {
            return !left.Equals(right);
        }
    }

    public class VNTagParameters : SortedDictionary<VNTagParameter, string>
    {
        public VNTagParameters() : base(new VNTagParameterComparer())
        {
        }

        public bool UpdateParameter(VNTagParameter parameter, string newValue)
        {
            // note, this might look strange but the comparer only checks for the parameter number, 
            // so in case the parameter already exists but is different, we'll remove it and add the new one
            if (ContainsKey(parameter))
            {
                var value = this[parameter];
                foreach (var key in Keys)
                    if (key.Number == parameter.Number)
                    {
                        if (!Equals(key, parameter))
                        {
                            Remove(parameter);
                            Add(parameter, newValue);
                            return true;
                        }

                        if (!Equals(value, newValue))
                        {
                            this[parameter] = newValue;
                            return true;
                        }

                        return false;
                    }
            }
            else
            {
                Add(parameter, newValue);
            }

            return false;
        }

        public void DefaultParameter(VNTagParameter parameter, string newValue)
        {
            // note, this might look strange but the comparer only checks for the parameter number, 
            // so in case the parameter already exists but is different, we'll remove it and add the new one
            if (ContainsKey(parameter))
            {
                foreach (var pair in this)
                    if (pair.Key.Number == parameter.Number)
                    {
                        if (!Equals(pair.Key, parameter))
                        {
                            Remove(parameter);
                            Add(parameter, pair.Value);
                        }

                        return;
                    }
            }
            else
            {
                Add(parameter, newValue);
            }
        }

        // to ensure parameters are in the right order for deserialization
        private class VNTagParameterComparer : IComparer<VNTagParameter>
        {
            public int Compare(VNTagParameter x, VNTagParameter y)
            {
                return x.Number.CompareTo(y.Number);
            }
        }
    }


    [Serializable]
    public abstract class VNTag : ScriptableObject, IStringSerializable
    {
        public readonly static char[] EscapeCharacters = new[] { ';' }; 
            
        private string _rawTag = "";
        public VNTagID ID { get; private set; }

        public string StringRepresentation
        {
            get => _rawTag;
            set => _rawTag = value;
        }

        public string Serialize(object? context)
        {
            if (context is VNTagSerializationContext VNTagContext) return Serialize(VNTagContext);

            return Serialize(new VNTagSerializationContext());
        }

        public object Deserialize(object? context)
        {
            if (context is VNTagDeserializationContext VNTagContext)
                return VNTagDeserializer.ParseTag(StringRepresentation, VNTagContext);

            return VNTagDeserializer.ParseTag(StringRepresentation,
                new VNTagDeserializationContext(0, StringRepresentation, 0));
        }

        public void _init(VNTagID id, string raw)
        {
            ID = id;
            _rawTag = raw;
            name = ToString();
        }

        /// <summary>
        ///     Get the tag ID to search for when parsing, case insensitive
        /// </summary>
        /// <returns></returns>
        public abstract string GetTagName();

        /// <summary>
        ///     This will be used to generate tags in the editor
        /// </summary>
        /// <param name="currentParameters">
        ///     the current parameters along with the current value, these are often going to be null, This can be useful for type
        ///     safe tag creation
        /// </param>
        /// <returns></returns>
        public VNTagParameters GetParameters(VNTagParameters currentParameters)
        {
            return Parameters(currentParameters);
        }

        /// <summary>
        ///     This will be used to generate tags in the editor
        /// </summary>
        /// <returns>
        ///     VNTagParameters is a type specialization of SortedDictionary with the VNTagParameter as key and the parameter
        ///     value as Object
        /// </returns>
        public VNTagParameters GetParameters()
        {
            var currentParameters = new VNTagParameters();
            return Parameters(currentParameters);
        }

        /// <summary>
        ///     Parameters that this tag takes,
        ///     this is mainly used in the editor to create a property drawer, which is why it also provides the current
        ///     parameters.
        ///     Note: a general rule of thumb, do not reference the fields of the tag instance, I would make this a static function
        ///     if I could to enforce it
        /// </summary>
        /// <param name="currentParameters"></param>
        /// <returns></returns>
        protected abstract VNTagParameters Parameters(VNTagParameters currentParameters);

        /// <summary>
        ///     Whether this tag should show up as an option in the editor
        /// </summary>
        /// <returns></returns>
        public virtual bool EditorVisible()
        {
            return true;
        }

        public void BaseExecute(VNTagContext context, out bool isFinished)
        {
            var instContext = context.Instantiate(ID, this);
            Execute(instContext, out isFinished);
        }

        protected abstract void Execute(VNTagContext context, out bool isFinished);

        protected static bool ExecuteHelper(bool? result)
        {
            if (!result.HasValue || result.Value) return true;

            return false;
        }

        /// <summary>
        ///     string -> tag
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns>whether the deserialization succeeded</returns>
        public abstract bool Deserialize(VNTagDeserializationContext context, params string[] parameters);

        /// <summary>
        ///     tag -> string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string Serialize(VNTagSerializationContext context);

        /// <summary>
        ///     formats each parameter into a string representation,
        ///     filters out null values!!!
        /// </summary>
        /// <param name="tagID"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected static string SerializeHelper(string tagID, params object[] parameters)
        {
            bool isvalid(object parameter)
            {
                if (parameter == null) return false;
                if (parameter is string s && string.IsNullOrEmpty(s)) return false;
                return true;
            }

            var formattedParameters = parameters.Where((p, index) =>
            {
                // basically if a parameter is null but has any valid parameters later in the array, we need to keep it to maintain the order
                if (!isvalid(p))
                {
                    var anyValid = false;
                    for (var i = index + 1; i < parameters.Length; i++)
                        if (isvalid(parameters[i]))
                        {
                            anyValid = true;
                            break;
                        }

                    return anyValid;
                }

                return true;
            }).Select(p =>
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
                    case string[] sarr:
                        return string.Join(';', sarr.Select(t => $"\"{t}\""));
                    case IEnumerable<string> earr:
                        return string.Join(';', earr.Select(t => $"\"{t}\""));
                    case null:
                        return "\"\"";
                    default:
                        return p.ToString();
                }
            });

            return "{" + tagID + ";" + string.Join(";", formattedParameters) + "}";
        }

        public int DeepHash()
        {
            var hashCode = new HashCode();

            // Get all public instance fields
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if (value != null) hashCode.Add(value);
            }

            // Get all public instance properties
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
                // Ensure the property has a getter and is not an indexer
                if (prop.CanRead && prop.GetIndexParameters().Length == 0)
                {
                    var value = prop.GetValue(this);
                    if (value != null) hashCode.Add(value);
                }

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return GetTagName() + ": " + ID;
        }


#if UNITY_EDITOR
        public static IEnumerable<Type> GetAllTagTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(VNTag)));
        }
#endif
    }
}