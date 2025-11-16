using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VNTags.Utility;

namespace VNTags.Editor
{
    public class CreateTagWindow : EditorWindow
    {
        private readonly Dictionary<int, (int index, bool freeInput)> _popupIndex = new();
        private readonly SortedDictionary<string, VNTag>              _tags       = new();
        private          bool                                         _addNew;
        private          string                                       _current;
        private          VNTag                                        _currentSelectedTag;
        private          int                                          _cursor;
        private          VNTagDeserializationContext                  _deserializationContext;

        private int                       _lastIndex;
        private bool                      _parameterChanged;
        private VNTagParameters           _parameters = new();
        private VNTagSerializationContext _serializationContext;
        private string                    _serializedTag;
        private int                       _tagIndex;
        private Action<string>            _target;


        private void OnGUI()
        {
            EditorGUILayout.Separator();

            string[] tagNames = GetPotentialTags();
            _tagIndex = EditorGUILayout.Popup(_tagIndex, tagNames);

            bool tagChanged = _lastIndex != _tagIndex;
            _lastIndex = _tagIndex;

            // index 0 the placeholder display value and thus not a tag
            if (_tagIndex != 0)
            {
                EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                if (tagChanged)
                {
                    _currentSelectedTag = _tags[tagNames[_tagIndex]];
                    _serializedTag      = null;
                    _parameters         = new VNTagParameters();
                }

                _parameters = _currentSelectedTag.GetParameters(_parameters);

                if ((_parameters != null) && (_parameters.Count > 0))
                {
                    _parameterChanged = false;

                    var keys = new List<VNTagParameter>(_parameters.Keys);
                    foreach (VNTagParameter parameter in keys)
                    {
                        _parameters.UpdateParameter(parameter, RenderParameterField(parameter, _parameters[parameter]));
                        EditorGUILayout.Separator();
                    }
                }
            }
            else
            {
                ResetTag();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(_current.Insert(_cursor, "▾"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            _cursor = EditorGUILayout.IntSlider(_cursor, 0, _current.Length);

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);

            bool deserializationResult = false;
            if (_currentSelectedTag != null)
            {
                if (_parameterChanged || (_serializedTag == null))
                {
                    Debug.unityLogger.logEnabled = false;
                    // re-serialize tag
                    deserializationResult = _currentSelectedTag.Deserialize(_deserializationContext, _parameters.Select(t => t.Value.ToString()).ToArray());
                    _serializedTag = _currentSelectedTag.Serialize(_serializationContext);
                    Debug.unityLogger.logEnabled = true;
                }
                else
                {
                    deserializationResult = !string.IsNullOrEmpty(_serializedTag);
                }
            }

            if (deserializationResult)
            {
                EditorGUILayout.TextArea(CreateNewLine());
            }
            else
            {
                EditorGUILayout.TextArea("");
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup((_currentSelectedTag == null) || !deserializationResult);
            if (GUILayout.Button("Save"))
            {
                Save();
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Close Window"))
            {
                Close();
                
            }
        }

        public static void ShowWindow
            (Action<string> target, VNTagSerializationContext serializationContext, VNTagDeserializationContext deserializationContext, string current = null, EditorWindow parent = null)
        {
            var window = CreateWindow<CreateTagWindow>();
            window.titleContent = new GUIContent($"Create new Tag ({current})");
            window.Init(target, current, serializationContext, deserializationContext);
        }

        private void Init
            (Action<string> target, string current, VNTagSerializationContext serializationContext, VNTagDeserializationContext deserializationContext)
        {
            _serializationContext   = serializationContext;
            _deserializationContext = deserializationContext;
            _current                = current;
            _target                 = target;
            var tags = VNTag.GetAllTagTypes();

            foreach (Type tagType in tags)
            {
                var tag = (VNTag)CreateInstance(tagType);
                if ((tag != null) && tag.EditorVisible())
                {
                    _tags.Add(tag.GetTagName(), tag);
                }
            }
        }

        private string[] GetPotentialTags()
        {
            return _tags.Keys.Prepend("Select a Tag").ToArray();
        }

        private void ResetTag()
        {
            _currentSelectedTag = null;
            _serializedTag      = null;
            _popupIndex.Clear();
        }

        public void Save()
        {
            _target.Invoke(CreateNewLine());
            Close();
        }

        private string CreateNewLine()
        {
            return _current.Insert(_cursor, _serializedTag);
        }

        private object RenderParameterField(VNTagParameter parameter, object value)
        {
            object current = value;
            EditorGUILayout.BeginFoldoutHeaderGroup(true, parameter.Name + (parameter.Optional ? "" : "*"));

            if ((parameter.EnumType != null) && parameter.EnumType.IsEnum)
            {
                bool isFlags = parameter.EnumType.GetCustomAttribute<FlagsAttribute>() != null;

                if (value is string sParameter)
                {
                    if (string.IsNullOrEmpty(sParameter))
                    {
                        value = Activator.CreateInstance(parameter.EnumType);
                    }
                    else if (!Enum.TryParse(parameter.EnumType, sParameter, true, out value))
                    {
                        Debug.LogWarning("CreateTagWindow: OnGUI: failed to parse Enum, " + parameter);
                        return value;
                    }
                }

                var enumInstance = (Enum)Enum.ToObject(parameter.EnumType, value);
                if (isFlags)
                {
                    value = EditorGUILayout.EnumFlagsField(enumInstance);
                }
                else
                {
                    value = EditorGUILayout.EnumPopup(enumInstance);
                }
            }
            else if ((parameter.Options != null) && (parameter.Options.Length > 0))
            {
                int index = parameter.Number;
                _popupIndex.TryAdd(parameter.Number, new ValueTuple<int, bool>(index, false));

                (int index, bool freeInput) popup = _popupIndex[index];
                if (popup.freeInput)
                {
                    value = EditorGUILayout.TextField((string)value);
                }
                else
                {
                    popup.index = EditorGUILayout.Popup(popup.index, parameter.Options);
                    value       = parameter.Options[popup.index];
                }

                popup.freeInput    = EditorGUILayout.Toggle("Free input", popup.freeInput);
                _popupIndex[index] = popup;
            }
            else
            {
                value = DisplayFieldOfTypeCode(parameter, value);
            }

            EditorGUILayout.LabelField(parameter.Description, EditorStyles.helpBox);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Separator();

            if (current != value)
            {
                _parameterChanged = true;
            }

            return value;
        }

        private object DisplayFieldOfTypeCode(VNTagParameter parameter, object value)
        {
            switch (parameter.Type)
            {
                case TypeCode.Boolean:
                    return EditorGUILayout.Toggle((bool)value);
                case TypeCode.Byte:
                    return (byte)EditorGUILayout.IntField((byte)value);
                case TypeCode.Char:
                    return EditorGUILayout.TextField((string)value).FirstChar().GetValueOrDefault();
                case TypeCode.DateTime:
                    return EditorGUILayout.TextField((string)value);
                case TypeCode.DBNull:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what this is or what you're trying to do, " + parameter);
                    break;
                case TypeCode.Decimal:
                    return (decimal)EditorGUILayout.DoubleField((double)value);
                case TypeCode.Double:
                    return EditorGUILayout.DoubleField((double)value);
                case TypeCode.Empty:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what you're trying to do, parameter type is empty, " + parameter);
                    break;
                case TypeCode.Int16:
                    return (short)EditorGUILayout.IntField((short)value);
                case TypeCode.Int32:
                    return EditorGUILayout.IntField((int)value);
                case TypeCode.Int64:
                    return EditorGUILayout.LongField((long)value);
                case TypeCode.Object:
                    // Debug.LogWarning("CreateTagWindow: OnGUI: Object references are unsupported, use a different system to retrieve object instead, "
                    //                + parameter);
                    return VNTagTextArea.TextAreaWithTagCreationDropDown(t => value = t, (string)value);
                    
                    break;
                case TypeCode.SByte:
                    return (sbyte)EditorGUILayout.IntField((sbyte)value);
                case TypeCode.Single:
                    return EditorGUILayout.FloatField((float)value);
                case TypeCode.String:
                    return EditorGUILayout.TextField((string)value);
                case TypeCode.UInt16:
                    return EditorGUILayout.IntField((int)value);
                case TypeCode.UInt32:
                    return EditorGUILayout.IntField((int)value);
                case TypeCode.UInt64:
                    ulong.TryParse(EditorGUILayout.TextField(((ulong)value).ToString()), out ulong result);
                    return result;
                default:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Unrecognised typecode, " + parameter);
                    break;
            }

            return null;
        }
    }
}