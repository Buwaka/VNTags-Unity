using System;
using UnityEditor;
using UnityEngine;

namespace VNTags.Editor
{
    [CustomPropertyDrawer(typeof(VNTagEditorAttribute), true)]
    public class VNTag_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "The [VNTag] attribute is only valid for string fields.", MessageType.Error);
            }
            else
            {
                VNTagTextArea.TextAreaWithTagCreationDropDown(property.displayName, tag =>
                {
                    property.stringValue = tag;
                    property.serializedObject.ApplyModifiedProperties();
                }, property.stringValue); 
            }
        }


    }
}