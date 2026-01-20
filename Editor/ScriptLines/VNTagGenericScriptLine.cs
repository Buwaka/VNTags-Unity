using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility;

namespace VNTags.Editor
{
    public class VNTagGenericScriptLine : VNTagScriptLine_base
    {
        private BackgroundTag    _backgroundChangeTag;
        private CharacterTag     _characterChangeTag;
        private ExpressionTag    _expressionChangeTag;
        private OutfitTag        _outfitChangeTag;
        public  int              BackgroundIndex;
        public  int              BGMIndex;
        public  int              ExpressionIndex;
        public  List<IEditorTag> ExtraTags = new();
        public  bool             Foldout;
        public  int              NameIndex;
        public  int              OutfitIndex;
        public  int              SFXIndex;

        public VNTagGenericScriptLine(string rawLine, ushort lineNumber) : base(rawLine, lineNumber)
        {
            Init();
        }

        public VNTagGenericScriptLine(VNTagScriptLine_base baseLine) : base(baseLine)
        {
            Init();
        }

        public CharacterTag CharacterChangeTag
        {
            get
            {
                if (_characterChangeTag == null)
                {
                    _characterChangeTag = ScriptableObject.CreateInstance<CharacterTag>();
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
                    _expressionChangeTag = ScriptableObject.CreateInstance<ExpressionTag>();
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
                    _outfitChangeTag = ScriptableObject.CreateInstance<OutfitTag>();
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
                    _backgroundChangeTag = ScriptableObject.CreateInstance<BackgroundTag>();
                    Tags.AddFirst(_backgroundChangeTag);
                }

                return _backgroundChangeTag;
            }
        }

        private void Init()
        {
            Preview = LineNumber + ": ";

            // filter out starter tags
            // todo but then render all the other tags, is gonna be hard to edit them generically, perhaps cast the all as DialogueTags?
            // we have to keep track of the iterator so we don't include the starter tags

            // starter tags
            LinkedList<VNTag>.Enumerator iterator = Tags.GetEnumerator();
            VNTag                        current  = null;

            _characterChangeTag = GetMainCharacter();
            if (_characterChangeTag != null)
            {
                if (_characterChangeTag.Character != null)
                {
                    Preview += _characterChangeTag.Character.Name + ": ";
                }
            }


            while (iterator.MoveNext())
            {
                current = iterator.Current;
                if (current is CharacterTag characterTag)
                {
                }
                else if (current is ExpressionTag expressionTag)
                {
                    if (_expressionChangeTag == null && _characterChangeTag != null && expressionTag.TargetCharacter == _characterChangeTag.Character)
                    {
                        _expressionChangeTag = expressionTag;
                    }
                }
                else if (current is OutfitTag outfitTag && _characterChangeTag != null && outfitTag.TargetCharacter == _characterChangeTag.Character)
                {
                    if (_outfitChangeTag == null)
                    {
                        _outfitChangeTag = outfitTag;
                    }
                }
                else if (current is BackgroundTag backgroundTag)
                {
                    if (_backgroundChangeTag == null)
                    {
                        _backgroundChangeTag = backgroundTag;
                    }
                }
                else
                {
                    break;
                }
            }

            // extra tags
            if (current != null)
            {
                do
                {
                    VNTag tag = iterator.Current;
                    if (tag == null)
                    {
                        continue;
                    }

                    if (tag is DialogueTag dTag)
                    {
                        Preview += dTag.Dialogue;
                        ExtraTags.Add(dTag);
                    }
                    else
                    {
                        var dummy = ScriptableObject.CreateInstance<DummyTag>();
                        dummy._init(VNTagID.GenerateID(LineNumber, (ushort)Tags.Count), RawLine);
                        dummy.Deserialize(new VNTagDeserializationContext(), tag.Serialize(new VNTagSerializationContext(Tags)));
                        ExtraTags.Add(dummy);
                    }
                } while (iterator.MoveNext());
            }

            iterator.Dispose();

            InitUIIndieces();
            Invalidate();
        }

        public override string Serialize()
        {
            var toSerialize = new List<VNTag>(ExtraTags.Cast<VNTag>());
            toSerialize = toSerialize.Prepend(_backgroundChangeTag).Prepend(_outfitChangeTag).Prepend(_expressionChangeTag).Prepend(_characterChangeTag).ToList();
            return VNTagSerializer.SerializeLine(toSerialize);
        }

        public void InvalidateCharacter()
        {
            ExpressionChangeTag.GetOutfitRef() = null;
            OutfitChangeTag.GetOutfitRef()     = null;
            ExpressionIndex                    = 0;
            OutfitIndex                        = 0;
            Invalidate();
        }

