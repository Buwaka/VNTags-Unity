using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
    [CustomPropertyDrawer(typeof(SortingLayerPicker))]
    public class SortingLayerPicker_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var id = property.FindPropertyRelative("id");

            var layers = SortingLayer.layers.Select(layer => layer.name).ToArray();

            var index = SortingLayer.GetLayerValueFromID(id.intValue) - SortingLayer.GetLayerValueFromID(SortingLayer.layers[0].id);
            index = Mathf.Clamp(index, 0, layers.Length - 1);
            index = EditorGUI.Popup(position, label.text, index, layers);
          
            id.intValue = SortingLayer.layers[index].id;
        }

    }
}