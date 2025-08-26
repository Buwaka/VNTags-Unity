#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
    [CustomPropertyDrawer(typeof(PropertyRequireComponent))]
    public class PropertyRequireComponent_PropertyDrawer : PropertyDrawer
    {
        private const float HelpBoxHeight = 40f;
        private       bool  HelpBoxActive;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var PropertyRect = new Rect(position.x,
                                        position.y,
                                        position.width,
                                        EditorGUIUtility.singleLineHeight);
            var HelpBoxRect = new Rect(position.x,
                                       position.y + EditorGUIUtility.singleLineHeight,
                                       position.width,
                                       HelpBoxHeight);

            var  requiredAttribute = attribute as PropertyRequireComponent;
            Type requiredType      = requiredAttribute.Type;

            EditorGUI.PropertyField(PropertyRect, property, label);

            if (property.boxedValue != null)
            {
                // Check if the property is a GameObject
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    // Get the assigned GameObject
                    var assignedObject = property.objectReferenceValue as GameObject;

                    // Perform validation only if a GameObject is assigned
                    if ((assignedObject != null) && (assignedObject.GetComponent(requiredType) != null))
                    {
                        HelpBoxActive = false;
                    }
                    else
                    {
                        EditorGUI.HelpBox(HelpBoxRect,
                                          "Object does not have component of type: " + requiredType.Name,
                                          MessageType.Error);
                        HelpBoxActive = true;
                        // property.objectReferenceValue = null;
                    }
                }
                else
                {
                    EditorGUI.HelpBox(HelpBoxRect,
                                      "Not a reference type",
                                      MessageType.Error);
                    HelpBoxActive = true;
                    // property.objectReferenceValue = null;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Start with the default height of the property field.
            float height = base.GetPropertyHeight(property, label);

            // Add the height of the HelpBox if a property is assigned.
            if (HelpBoxActive)
            {
                height += HelpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}
#endif