using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR


namespace VNTags
{
    public class VNTagEditLine
    {
        public List<DialogueTag> DialogueTags = new List<DialogueTag>();
        public VNCharacter? Character;
        public VNExpression? ChangeExpression;
        public VNOutfit? ChangeOutfit;
        public VNBackground? ChangeBackground;
        public string RawLine;
        public string Preview = "";
        public ICollection<IVNTag> Tags;

        public VNTagEditLine(string rawLine, int lineNumber)
        {
            RawLine = rawLine;
            Tags = VNTagParser.ParseLine(RawLine, lineNumber);

            // filter out starter tags
            foreach (var tag in Tags)
            {
                if (tag is CharacterTag)
                {
                    Character = ((CharacterTag)tag).Character;
                    if (Character.HasValue)
                    {
                        Preview += Character.Value.Name + ": ";
                    }
                }
                else if (tag is ExpressionTag)
                {
                    ChangeExpression = ((ExpressionTag)tag).Expression;
                }
                else if (tag is OutfitTag)
                {
                    ChangeOutfit = ((OutfitTag)tag).Outfit;
                }
                else if (tag is BackgroundTag)
                {
                    ChangeBackground = ((BackgroundTag)tag).Background;
                }
                else if (tag is DialogueTag)
                {
                    break;
                }
            }
            
            // get all the dialogue tags
            foreach (var tag in Tags)
            {
                if (tag is DialogueTag)
                {
                    DialogueTag dTag = (DialogueTag)tag;
                    Preview += dTag.Dialogue;
                    DialogueTags.Add(dTag);
                }
            }

            SetIndieces();
        }

        
        public int NameIndex = 0;
        public int ExpressionIndex = 0;
        public int OutfitIndex = 0;
        public int BackgroundIndex = 0;
        public bool Foldout = false;
        public void SetIndieces()
        {

            if (Character.HasValue)
            {
                if (Character != null)
                {
                    var names = VNTagsConfig.GetConfig().GetCharacterNamesGUI("");
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (names[i].text == Character.Value.Name)
                        {
                            NameIndex = i;
                            break;
                        }
                    }


                    if (ChangeExpression.HasValue)
                    {
                        for (int i = 0; i < Character.Value.Expressions.Length; i++)
                        {
                            if (Character.Value.Expressions[i].Name == ChangeExpression.Value.Name)
                            {
                                ExpressionIndex = i;
                                break;
                            }
                        }
                    }

                    if (ChangeOutfit.HasValue)
                    {
                        for (int i = 0; i < Character.Value.Outfits.Length; i++)
                        {
                            if (Character.Value.Outfits[i].Name == ChangeOutfit.Value.Name)
                            {
                                OutfitIndex = i;
                                break;
                            }
                        }
                    }
                }
            }

            if (ChangeBackground != null)
            {
                var backgrounds = VNTagsConfig.GetConfig().Backgrounds;
                for (int i = 0; i < backgrounds.Length; i++)
                {
                    
                    if (backgrounds[i].Name == ChangeBackground.Value.Name)
                    {
                        BackgroundIndex = i;
                        break;
                    }
                }
            }

        }


    }

    [CustomEditor(typeof(TextAsset))]
    public class VNTagEditor : Editor
    {
        bool isTargetFile = true;

        static Dictionary<UnityEngine.Object, VNTagEditLine[]> EditingLines = new Dictionary<UnityEngine.Object, VNTagEditLine[]>();
        




        public void LoadFile(bool reload = false)
        {
            if (target is TextAsset)
            {
                string data = ((TextAsset)target).text;
                
                var lines = data.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                VNTagEditLine[] editLines;

                if (EditingLines.ContainsKey(target) && !reload)
                {
                    editLines = EditingLines[target];
                }
                else
                {
                    editLines = new VNTagEditLine[lines.Length];
                    EditingLines[target] = editLines;
                }

                for(var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    if (VNTagParser.isSignificant(line))
                    {
                        editLines[index] = new VNTagEditLine(line, index + 1);
                    }
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
            EditorGUI.EndDisabledGroup();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.Button("Save");
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Separator();

            foreach (var line in EditingLines[target])
            {
                if (line != null)
                {
                    line.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(line.Foldout, line.Preview);
                    if (line.Foldout)
                    {
                        RenderLine(line);
                        EditorGUILayout.Separator();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            // sObject.Update();
            // EditorGUI.EndDisabledGroup();
            //
            // EditorGUILayout.PropertyField(prop, true);
            // sObject.ApplyModifiedProperties();
        }

        private void RenderLine(VNTagEditLine line)
        {
            //dialogue
            foreach (var dialogueTag in line.DialogueTags)
            {
                dialogueTag.Dialogue = EditorGUILayout.TextArea(dialogueTag.Dialogue);
            }




            EditorGUILayout.BeginHorizontal();
            
            // Name
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Character");
            GUIContent[] names = VNTagsConfig.GetConfig().GetCharacterNamesGUI("Narrator");
            line.NameIndex = EditorGUILayout.Popup(line.NameIndex, names);
            line.Character = VNTagsConfig.GetConfig().GetCharacterByIndex(line.NameIndex);
            EditorGUILayout.EndVertical();
            
            EditorGUI.BeginDisabledGroup(line.Character == null);
            if (line.Character == null)
            {
                GUIContent[] nullVal = Array.Empty<GUIContent>();
                // Expression
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PrefixLabel("Expression");
                EditorGUILayout.Popup(0, nullVal);
                EditorGUILayout.EndVertical();
                
                //Outfit
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PrefixLabel("Outfit");
                EditorGUILayout.Popup(0, nullVal);
                EditorGUILayout.EndVertical();
            }
            else
            {
                // Expression
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PrefixLabel("Expression");
                line.ExpressionIndex = EditorGUILayout.Popup(line.ExpressionIndex, line.Character.Value.GetExpressionNamesGUI("No Expression Change"));
                line.ChangeExpression = line.Character.Value.GetExpressionByIndex(line.ExpressionIndex);
                EditorGUILayout.EndVertical();


                //Outfit
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PrefixLabel("Outfit");
                line.OutfitIndex = EditorGUILayout.Popup(line.OutfitIndex, line.Character.Value.GetOutfitNamesGUI("No Outfit Change"));
                line.ChangeOutfit = line.Character.Value.GetOutfitByIndex(line.OutfitIndex);
                EditorGUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

        }

        void SerializeLines()
        {
            //todo
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

#endif