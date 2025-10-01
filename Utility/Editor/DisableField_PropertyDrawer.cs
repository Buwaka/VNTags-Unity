using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
    [CustomPropertyDrawer(typeof(DisableFieldAttribute))]
    public class DisableField_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}