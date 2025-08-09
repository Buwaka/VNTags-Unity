using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VNTags.Utility;
using File = System.IO.File;
using Object = UnityEngine.Object;

#if UNITY_EDITOR


namespace VNTags
{
    public class VNTagEditLine
    {
        private readonly LinkedList<VNTag> Tags;
        private          BackgroundTag      _backgroundChangeTag;


        private CharacterTag      _characterChangeTag;
        private ExpressionTag     _expressionChangeTag;
        private OutfitTag         _outfitChangeTag;
        public  int               BackgroundIndex;
        public  int               BGMIndex     = 0;
        public  List<DialogueTag> DialogueTags = new();
        public  int               ExpressionIndex;
        public  bool              Foldout;


        public int    NameIndex;
        public int    OutfitIndex;
        public string Preview = "";
        public string RawLine;
        public string SerializedPreview = "";
        public int    SFXIndex          = 0;

        public VNTagEditLine(string rawLine, int lineNumber)
        {
            Preview = lineNumber + ": ";
            RawLine = rawLine;
            Tags    = new LinkedList<VNTag>(VNTagDeserializer.ParseLine(RawLine, (UInt16) lineNumber));


            // filter out starter tags
            foreach (VNTag tag in Tags)
            {
                if (tag is CharacterTag characterTag && (_characterChangeTag == null))
                {
                    _characterChangeTag = characterTag;
                    if (_characterChangeTag.Character != null)
                    {
                        Preview += _characterChangeTag.Character.Name + ": ";
                    }
                }
                else if (tag is ExpressionTag expressionTag)
                {
                    _expressionChangeTag = expressionTag;
                }
                else if (tag is OutfitTag outfitTag)
                {
                    _outfitChangeTag = outfitTag;
                }
                else if (tag is BackgroundTag backgroundTag)
                {
                    _backgroundChangeTag = backgroundTag;
                }
                else if (tag is DialogueTag)
                {
                    break;
                }
            }

            // get all the dialogue tags
            foreach (VNTag tag in Tags)
            {
                if (tag is DialogueTag)
                {
                    var dTag = (DialogueTag)tag;
                    Preview += dTag.Dialogue;
                    DialogueTags.Add(dTag);
                }
            }

            SetIndieces();
            Invalidate();
        }

        public CharacterTag CharacterChangeTag
        {
            get
            {
                if (_characterChangeTag == null)
                {
                    _characterChangeTag = new CharacterTag();
                    Tags.AddFirst(_characterChangeTag);
                }

                return _characterChangeTag;
            }
        }

        public ExpressionTag ExpressionChangeTag
        {
            get
            {
                if (_expressionChangeTag == null)
                {
                    _expressionChangeTag = new ExpressionTag();
                    Tags.AddFirst(_expressionChangeTag);
                }

                return _expressionChangeTag;
            }
        }

        public OutfitTag OutfitChangeTag
        {
            get
            {
                if (_outfitChangeTag == null)
                {
                    _outfitChangeTag = new OutfitTag();
                    Tags.AddFirst(_outfitChangeTag);
                }

                return _outfitChangeTag;
            }
        }

        public BackgroundTag BackgroundChangeTag
        {
            get
            {
                if (_backgroundChangeTag == null)
                {
                    _backgroundChangeTag = new BackgroundTag();
                    Tags.AddFirst(_backgroundChangeTag);
                }

                return _backgroundChangeTag;
            }
        }

        public void InvalidateCharacter()
        {
            ExpressionChangeTag.GetOutfitRef() = null;
            OutfitChangeTag.GetOutfitRef()     = null;
            ExpressionIndex                    = 0;
            OutfitIndex                        = 0;
            Invalidate();
        }

        public void Invalidate()
        {
            SerializedPreview = VNTagSerializer.SerializeLine(Tags);
        }

