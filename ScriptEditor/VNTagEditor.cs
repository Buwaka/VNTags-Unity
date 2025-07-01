using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace VNTags
{
    public class VNTagEditLine
    {
        public string Dialogue;
        public string ChangeDialogueName;
        public string ChangeExpression;
        public string ChangeCostume;
        public string ChangeBackground;
        public string RawLine;
        public ICollection<IVNTag> Tags;

        public VNTagEditLine(string rawLine)
        {
            RawLine = rawLine;
            Tags = VNTagParser.ParseLine(RawLine);
        }
    }

    [CustomEditor(typeof(TextAsset))]
    public class VNTagEditor : Editor
    {
        bool isTargetFile = true;

        Dictionary<string, VNTagEditLine> EditingLines = new Dictionary<string, VNTagEditLine>();


        private bool isSignificant(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Trim(' ').Length > 0;
        }

        public void LoadFile()
        {
            if (target is TextAsset)
            {
                string data = ((TextAsset)target).text;
                
                var lines = data.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                foreach (var line in lines)
                {
                    VNTagEditLine editLine = null;
                    if (isSignificant(line))
                    {
                        editLine = new VNTagEditLine(line);
                    }
                    EditingLines.Add(line, editLine);
                }
                
            }
        }
        
        private void OnEnable()
        {
            var path = AssetDatabase.GetAssetPath(target);
            // Check if md file
            if (path.EndsWith(".md", System.StringComparison.OrdinalIgnoreCase))
            {
                isTargetFile = true;
                LoadFile();
            }
            else
            {
                isTargetFile = false;
            }
        }

        public override void OnInspectorGUI()
        {
            if (isTargetFile)
            {
                VNTagInspectorGUI();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }
 
        private void VNTagInspectorGUI()
        {
            // sObject.Update();
            // EditorGUI.EndDisabledGroup();
            //
            // EditorGUILayout.PropertyField(prop, true);
            // sObject.ApplyModifiedProperties();
        }
        
        

    // [MenuItem("Assets/Edit VNTags Script",false,10)]
    // static void VNTagsContextMenu(MenuCommand command)
    // {
    //     Rigidbody body = (Rigidbody)command.context;
    //     body.mass = body.mass * 2;
    //     Debug.Log("Doubled Rigidbody's Mass to " + body.mass + " from Context Menu.");
    //
    // }
    //
    // [MenuItem("Assets/Edit VNTags Script",true,10)]
    // static bool VNTagsContextMenu_Validate()
    // {
    //     return Selection.activeObject.GetType() == typeof(UnityEngine.TextAsset);
    // }

    
    
}
}