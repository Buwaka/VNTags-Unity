using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PropertyRequireInterface))]
public class PropertyRequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PropertyRequireInterface attr = attribute as PropertyRequireInterface;
        
        // Draw the object field as a normal UnityEngine.Object
        property.objectReferenceValue = EditorGUI.ObjectField(
            position, 
            label, 
            property.objectReferenceValue, 
            typeof(UnityEngine.Object), 
            false);

        // Check if the assigned object implements the interface
        if (property.objectReferenceValue != null && attr != null)
        {
            if (!attr.InterfaceType.IsAssignableFrom(property.objectReferenceValue.GetType()))
            {
                Debug.LogError($"{property.objectReferenceValue.name} does not implement {attr.InterfaceType.Name}");
                property.objectReferenceValue = null;
            }
        }
    }
}
