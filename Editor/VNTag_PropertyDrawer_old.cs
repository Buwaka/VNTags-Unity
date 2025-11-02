// namespace VNTags.Editor
// {
//     [CustomPropertyDrawer(typeof(VNTag), true)]
//     public class VNTag_PropertyDrawer : PropertyDrawer
//     {
//         private static readonly Dictionary<object, VNTag_PropertyDrawer>      _propertyDrawers = new();
//         private readonly        Dictionary<uint, (int index, bool freeInput)> _popupIndex      = new();
//
//         private readonly SortedDictionary<string, VNTag> _tags = new();
//         private          bool                            _initialized;
//         private          int                             _lastIndex;
//         private          VNTagParameters                 _parameters = new();
//         private          string                          _serializedTag;
//         private          int                             _tagIndex;
//
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return base.GetPropertyHeight(property, label);
//         }
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             // in case we ever want to try and extract which tag we're trying to draw instead of just generically any tag
//             // if (property.managedReferenceFieldTypename.Contains(typeof(VNTag).FullName))
//             // {
//             //     
//             // }
//
//             object value = null;
//             if (property.propertyType == SerializedPropertyType.ManagedReference)
//             {
//                 // This is a SerializeReference field, so use managedReferenceValue.
//                 // Example: property.managedReferenceValue = new MyManagedReferenceType();
//                 value = property.managedReferenceValue;
//             }
//             else if (property.propertyType == SerializedPropertyType.ObjectReference)
//             {
//                 // This is a PPtr field, so use objectReferenceValue.
//                 // The error indicates this is the case for you.
//                 // Example: property.objectReferenceValue = someVNTagInstance;
//                 value = property.objectReferenceValue;
//             }
//
//             bool  resultChange = false;
//             VNTag result       = VNTagPropertyDrawer(property);
//             if (result != null)
//             {
//                 resultChange = true;
//             }
//
//
//             if (result == null)
//             {
//                 return;
//             }
//
//
//             if (resultChange)
//             {
//                 if (property.propertyType == SerializedPropertyType.ManagedReference)
//                 {
//                     throw new NotImplementedException();
//                 }
//
//                 if (property.propertyType == SerializedPropertyType.ObjectReference)
//                 {
//                     // This is a PPtr field, so use objectReferenceValue.
//                     // The error indicates this is the case for you.
//                     // Example: property.objectReferenceValue = someVNTagInstance;
//
//                     // if (AssetDatabase.IsSubAsset(result))
//                     // {
//                     //     AssetDatabase.RemoveObjectFromAsset(result);
//                     // }
//                     //
//                     // if (!AssetDatabase.IsSubAsset(result))
//                     // {
//                     //     AssetDatabase.AddObjectToAsset(result, property.serializedObject.targetObject);
//                     // }
//
//                     SubAssetExtension.UpdateSubAsset(property, result);
//
//                     // property.objectReferenceValue = result;
//                 }
//             }
//
//
//             // if (PrefabUtility.GetPrefabType(property.serializedObject.targetObject) == PrefabType.Prefab)
//             // {
//             //     AssetDatabase.AddObjectToAsset(tag, property.serializedObject.targetObject);
//             // }
//
//             // property.serializedObject.ApplyModifiedProperties();
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="target">Either a SerializedProperty or a VNTagParameter</param>
//         /// <param name="tagType">optional, in case this tag can only be of a certain type</param>
//         /// <returns></returns>
//         public static VNTag VNTagPropertyDrawer(object target, Type tagType = null)
//         {
//             VNTag_PropertyDrawer drawer;
//             VNTag                result;
//
//             if (target == null)
//             {
//                 Debug.LogError("VNTag_PropertyDrawer: VNTagPropertyDrawer: target is null, either provide a VNTag or a parameter to associate the propertyDrawer with, aborting");
//                 return null;
//             }
//             
//             if (target is SerializedProperty prop)
//             {
//                 if (_propertyDrawers.TryGetValue(prop.propertyPath, out drawer))
//                 {
//                     result = drawer._VNTagPropertyDrawer(null, tagType);
//                 }
//                 else
//                 {
//                     drawer = new VNTag_PropertyDrawer();
//                     drawer.Init();
//                     result = drawer._VNTagPropertyDrawer(null, tagType);
//                     _propertyDrawers.Add(prop.propertyPath, drawer);
//                 }
//             }
//             else if (target is VNTagParameter parameter)
//             {
//                 if (_propertyDrawers.TryGetValue(parameter.ID, out drawer))
//                 {
//                     result = drawer._VNTagPropertyDrawer(null, tagType);
//                 }
//                 else
//                 {
//                     drawer = new VNTag_PropertyDrawer();
//                     result = drawer._VNTagPropertyDrawer(null, tagType);
//                     _propertyDrawers.Add(parameter.ID, drawer);
//                 }
//
//                 if (result != null)
//                 {
//                     _propertyDrawers.Remove(parameter.ID);
//                 }
//             }
//             else
//             {
//                 Debug.LogError("VNTag_PropertyDrawer: VNTagPropertyDrawer: target is not a VNTagParameter or a serializedProperty, aborting");
//                 return null;
//             }
//
//             return result;
//         }
//
//         public VNTag _VNTagPropertyDrawer(VNTag targetTag, Type tagType = null)
//         {
//             if (!_initialized)
//             {
//                 Init();
//             }
//
//
//             if ((tagType != null) && (targetTag == null))
//             {
//                 targetTag = (VNTag)ScriptableObject.CreateInstance(tagType);
//                 targetTag._init(GetHashCode(), "");
//
//                 if (targetTag == null)
//                 {
//                     Debug.LogError("VNTag_PropertyDrawer: VNTagPropertyDrawer: targetTag is null and tagType is null, aborting");
//                     return null;
//                 }
//             }
//
//
//             EditorGUILayout.Separator();
//
//             if (tagType != null)
//             {
//                 EditorGUILayout.LabelField(targetTag.GetTagName());
//                 _parameters = targetTag.GetParameters(_parameters);
//             }
//             else
//             {
//                 EditorGUILayout.LabelField("VNTag");
//                 string[] tagNames = GetPotentialTags();
//                 _tagIndex = EditorGUILayout.Popup(_tagIndex, tagNames);
//
//                 bool tagChanged = _lastIndex != _tagIndex;
//                 _lastIndex = _tagIndex;
//
//                 // index 0 the placeholder display value and thus not a tag
//                 if (_tagIndex != 0)
//                 {
//                     if (tagChanged || (targetTag == null))
//                     {
//                         targetTag      = _tags[tagNames[_tagIndex]];
//                         _serializedTag = null;
//                         _parameters    = targetTag.GetParameters();
//                     }
//                 }
//                 else
//                 {
//                     return null;
//                 }
//             }
//
//             bool parameterChanged = false;
//             if (_parameters.Count > 0)
//             {
//                 EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
//                 RenderParameters(_parameters, out parameterChanged);
//                 if (parameterChanged && (targetTag != null))
//                 {
//                     _parameters = targetTag.GetParameters(_parameters);
//                 }
//             }
//
//
//             if (targetTag != null)
//             {
//                 EditorGUILayout.Separator();
//                 EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
//                 bool deserializationResult = false;
//                 if (parameterChanged || (_serializedTag == null))
//                 {
//                     // Debug.unityLogger.logEnabled = false;
//                     // re-serialize tag
//                     deserializationResult = targetTag.Deserialize(new VNTagDeserializationContext(), _parameters.Select(t => t.Value?.ToString()).ToArray());
//                     _serializedTag        = targetTag.Serialize(new VNTagSerializationContext());
//                     // Debug.unityLogger.logEnabled = true;
//                 }
//                 else
//                 {
//                     deserializationResult = !string.IsNullOrEmpty(_serializedTag);
//                 }
//
//                 EditorGUI.BeginDisabledGroup(true);
//                 if (deserializationResult)
//                 {
//                     EditorGUILayout.TextArea(_serializedTag);
//                 }
//                 else
//                 {
//                     EditorGUILayout.TextArea("");
//                 }
//
//                 EditorGUI.EndDisabledGroup();
//             }
//
//             EditorGUILayout.Separator();
//
//             return targetTag;
//         }
//
//         private void RenderParameters(VNTagParameters parameters, out bool hasChanged)
//         {
//             hasChanged = false;
//             foreach (VNTagParameter parameter in parameters.Keys.ToList())
//             {
//                 parameters[parameter] = RenderParameterField(parameter, parameters[parameter], out bool changed);
//                 EditorGUILayout.Separator();
//                 hasChanged |= changed;
//             }
//         }
//
//
//         private void Init()
//         {
//             _initialized = true;
//             var tags = VNTag.GetAllTagTypes();
//
//             foreach (Type tagType in tags)
//             {
//                 var tag = (VNTag)ScriptableObject.CreateInstance(tagType);
//                 tag._init(GetHashCode(), "");
//                 if ((tag != null) && tag.EditorVisibility())
//                 {
//                     _tags.Add(tag.GetTagName(), tag);
//                 }
//             }
//         }
//
//         private string[] GetPotentialTags()
//         {
//             return _tags.Keys.Prepend("Select a Tag").ToArray();
//         }
//
//         private void ResetTag()
//         {
//             _serializedTag = null;
//             _popupIndex.Clear();
//         }
//
//         private object RenderParameterField(VNTagParameter parameter, object value, out bool valueChanged)
//         {
//             valueChanged = false;
//             object current = value;
//
//             EditorGUI.indentLevel++;
//             EditorGUILayout.LabelField("➤" + parameter.Name + (parameter.Optional ? "" : "*"));
//
//             if ((parameter.EnumType != null) && parameter.EnumType.IsEnum)
//             {
//                 bool isFlags = parameter.EnumType.GetCustomAttribute<FlagsAttribute>() != null;
//
//                 if (value is string sParameter)
//                 {
//                     if (string.IsNullOrEmpty(sParameter))
//                     {
//                         value = ScriptableObject.CreateInstance(parameter.EnumType);
//                     }
//                     else if (!Enum.TryParse(parameter.EnumType, sParameter, true, out value))
//                     {
//                         Debug.LogWarning("CreateTagWindow: OnGUI: failed to parse Enum, " + parameter);
//                         EditorGUI.indentLevel--;
//                         EditorGUILayout.EndFoldoutHeaderGroup();
//                         return null;
//                     }
//                 }
//
//                 var enumInstance = (Enum)Enum.ToObject(parameter.EnumType, value ?? 0);
//                 if (isFlags)
//                 {
//                     value = EditorGUILayout.EnumFlagsField(enumInstance);
//                 }
//                 else
//                 {
//                     value = EditorGUILayout.EnumPopup(enumInstance);
//                 }
//             }
//             else if ((parameter.Options != null) && (parameter.Options.Length > 0))
//             {
//                 _popupIndex.TryAdd(parameter.Number, new ValueTuple<int, bool>(0, false));
//
//                 (int index, bool freeInput) popup = _popupIndex[parameter.Number];
//                 if (popup.freeInput)
//                 {
//                     value = EditorGUILayout.TextField((string)value);
//                 }
//                 else
//                 {
//                     popup.index = EditorGUILayout.Popup(popup.index, parameter.Options);
//                     value       = parameter.Options[popup.index];
//                 }
//
//                 popup.freeInput               = EditorGUILayout.Toggle("Free input", popup.freeInput);
//                 _popupIndex[parameter.Number] = popup;
//             }
//             else
//             {
//                 value = DisplayFieldOfTypeCode(parameter, value);
//             }
//
//             EditorGUILayout.LabelField(parameter.Description, EditorStyles.helpBox);
//             EditorGUILayout.Separator();
//
//             if (((value   != null) && (current != null) && !value.ToString().Equals(current.ToString()))
//              || ((value   == null) && (current != null))
//              || ((current == null) && (value   != null)))
//             {
//                 valueChanged = true;
//             }
//
//             EditorGUI.indentLevel--;
//             return value;
//         }
//
//         private object DisplayFieldOfTypeCode(VNTagParameter parameter, object value)
//         {
//             switch (parameter.Type)
//             {
//                 case TypeCode.Boolean:
//                     return EditorGUILayout.Toggle((bool)value);
//                 case TypeCode.Byte:
//                     return (byte)EditorGUILayout.IntField((byte)value);
//                 case TypeCode.Char:
//                     return EditorGUILayout.TextField((string)value).FirstChar().GetValueOrDefault();
//                 case TypeCode.DateTime:
//                     return EditorGUILayout.TextField((string)value);
//                 case TypeCode.DBNull:
//                     Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what this is or what you're trying to do, " + parameter);
//                     break;
//                 case TypeCode.Decimal:
//                     return (decimal)EditorGUILayout.DoubleField((double)value);
//                 case TypeCode.Double:
//                     return EditorGUILayout.DoubleField((double)value);
//                 case TypeCode.Empty:
//                     Debug.LogWarning("CreateTagWindow: OnGUI: Not sure what you're trying to do, parameter type is empty, " + parameter);
//                     break;
//                 case TypeCode.Int16:
//                     return (short)EditorGUILayout.IntField((short)value);
//                 case TypeCode.Int32:
//                     return EditorGUILayout.IntField((int)value);
//                 case TypeCode.Int64:
//                     return EditorGUILayout.LongField((long)value);
//                 case TypeCode.Object:
//                     if (value is VNTag tag)
//                     {
//                         return VNTagPropertyDrawer(tag);
//                     }
//
//                     return VNTagPropertyDrawer(parameter);
//                 case TypeCode.SByte:
//                     return (sbyte)EditorGUILayout.IntField((sbyte)value);
//                 case TypeCode.Single:
//                     return EditorGUILayout.FloatField((float)value);
//                 case TypeCode.String:
//                     return EditorGUILayout.TextField((string)value);
//                 case TypeCode.UInt16:
//                     return EditorGUILayout.IntField((int)value);
//                 case TypeCode.UInt32:
//                     return EditorGUILayout.IntField((int)value);
//                 case TypeCode.UInt64:
//                     ulong.TryParse(EditorGUILayout.TextField(((ulong)value).ToString()), out ulong result);
//                     return result;
//                 default:
//                     Debug.LogWarning("CreateTagWindow: OnGUI: Unrecognised typecode, " + parameter);
//                     break;
//             }
//
//             return null;
//         }
//     }
// }

