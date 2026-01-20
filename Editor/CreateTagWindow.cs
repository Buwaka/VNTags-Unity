using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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

                if (_parameters != null && _parameters.Count > 0)
                {
                    _parameterChanged = false;

                    var keys = new List<VNTagParameter>(_parameters.Keys);
                    foreach (VNTagParameter parameter in keys)
                    {
                        string          currentValue = _parameters[parameter];
                        CreateTagWindow thisWindow   = this;
                        RenderParameterField(
                                             parameter,
                                             currentValue,
                                             newValue =>
                                             {
                                                 _parameterChanged |= _parameters.UpdateParameter(parameter, newValue);
                                                 if (_parameterChanged)
                                                 {
                                                     RefreshTagSerialization();
                                                 }
                                             });
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
            EditorGUILayout.TextArea(_current.Insert(_cursor, "▾"), new GUIStyle(EditorStyles.textArea) { wordWrap = true, stretchHeight = true });
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            _cursor = EditorGUILayout.IntSlider(_cursor, 0, _current.Length);

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);


            RefreshTagSerialization();

            EditorGUILayout.TextArea(CreateNewLine(), new GUIStyle(EditorStyles.textArea) { wordWrap = true, stretchHeight = true });


            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(_currentSelectedTag == null || string.IsNullOrEmpty(_serializedTag));
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

        private void RefreshTagSerialization()
        {
            if (_currentSelectedTag != null)
            {
                if (_parameterChanged || _serializedTag == null)
                {
                    Debug.unityLogger.logEnabled = false;
                    // re-serialize tag
                    _currentSelectedTag.Deserialize(_deserializationContext, _parameters.Select(t => t.Value.ToString()).ToArray());
                    _serializedTag               = _currentSelectedTag.Serialize(_serializationContext);
                    Debug.unityLogger.logEnabled = true;
                }
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
                if (tag != null && tag.EditorVisible())
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
            return _current.Insert(_cursor, _serializedTag ?? "");
        }

        private void RenderParameterField(VNTagParameter parameter, string value, Action<string> setValue)
        {
            string current = value;
            EditorGUILayout.BeginFoldoutHeaderGroup(true, parameter.Name + (parameter.Optional ? "" : "*"));

            if (parameter.EnumType != null && parameter.EnumType.IsEnum)
            {
                bool isFlags = parameter.EnumType.GetCustomAttribute<FlagsAttribute>() != null;

                object eValue;
                if (string.IsNullOrEmpty(value))
                {
                    eValue = Activator.CreateInstance(parameter.EnumType);
                }
                else if (!Enum.TryParse(parameter.EnumType, value, true, out eValue))
                {
                    Debug.LogWarning("CreateTagWindow: OnGUI: failed to parse Enum, " + parameter);
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.Separator();
                    return;
                }


                var enumInstance = (Enum)Enum.ToObject(parameter.EnumType, eValue);
                if (isFlags)
                {
                    value = EditorGUILayout.EnumFlagsField(enumInstance).ToString();
                }
                else
                {
                    value = EditorGUILayout.EnumPopup(enumInstance).ToString();
                }
                setValue(value);
            }
            else if (parameter.Options != null && parameter.Options.Length > 0)
            {
                int index = parameter.Number;
                _popupIndex.TryAdd(parameter.Number, new ValueTuple<int, bool>(index, false));

                (int index, bool freeInput) popup = _popupIndex[index];
                if (popup.freeInput)
                {
                    value = EditorGUILayout.TextField(value);
                }
                else
                {
                    popup.index = EditorGUILayout.Popup(popup.index, parameter.Options);
                    value       = parameter.Options[popup.index];
                }

                popup.freeInput    = EditorGUILayout.Toggle("Free input", popup.freeInput);
                _popupIndex[index] = popup;
                setValue(value);
            }
            else
            {
                // Now uses an action instead of relying on a return value
                DisplayFieldOfTypeCode(parameter, value, newValue =>
                {
                    setValue?.Invoke(newValue);
                    _parameterChanged = true;
                });
            }

            EditorGUILayout.LabelField(parameter.Description, EditorStyles.helpBox);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Separator();

            // if (!Equals(current, value))
            // {
            //     _parameterChanged = true;
            //     setValue?.Invoke(value);
            // }
        }

        private void DisplayFieldOfTypeCode(VNTagParameter parameter, string value, Action<string> setValue)
        {
            switch (parameter.Type)
            {
                case TypeCode.Boolean:
                    if (!bool.TryParse(value, out bool boolResult)) boolResult = false;
                    setValue(EditorGUILayout.Toggle(boolResult).ToString());
                    break;
                case TypeCode.Byte:
                    if (!byte.TryParse(value, out byte byteResult)) byteResult = 0;
                    setValue(((byte)Mathf.Clamp(EditorGUILayout.IntField(byteResult), byte.MinValue, byte.MaxValue)).ToString());
                    break;
                case TypeCode.Char:
                    string charResult = EditorGUILayout.TextField(value);
                    setValue(string.IsNullOrEmpty(charResult) ? string.Empty : charResult[0].ToString());
                    break;
                case TypeCode.DateTime:
                    setValue(EditorGUILayout.TextField(value));
                    break;
                case TypeCode.DBNull:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what this is or what you're trying to do, " + parameter);
                    break;
                case TypeCode.Decimal:
                    if (!decimal.TryParse(value, out decimal decimalResult)) decimalResult = 0;
                    setValue(EditorGUILayout.DoubleField((double)decimalResult).ToString());
                    break;
                case TypeCode.Double:
                    if (!double.TryParse(value, out double doubleResult)) doubleResult = 0;
                    setValue(EditorGUILayout.DoubleField(doubleResult).ToString());
                    break;
                case TypeCode.Empty:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what you're trying to do, parameter type is empty, " + parameter);
                    break;
                case TypeCode.Int16:
                    if (!short.TryParse(value, out short int16Result)) int16Result = 0;
                    setValue(((short)Mathf.Clamp(EditorGUILayout.IntField(int16Result), short.MinValue, short.MaxValue)).ToString());
                    break;
                case TypeCode.Int32:
                    if (!int.TryParse(value, out int int32Result)) int32Result = 0;
                    setValue(EditorGUILayout.IntField(int32Result).ToString());
                    break;
                case TypeCode.Int64:
                    if (!long.TryParse(value, out long int64Result)) int64Result = 0;
                    setValue(EditorGUILayout.LongField(int64Result).ToString());
                    break;
                case TypeCode.Object:
                    // Uses action instead of relying on the return value
                    VNTagTextArea.TextAreaWithTagCreationDropDown(
                                                                  setValue,
                                                                  value
                                                                 );
                    break;
                case TypeCode.SByte:
                    if (!sbyte.TryParse(value, out sbyte sbyteResult)) sbyteResult = 0;
                    setValue(((sbyte)Mathf.Clamp(EditorGUILayout.IntField(sbyteResult), sbyte.MinValue, sbyte.MaxValue)).ToString());
                    break;
                case TypeCode.Single:
                    if (!float.TryParse(value, out float singleResult)) singleResult = 0;
                    setValue(EditorGUILayout.FloatField(singleResult).ToString());
                    break;
                case TypeCode.String:
                    setValue(EditorGUILayout.TextField(value));
                    break;
                case TypeCode.UInt16:
                    if (!ushort.TryParse(value, out ushort uint16Result)) uint16Result = 0;
                    setValue(((ushort)Mathf.Clamp(EditorGUILayout.IntField(uint16Result), ushort.MinValue, ushort.MaxValue)).ToString());
                    break;
                case TypeCode.UInt32:
                    if (!uint.TryParse(value, out uint uint32Result)) uint32Result = 0;
                    setValue(((uint)Mathf.Clamp(EditorGUILayout.LongField(uint32Result), uint.MinValue, uint.MaxValue)).ToString());
                    break;
                case TypeCode.UInt64:
                    if (!ulong.TryParse(value, out ulong uint64Result)) uint64Result = 0;
                    string ulongString                                               = EditorGUILayout.TextField(uint64Result.ToString());
                    if (ulong.TryParse(ulongString, out ulong newUlong))
                    {
                        setValue(newUlong.ToString());
                    }
                    break;
                default:
                    Debug.LogWarning("CreateTagWindow: OnGUI: Unrecognised typecode, " + parameter);
                    break;
            }
        }
    }
}