        public void SetIndieces()
        {
            if ((_characterChangeTag != null) && (_characterChangeTag.Character != null))
            {
                var characterNames = VNTagsConfig.GetConfig().GetCharacterNamesGUI("");
                for (int i = 0; i < characterNames.Length; i++)
                {
                    if (characterNames[i]
                       .text
                       .Equals(_characterChangeTag.Character.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        NameIndex = i;
                        break;
                    }
                }

                var expressionNames = _characterChangeTag.Character.GetExpressionNamesGUI("");
                if ((_expressionChangeTag != null) && (_expressionChangeTag.Expression != null))
                {
                    for (int i = 0; i < expressionNames.Length; i++)
                    {
                        if (expressionNames[i]
                           .text.Equals(_expressionChangeTag.Expression.Name,
                                        StringComparison.OrdinalIgnoreCase))
                        {
                            ExpressionIndex = i;
                            break;
                        }
                    }
                }

                var outfitNames = _characterChangeTag.Character.GetOutfitNamesGUI("");
                if ((_outfitChangeTag != null) && (_outfitChangeTag.Outfit != null))
                {
                    for (int i = 0; i < outfitNames.Length; i++)
                    {
                        if (outfitNames[i]
                           .text
                           .Equals(_outfitChangeTag.Outfit.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            OutfitIndex = i;
                            break;
                        }
                    }
                }
            }

            if (_backgroundChangeTag != null)
            {
                //todo
                var backgrounds = VNTagsConfig.GetConfig().Backgrounds;
                for (int i = 0; i < backgrounds.Length; i++)
                {
                    if (backgrounds[i].Name == _backgroundChangeTag.Background.Name)
                    {
                        BackgroundIndex = i;
                        break;
                    }
                }
            }
        }

        public string Serialize()
        {
            return VNTagSerializer.SerializeLine(Tags);
        }
    }

    [CustomEditor(typeof(TextAsset))]
    public class VNTagEditor : Editor
    {
        private static readonly Dictionary<Object, VNTagEditLine[]> EditingLines  = new();
        private                 bool                                _isTargetFile = true;
        private                 bool                                _invalidate   = true;
        
        public override         VisualElement                       CreateInspectorGUI()
        {
            AssetWatcher.WatchAsset(target, InvalidateTarget);
            return base.CreateInspectorGUI();
        }

        ~VNTagEditor()
        {
            AssetWatcher.UnwatchAsset(target, InvalidateTarget);
        }

        public void InvalidateTarget()
        {
            _invalidate = true;
        }

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


        public void LoadFile()
        {
            if (target is TextAsset asset)
            {
                string data = asset.text;

                string[] lines = data.Split(
                                            new[] { "\r\n", "\r", "\n" },
                                            StringSplitOptions.None
                                           );
                
                EditingLines[target] = new VNTagEditLine[lines.Length];

                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];
                    if (VNTagDeserializer.IsSignificant(line))
                    {
                        EditingLines[target][index] = new VNTagEditLine(line, index + 1);
                    }
                }
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

            foreach (VNTagEditLine line in EditingLines[target])
            {
                if (line != null)
                {
                    line.Foldout = EditorGUILayout.BeginToggleGroup(line.Preview, line.Foldout);
                    // line.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(line.Foldout, line.Preview);
                    if (line.Foldout)
                    {
                        RenderLine(line);
                    }

                    // EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.Separator();
                }
            }
        }

        private void RenderPopup<T>(string       label,
                                    GUIContent[] options,
                                    ref int      index,
                                    Func<int, T> fetcher,
                                    ref T        target,
                                    Action       post)
        {
            EditorGUILayout.PrefixLabel(label);

            int lastIndex = index;
            index = EditorGUILayout.Popup(lastIndex, options);

            if (lastIndex == index)
            {
                return;
            }

            T temp = fetcher(index);
            if (temp != null)
            {
                target = temp;
            }
            else if (index != 0)
            {
                Debug.LogError("VNTagEditor: RenderPopup: Option has no value, '"
                             + options[index]
                             + "' with index "
                             + index);
            }
            else
            {
                target = default;
            }

            post();
        }

        private void RenderEmptyPopup(string label)
        {
            var nullVal = Array.Empty<GUIContent>();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.Popup(0, nullVal);
        }


        private void RenderLine(VNTagEditLine line)
        {
            //dialogue
            foreach (DialogueTag dialogueTag in line.DialogueTags)
            {
                dialogueTag.SetDialogue(EditorGUILayout.TextArea(dialogueTag.Dialogue));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            // Name
            RenderPopup("Character",
                        VNTagsConfig.GetConfig().GetCharacterNamesGUI("Narrator"),
                        ref line.NameIndex,
                        VNTagsConfig.GetConfig().GetCharacterByIndex,
                        ref line.CharacterChangeTag.GetCharacterRef(),
                        line.InvalidateCharacter);

            // EditorGUILayout.PrefixLabel("Character");
            // GUIContent[] names = VNTagsConfig.GetConfig().GetCharacterNamesGUI("Narrator");
            // line.NameIndex = EditorGUILayout.Popup(line.NameIndex, names);
            // var temp = VNTagsConfig.GetConfig().GetCharacterByIndex(line.NameIndex);
            // if (temp.HasValue)
            // {
            //     line.CharacterChangeTag.Character  = temp.Value;
            //     line.Invalidate();
            // }
            // else if(line.NameIndex != 0)
            // {
            //     Debug.LogError("VNTagEditor: VNInspector: Expression has no value, '" + names[line.ExpressionIndex] + "' with index " + line.ExpressionIndex);
            // }

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
                RenderPopup("Expression",
                            line.CharacterChangeTag.Character.GetExpressionNamesGUI("No Expression Change"),
                            ref line.ExpressionIndex,
                            line.CharacterChangeTag.Character.GetExpressionByIndex,
                            ref line.ExpressionChangeTag.GetOutfitRef(),
                            line.Invalidate);

                // EditorGUILayout.PrefixLabel("Expression");
                // var expressions = line.CharacterChangeTag.Character.Value.GetExpressionNamesGUI("No Expression Change");
                // line.ExpressionIndex = EditorGUILayout.Popup(line.ExpressionIndex, expressions);
                //
                // var temp1 = line.CharacterChangeTag.Character.Value.GetExpressionByIndex(line.ExpressionIndex);
                // if (temp1.HasValue)
                // {
                //     line.ExpressionChangeTag.Expression  = temp1.Value;
                //     line.Invalidate();
                // }
                // else if(line.ExpressionIndex != 0)
                // {
                //     Debug.LogError("VNTagEditor: VNInspector: Expression has no value, '" + expressions[line.ExpressionIndex] + "' with index " + line.ExpressionIndex);
                // }
                EditorGUILayout.EndVertical();


                //Outfit
                EditorGUILayout.BeginVertical();
                RenderPopup("Outfit",
                            line.CharacterChangeTag.Character.GetOutfitNamesGUI("No Outfit Change"),
                            ref line.OutfitIndex,
                            line.CharacterChangeTag.Character.GetOutfitByIndex,
                            ref line.OutfitChangeTag.GetOutfitRef(),
                            line.Invalidate);

                // EditorGUILayout.PrefixLabel("Outfit");
                // var outfits = line.CharacterChangeTag.Character.Value.GetOutfitNamesGUI("No Outfit Change");
                // line.OutfitIndex = EditorGUILayout.Popup(line.OutfitIndex, outfits);
                // var temp2 = line.CharacterChangeTag.Character.Value.GetOutfitByIndex(line.OutfitIndex);
                // if (temp2.HasValue)
                // {
                //     line.OutfitChangeTag.Outfit = temp2.Value;
                //     line.Invalidate();
                // }
                // else if(line.OutfitIndex != 0)
                // {
                //     Debug.LogError("VNTagEditor: VNInspector: outfit has no value, '" + outfits[line.OutfitIndex] + "' with index " + line.OutfitIndex);
                // }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Background");
            line.BackgroundIndex = EditorGUILayout.Popup(line.BackgroundIndex,
                                                         VNTagsConfig.GetConfig()
                                                                     .GetBackgroundNamesGUI("No Background Change"));
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
                    foreach (VNTagEditLine line in lines)
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

#endif