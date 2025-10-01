﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VNTags.Utility;

 namespace VNTags.Editor
 {
     [CustomPropertyDrawer(typeof(VNTagEditorAttribute), true)]
    public class VNTag_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, 
                                  "The [StringOnly] attribute is only valid for string fields.", 
                                  MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(property.displayName, GUILayout.Width(100));
                property.stringValue = EditorGUILayout.TextArea(property.stringValue, GUILayout.ExpandHeight(true));

                if (EditorGUILayout.DropdownButton(new GUIContent("test"), FocusType.Passive, GUILayout.Width(20)))
                {
                    ShowTextBoxContextMenu((string tag) =>
                    {
                        property.stringValue = tag;
                        property.serializedObject.ApplyModifiedProperties();
                    }, property.stringValue);
                }
                EditorGUILayout.EndHorizontal();
            }
            
        }
        
        private void ShowTextBoxContextMenu(Action<string> target, string current)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create new Tag"), false, () => CreateTagWindow.ShowWindow(target, current));

            menu.ShowAsContext();
        }
    }
 }