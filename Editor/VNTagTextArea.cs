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
                ShowTextBoxContextMenu((t) => tag.SetValue(t), tag.GetValue(), tag,  line);
            }

            EditorGUILayout.EndHorizontal();
            return result;
        }
        
        public static string TextAreaWithTagCreationDropDown(Action<string> target, string current)
        {
            return TextAreaWithTagCreationDropDown(null, target, current);
        }
        
        public static string TextAreaWithTagCreationDropDown(string label, Action<string> target, string current)
        {
            EditorGUILayout.BeginHorizontal();
            if (label != null)
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
            }

            // Get the new value from the text area. This 'newValue' is what the user has typed in this frame.
            string newValue = EditorGUILayout.TextArea(current);

            // If the value has changed (either by user typing or external update via dropdown),
            // invoke the target action to update the external string.
            if (newValue != current)
            {
                target.Invoke(newValue);
            }

            if (EditorGUILayout.DropdownButton(new GUIContent("test"), FocusType.Passive, GUILayout.Width(20)))
            {
                // Pass the original target action and the current value to the context menu.
                // When the context menu eventually invokes 'target', it will update the external variable.
                ShowTextBoxContextMenu(target, current);
            }

            EditorGUILayout.EndHorizontal();
            // Return the current value, which should be updated by 'target' if something changed.
            return current;
        }
        
        private static  void ShowTextBoxContextMenu(Action<string> target, string current, IEditorTag tag = null, VNTagScriptLine_base line = null)
        {
            var menu                 = new GenericMenu();

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