        public override void Invalidate()
        {
            SerializedPreview = VNTagSerializer.SerializeLine(Tags);
        }

        public void InitUIIndieces()
        {
            if (_characterChangeTag != null && _characterChangeTag.Character != null)
            {
                var characterNames = VNTagsConfig.GetConfig().GetCharacterNamesGUI("");
                for (int i = 0; i < characterNames.Length; i++)
                {
                    if (characterNames[i].text.Equals(_characterChangeTag.Character.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        NameIndex = i;
                        break;
                    }
                }

                var expressionNames = _characterChangeTag.Character.GetExpressionNamesGUI("");
                if (_expressionChangeTag != null && _expressionChangeTag.Expression != null)
                {
                    for (int i = 0; i < expressionNames.Length; i++)
                    {
                        if (expressionNames[i].text.Equals(_expressionChangeTag.Expression.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            ExpressionIndex = i;
                            break;
                        }
                    }
                }

                var outfitNames = _characterChangeTag.Character.GetOutfitNamesGUI("");
                if (_outfitChangeTag != null && _outfitChangeTag.Outfit != null)
                {
                    for (int i = 0; i < outfitNames.Length; i++)
                    {
                        if (outfitNames[i].text.Equals(_outfitChangeTag.Outfit.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            OutfitIndex = i;
                            break;
                        }
                    }
                }
            }

            if (_backgroundChangeTag != null)
            {
                var backgrounds = VNTagsConfig.GetConfig().GetBackgroundNamesGUI("");
                for (int i = 0; i < backgrounds.Length; i++)
                {
                    if (_backgroundChangeTag.Background == null || backgrounds[i].text.Equals(_backgroundChangeTag.Background.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        BackgroundIndex = i;
                        break;
                    }
                }
            }
        }

        public override void RenderLine(VNTagScript_Editor vnTagScriptEditor)
        {
            //dialogue
            foreach (IEditorTag tag in ExtraTags)
            {
                VNTagTextArea.TextAreaWithTagCreationDropDown(tag, this);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            // Name
            string nullName = CharacterChangeTag.Character == null || !CharacterChangeTag.Character.IsBlankCharacter()
                ? "Narrator"
                : CharacterChangeTag.Character.Name;
            LayoutHelpers.RenderPopup("Character",
                                      VNTagsConfig.GetConfig().GetCharacterNamesGUI(nullName),
                                      ref NameIndex,
                                      VNTagsConfig.GetConfig().GetCharacterByIndex,
                                      ref CharacterChangeTag.GetCharacterRef(),
                                      InvalidateCharacter);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginDisabledGroup(CharacterChangeTag.Character == null);
            if (CharacterChangeTag.Character == null)
            {
                // Expression
                EditorGUILayout.BeginVertical();
                vnTagScriptEditor.RenderEmptyPopup("Expression");
                EditorGUILayout.EndVertical();

                //Outfit
                EditorGUILayout.BeginVertical();
                vnTagScriptEditor.RenderEmptyPopup("Outfit");
                EditorGUILayout.EndVertical();
            }
            else
            {
                // Expression
                EditorGUILayout.BeginVertical();
                LayoutHelpers.RenderPopup("Expression",
                                          CharacterChangeTag.Character.GetExpressionNamesGUI("No Expression Change"),
                                          ref ExpressionIndex,
                                          CharacterChangeTag.Character.GetExpressionByIndex,
                                          ref ExpressionChangeTag.GetOutfitRef(),
                                          Invalidate);
                EditorGUILayout.EndVertical();


                //Outfit
                EditorGUILayout.BeginVertical();
                LayoutHelpers.RenderPopup("Outfit",
                                          CharacterChangeTag.Character.GetOutfitNamesGUI("No Outfit Change"),
                                          ref OutfitIndex,
                                          CharacterChangeTag.Character.GetOutfitByIndex,
                                          ref OutfitChangeTag.GetOutfitRef(),
                                          Invalidate);
                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Background");
            BackgroundIndex = EditorGUILayout.Popup(BackgroundIndex, VNTagsConfig.GetConfig().GetBackgroundNamesGUI("No Background Change"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            vnTagScriptEditor.RenderEmptyPopup("Sound Effect");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            vnTagScriptEditor.RenderEmptyPopup("Music");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            // EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Preview");
            EditorGUILayout.SelectableLabel(SerializedPreview, new GUIStyle(EditorStyles.textArea) { wordWrap = true, stretchHeight = true });
            // EditorGUI.EndDisabledGroup();
        }
    }
}