using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VNTags.Utility;
using File = System.IO.File;
using Object = UnityEngine.Object;


namespace VNTags.Editor
{
    [CustomEditor(typeof(TextAsset))]
    public class VNTagScript_Editor : UnityEditor.Editor
    {
        private static readonly Dictionary<Object, VNTagScriptLine_base[]> EditingLines  = new();
        private                 bool                                       _invalidate   = true;
        private                 bool                                       _isTargetFile = true;

        private void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(target);

            // Check if md file
            if (path.EndsWith(".md", StringComparison.OrdinalIgnoreCase) && _invalidate)
            {
                _isTargetFile = true;
                LoadFile();
            }
            else
            {
                _isTargetFile = false;
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            AssetWatcher.WatchAsset(target, InvalidateTarget);
            return base.CreateInspectorGUI();
        }

        private void OnDestroy()
        {
            AssetWatcher.UnwatchAsset(target, InvalidateTarget);
        }

        public void InvalidateTarget()
        {
            _invalidate = true;
        }


        public void LoadFile()
        {
            if (target is TextAsset asset)
            {
                string data = asset.text;

                string[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                var editLines = new List<VNTagScriptLine_base>(lines.Length);

                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];
                    if (VNTagDeserializer.IsSignificant(line))
                    {
                        editLines.Add(new VNTagScriptLine_base(line, (ushort)(index + 1)));
                    }
                }

                for (int index = 0; index < editLines.Count; index++)
                {
                    VNTagScriptLine_base baseLine = editLines[index];
                    if (baseLine.IsSceneSetupLine())
                    {
                        editLines[index] = new VNTagSceneSetupLine(baseLine);
                    }
                    else
                    {
                        editLines[index] = new VNTagGenericScriptLine(baseLine);
                    }
                }

                if ((editLines.Count == 0) || !editLines.First().IsSceneSetupLine())
                {
                    editLines.Insert(0, new VNTagSceneSetupLine("", 0));
                }

                EditingLines[target] = editLines.ToArray();

                Repaint();
            }
        }

        public override void OnInspectorGUI()
        {
            if (_isTargetFile)
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
            EditorGUI.EndDisabledGroup();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                SerializeLines();
            }

            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            var lines = EditingLines[target];

            for (int i = 0; i < lines.Length; i++)
            {
                VNTagScriptLine_base line = lines[i];

                line.Foldout = EditorGUILayout.BeginToggleGroup(line.Preview, line.Foldout);
                if (line.Foldout)
                {
                    line.RenderLine(this);
                }
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Separator();
            }
        }


        public void RenderEmptyPopup(string label)
        {
            var nullVal = Array.Empty<GUIContent>();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.Popup(0, nullVal);
        }


        private void SerializeLines()
        {
            string path = AssetDatabase.GetAssetPath(target);

            if (!string.IsNullOrEmpty(path) && EditingLines.ContainsKey(target))
            {
                try
                {
                    // Get the content from the SerializedProperty
                    var lines  = EditingLines[target];
                    var script = new StringBuilder();
                    foreach (VNTagScriptLine_base line in lines)
                    {
                        if (line == null)
                        {
                            script.AppendLine();
                        }
                        else
                        {
                            script.AppendLine(line.Serialize());
                        }
                    }

                    // Write the new content to the file
                    File.WriteAllText(path, script.ToString());

                    // Tell Unity to re-import the asset so the changes are reflected
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh(); // Refresh the AssetDatabase to ensure consistency
                    InvalidateTarget();

                    Debug.Log($"VNTagEditor: SerializeLines: Successfully overwrote Script at: {path}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"VNTagEditor: SerializeLines: Failed to write to TextAsset at {path}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("Could not get asset path for the target or script has no contents");
            }
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