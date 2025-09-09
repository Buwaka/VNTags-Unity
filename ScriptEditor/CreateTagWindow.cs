using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility;
using Object = System.Object;

namespace VNTags.Editor
{
#if UNITY_EDITOR
    public class CreateTagWindow : EditorWindow
    {
        private readonly List<Object>                                 _parameters = new();
        private readonly Dictionary<int, (int index, bool freeInput)> _popupIndex = new();
        private readonly SortedDictionary<string, VNTag>              _tags       = new();
        private          VNTag                                        _currentSelectedTag;
        private          int                                          _cursor;

        private int           _lastIndex;
        private VNTagEditLine _lineEditor;
        private bool          _parameterChanged;
        private string        _serializedTag;
        private int           _tagIndex;
        private DialogueTag   _targetTag;
        private bool          _addNew;

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
                }

                var parameters = _currentSelectedTag.GetParameters(_parameters);

                if ((parameters != null) && (parameters.Length > 0))
                {
                    if (tagChanged)
                    {
                        _parameters.Clear();
                        foreach (VNTagParameter parameter in parameters)
                        {
                            _parameters.Add(parameter.DefaultValue ?? TypeCodeUtil.GetDefaultInstance(parameter.Type));
                        }
                    }

                    // EditorGUILayout.BeginVertical();
                    _parameterChanged = false;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        VNTagParameter parameter = parameters[i];
                        RenderParameterField(i, parameter);
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
            EditorGUILayout.TextArea(_targetTag.Dialogue.Insert(_cursor, "▾"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            _cursor = EditorGUILayout.IntSlider(_cursor, 0, _targetTag.Dialogue.Length);

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);

            bool deserializationResult = false;
            if (_currentSelectedTag != null)
            {
                if (_parameterChanged || (_serializedTag == null))
                {
                    Debug.unityLogger.logEnabled = false;
                    // re-serialize tag
                    deserializationResult = _currentSelectedTag.Deserialize(_lineEditor.CreateDeserializationContext(_targetTag),
                                                                            _parameters.Select(t => t.ToString()).ToArray());
                    _serializedTag               = _currentSelectedTag.Serialize(_lineEditor.SerializationContext);
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

        public static void ShowWindow(DialogueTag dialogueTag, VNTagEditLine line)
        {
            var window = GetWindow<CreateTagWindow>("Create new Tag");
            window.Init(dialogueTag, line);
        }

        private void Init(DialogueTag dialogueTag, VNTagEditLine line)
        {
            _targetTag  = dialogueTag;
            _lineEditor = line;
            var tags = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(VNTag)));

            foreach (Type tagType in tags)
            {
                var tag = (VNTag)Activator.CreateInstance(tagType);
                if ((tag != null) && tag.EditorVisibility())
                {
                    _tags.Add(tag.GetTagName(), tag);
                }
            }

            if (_addNew)
            {
                // todo
                string sTag = dialogueTag.Serialize(line.SerializationContext);
                _currentSelectedTag = VNTagDeserializer.ParseTag(sTag, line.CreateDeserializationContext(dialogueTag));
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
            // todo
            _targetTag.SetDialogue(CreateNewLine());
            Close();
        }

        private string CreateNewLine()
        {
            return _targetTag.Dialogue.Insert(_cursor, _serializedTag);
        }

        private void RenderParameterField(int index, VNTagParameter parameter)
        {
            object value = _parameters[index];

            EditorGUILayout.BeginFoldoutHeaderGroup(true, parameter.Name);

            if ((parameter.EnumType != null) && parameter.EnumType.IsEnum)
            {
                bool isFlags = parameter.EnumType.GetCustomAttribute<FlagsAttribute>() != null;

                if (_parameters[index] is string sParameter)
                {
                    if (string.IsNullOrEmpty(sParameter))
                    {
                        value = Activator.CreateInstance(parameter.EnumType);
                    }
                    else if (!Enum.TryParse(parameter.EnumType, sParameter, true, out value))
                    {
                        Debug.LogWarning("CreateTagWindow: OnGUI: failed to parse Enum, " + parameter);
                        return;
                    }
                }

                var enumInstance = (Enum)Enum.ToObject(parameter.EnumType, value);
                if (isFlags)
                {
                    _parameters[index] = EditorGUILayout.EnumFlagsField(enumInstance);
                }
                else
                {
                    _parameters[index] = EditorGUILayout.EnumPopup(enumInstance);
                }
            }
            else if ((parameter.Options != null) && (parameter.Options.Length > 0))
            {
                _popupIndex.TryAdd(index, new ValueTuple<int, bool>(index, false));

                (int index, bool freeInput) popup = _popupIndex[index];
                if (popup.freeInput)
                {
                    _parameters[index] = EditorGUILayout.TextField((string)_parameters[index]);
                }
                else
                {
                    popup.index        = EditorGUILayout.Popup(popup.index, parameter.Options);
                    _parameters[index] = parameter.Options[popup.index];
                }

                popup.freeInput    = EditorGUILayout.Toggle("Free input", popup.freeInput);
                _popupIndex[index] = popup;
            }
            else
            {
                _parameters[index] = DisplayFieldOfTypeCode(parameter, _parameters[index]);
            }

            EditorGUILayout.LabelField(parameter.Description, EditorStyles.helpBox);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Separator();

            if (_parameters[index] != value)
            {
                _parameterChanged = true;
            }
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
                    Debug.LogWarning("CreateTagWindow: OnGUI: Object references are unsupported, use a different system to retrieve object instead, "
                                   + parameter);
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
    #endif
}