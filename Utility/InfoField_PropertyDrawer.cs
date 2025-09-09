# if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
    [CustomPropertyDrawer(typeof(InfoFieldAttribute))]
    public class InfoField_PropertyDrawer : PropertyDrawer
    {
        private float _height           = 18f;
        private Color _backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f); // Dark grey
        private int   _margin          = 5;

        public override void  OnGUI(Rect                           position, SerializedProperty property, GUIContent label)
        {
            if(attribute is InfoFieldAttribute attr)
            {
                string text       = attr.Info;
                
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.wordWrap = true;
                _height        = style.CalcHeight(new GUIContent(text), position.width);
                Rect labelRect = new Rect(position.x - _margin, position.y - _margin, position.width + _margin, _height + _margin);

                EditorGUI.DrawRect(labelRect, _backgroundColor);
                EditorGUI.LabelField(labelRect, new GUIContent(text), style);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent         label)
        {
            return _height;
        }
    }
}
#endif