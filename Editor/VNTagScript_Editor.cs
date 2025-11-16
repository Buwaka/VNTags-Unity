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
                    if (line is VNTagSceneSetupLine setup)
                    {
                        RenderSceneSetupLine(setup);
                    }

                    if (line is VNTagGenericScriptLine scriptLine)
                    {
                        RenderLine(scriptLine);
                    }
                }
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Separator();
            }
        }


        public VNCharacterData[] MaskToCharacterArray(int mask)
        {
            return LayoutHelpers.MaskToValueArray(mask, VNTagsConfig.GetConfig().AllCharacters);
        }


        private void RenderEmptyPopup(string label)
        {
            var nullVal = Array.Empty<GUIContent>();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.Popup(0, nullVal);
        }


        private void RenderSceneSetupLine(VNTagSceneSetupLine setupLine)
        {
            EditorGUILayout.LabelField("Scene Setup");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderPopup("Background",
                                      VNTagsConfig.GetConfig().GetBackgroundNamesGUI("No Background"),
                                      ref setupLine.BackgroundIndex,
                                      VNTagsConfig.GetConfig().GetBackgroundByIndex,
                                      ref setupLine.BackgroundTag.GetBackgroundRef(),
                                      setupLine.Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderMaskMultiSelectPopup("Character(s)",
                                                     VNTagsConfig.GetConfig().GetCharacterNamesGUI(),
                                                     ref setupLine.CharacterMask,
                                                     MaskToCharacterArray,
                                                     ref setupLine.CharacterTag.GetCharacterRef(),
                                                     setupLine.Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderPopup("Transition",
                                      VNTagsConfig.GetConfig().GetTransitionNamesGUI("No Transition"),
                                      ref setupLine.TransitionName,
                                      VNTagsConfig.GetConfig().GetTransitionByNameOrAlias,
                                      ref setupLine.TransitionTag.GetTransitionRef(),
                                      setupLine.Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            // music
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            setupLine.SceneResetFlag = EditorGUILayout.Toggle("Reset Scene", setupLine.SceneResetFlag);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            setupLine.ToggleVNUIFlag =  EditorGUILayout.Toggle("Toggle Textbox", setupLine.ToggleVNUIFlag);;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Separator();
        }

        private void RenderLine(VNTagGenericScriptLine line)
        {
            //dialogue
            foreach (IEditorTag tag in line.ExtraTags)
            {
                VNTagTextArea.TextAreaWithTagCreationDropDown(tag, line);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            // Name
            string nullName = (line.CharacterChangeTag.Character == null) || !line.CharacterChangeTag.Character.IsBlankCharacter()
                ? "Narrator"
                : line.CharacterChangeTag.Character.Name;
            LayoutHelpers.RenderPopup("Character",
                                      VNTagsConfig.GetConfig().GetCharacterNamesGUI(nullName),
                                      ref line.NameIndex,
                                      VNTagsConfig.GetConfig().GetCharacterByIndex,
                                      ref line.CharacterChangeTag.GetCharacterRef(),
                                      line.InvalidateCharacter);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginDisabledGroup(line.CharacterChangeTag.Character == null);
            if (line.CharacterChangeTag.Character == null)
            {
                // Expression
                EditorGUILayout.BeginVertical();
                RenderEmptyPopup("Expression");
                EditorGUILayout.EndVertical();

                //Outfit
                EditorGUILayout.BeginVertical();
                RenderEmptyPopup("Outfit");
                EditorGUILayout.EndVertical();
            }
            else
            {
                // Expression
                EditorGUILayout.BeginVertical();
                LayoutHelpers.RenderPopup("Expression",
                                          line.CharacterChangeTag.Character.GetExpressionNamesGUI("No Expression Change"),
                                          ref line.ExpressionIndex,
                                          line.CharacterChangeTag.Character.GetExpressionByIndex,
                                          ref line.ExpressionChangeTag.GetOutfitRef(),
                                          line.Invalidate);
                EditorGUILayout.EndVertical();


                //Outfit
                EditorGUILayout.BeginVertical();
                LayoutHelpers.RenderPopup("Outfit",
                                          line.CharacterChangeTag.Character.GetOutfitNamesGUI("No Outfit Change"),
                                          ref line.OutfitIndex,
                                          line.CharacterChangeTag.Character.GetOutfitByIndex,
                                          ref line.OutfitChangeTag.GetOutfitRef(),
                                          line.Invalidate);
                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Background");
            line.BackgroundIndex = EditorGUILayout.Popup(line.BackgroundIndex, VNTagsConfig.GetConfig().GetBackgroundNamesGUI("No Background Change"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            RenderEmptyPopup("Sound Effect");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            RenderEmptyPopup("Music");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            // EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Preview");
            EditorGUILayout.SelectableLabel(line.SerializedPreview);
            // EditorGUI.EndDisabledGroup();
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