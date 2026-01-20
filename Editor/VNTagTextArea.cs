using System;
using UnityEditor;
using UnityEngine;

namespace VNTags.Editor
{
    public static class VNTagTextArea
    {
        public static string TextAreaWithTagCreationDropDown(IEditorTag tag, VNTagScriptLine_base line)
        {
            return TextAreaWithTagCreationDropDown(null, tag, line);
        }

        public static string TextAreaWithTagCreationDropDown(string label, IEditorTag tag, VNTagScriptLine_base line = null)
        {
            EditorGUILayout.BeginHorizontal();
            if (label != null)
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
            }

            string result = EditorGUILayout.TextArea(tag.GetValue());
            tag.SetValue(result);

            if (EditorGUILayout.DropdownButton(new GUIContent("test"), FocusType.Passive, GUILayout.Width(20)))
            {
                ShowTextBoxContextMenu(t => tag.SetValue(t), tag.GetValue(), tag, line);
            }

            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static void TextAreaWithTagCreationDropDown(Action<string> target, string current)
        {
            TextAreaWithTagCreationDropDown(null, target, current);
        }

        public static void TextAreaWithTagCreationDropDown(string label, Action<string> target, string current)
        {
            EditorGUILayout.BeginHorizontal();
            if (label != null)
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
            }

            string newValue = EditorGUILayout.TextArea(current);

            if (newValue != current)
            {
                target.Invoke(newValue);
            }

            if (EditorGUILayout.DropdownButton(new GUIContent("test"), FocusType.Passive, GUILayout.Width(20)))
            {
                ShowTextBoxContextMenu(target, current);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void ShowTextBoxContextMenu(Action<string> target, string current, IEditorTag tag = null, VNTagScriptLine_base line = null)
        {
            var menu = new GenericMenu();

            var serializationContext   = new VNTagSerializationContext();
            var deserializationContext = new VNTagDeserializationContext();
            if (tag != null && line != null)
            {
                serializationContext   = line.SerializationContext;
                deserializationContext = line.CreateDeserializationContext(tag);
            }



            menu.AddItem(new GUIContent("Create new Tag"),
                         false,
                         () => CreateTagWindow.ShowWindow(target,
                                                          serializationContext,
                                                          deserializationContext,
                                                          current));
            // menu.AddSeparator("");
            // menu.AddItem(new GUIContent("Do Something Else"), false, () => Debug.Log("Doing something else!"));


            menu.ShowAsContext();
        }
    }
}