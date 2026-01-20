using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility;

namespace VNTags.Editor
{
    public class VNTagSceneSetupLine : VNTagScriptLine_base
    {
        private const string NullTransition = "No Transition";
        
        public int           BackgroundIndex;
        public BackgroundTag BackgroundTag;
        public int           BGMIndex;

        public int               CharacterMask;
        public MultiCharacterTag CharacterTag;

        public bool SceneResetFlag = true;
        public bool ToggleVNUIFlag = true;


        public string        TransitionName;
        public TransitionTag TransitionTag;

        public VNTagSceneSetupLine(string rawLine, ushort lineNumber) : base(rawLine, lineNumber)
        {
            Init();
        }

        public VNTagSceneSetupLine(VNTagScriptLine_base other) : base(other)
        {
            Init();
        }

        private void Init()
        {
            Preview = "Scene Setup #" + LineNumber;
            foreach (VNTag tag in Tags)
            {
                switch (tag)
                {
                    case MultiCharacterTag characterTag when CharacterTag == null:
                        CharacterTag = characterTag;
                        break;
                    case BackgroundTag backgroundTag when BackgroundTag == null:
                        BackgroundTag = backgroundTag;
                        break;
                    case TransitionTag transitionTag when TransitionTag == null:
                        TransitionTag = transitionTag;
                        break;
                    case SceneResetTag:
                        SceneResetFlag = true;
                        break;
                    case ToggleVNUI:
                        ToggleVNUIFlag = true;
                        break;
                }
            }

            if (TransitionTag == null)
            {
                TransitionTag = ScriptableObject.CreateInstance<TransitionTag>();
                TransitionTag.SetNone();
            }
            else
            {
                var midTransitionTags = VNTagDeserializer.ParseLine(TransitionTag.MidTransitionTags, 0);
                foreach (VNTag tag in midTransitionTags)
                {
                    switch (tag)
                    {
                        case MultiCharacterTag characterTag:
                            CharacterTag = characterTag;
                            break;
                        case BackgroundTag backgroundTag:
                            BackgroundTag = backgroundTag;
                            break;
                    }
                }
                Tags.Remove(TransitionTag);
            }

            if (BackgroundTag == null)
            {
                BackgroundTag = ScriptableObject.CreateInstance<BackgroundTag>();
                BackgroundTag.SetNone();
            }
            else
            {
                Tags.Remove(BackgroundTag);
            }

            if (CharacterTag == null)
            {
                CharacterTag = ScriptableObject.CreateInstance<MultiCharacterTag>();
            }
            else
            {
                Tags.Remove(CharacterTag);
            }

            if (ToggleVNUIFlag)
            {
                Tags.RemoveofType(typeof(ToggleVNUI));
            }

            if (SceneResetFlag)
            {
                Tags.RemoveofType(typeof(SceneResetTag));
            }


            InitUIIndieces();
            Invalidate();
        }

        public override void Invalidate()
        {
            SerializedPreview = Serialize();
        }

        public void InitUIIndieces()
        {
            // public int CharacterMask;
            // public int BackgroundIndex;
            // public int TransitionIndex;
            // public int BGMIndex;

            // Character Mask
            CharacterMask = LayoutHelpers.ValueArrayToMask(CharacterTag.Characters, VNTagsConfig.GetConfig().AllCharacters);

            // BackgroundIndex
            BackgroundIndex = 0;
            if (BackgroundTag != null && BackgroundTag.Background != null)
            {
                var backgroundNames = VNTagsConfig.GetConfig().GetBackgroundNamesGUI("");
                for (int i = 0; i < backgroundNames.Length; i++)
                {
                    if (backgroundNames[i].text.Equals(BackgroundTag.Background.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        BackgroundIndex = i;
                        break;
                    }
                }
            }

            // test out the setup and transitiontag ingame


            // TransitionIndex
            if (TransitionTag != null)
            {
                if (TransitionTag.Transition.IsNone() || TransitionTag.Transition == null)
                {
                    TransitionName = IVNData.DefaultKeyword;
                }
                else if (TransitionTag.Transition != null)
                {
                    TransitionName = TransitionTag.Transition.Name;
                }
                else
                {
                    TransitionName = "null";
                }
            }
        }

        public override string Serialize()
        {
            var tempTags = new VNTagQueue();

            if (!TransitionName.Equals(NullTransition, StringComparison.OrdinalIgnoreCase))
            {
                //insert background tag into transition
                var transitionTags = new List<VNTag>();
                if (SceneResetFlag)
                {
                    transitionTags.Add(ScriptableObject.CreateInstance<SceneResetTag>());
                }
                tempTags.RemoveofType(typeof(SceneResetTag));

                transitionTags.Add(BackgroundTag);
                transitionTags.Add(CharacterTag);

                TransitionTag.MidTransitionTags = VNTagSerializer.SerializeLine(transitionTags);
                tempTags.AddFirst(TransitionTag);
            }
            else
            {
                tempTags.AddLast(TransitionTag);
                tempTags.AddLast(BackgroundTag);
                tempTags.AddLast(CharacterTag);
                if (SceneResetFlag)
                {
                    tempTags.AddUnique(ScriptableObject.CreateInstance<SceneResetTag>());
                }
                else
                {
                    tempTags.RemoveofType(typeof(SceneResetTag));
                }
            }



            if (ToggleVNUIFlag)
            {
                tempTags.AddUnique(ScriptableObject.CreateInstance<ToggleVNUI>(), false);
            }
            else
            {
                tempTags.RemoveofType(typeof(ToggleVNUI));
            }


            return VNTagSerializer.SerializeLine(tempTags) + base.Serialize();
        }

        public override void RenderLine(VNTagScript_Editor editor)
        {
            EditorGUILayout.LabelField("Scene Setup");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderPopup("Background",
                                      VNTagsConfig.GetConfig().GetBackgroundNamesGUI("No Background"),
                                      ref BackgroundIndex,
                                      VNTagsConfig.GetConfig().GetBackgroundByIndex,
                                      ref BackgroundTag.GetBackgroundRef(),
                                      Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderMaskMultiSelectPopup("Character(s)",
                                                     VNTagsConfig.GetConfig().GetCharacterNamesGUI(),
                                                     ref CharacterMask, mask => LayoutHelpers.MaskToValueArray(mask, VNTagsConfig.GetConfig().AllCharacters),
                                                     ref CharacterTag.GetCharacterRef(),
                                                     Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            LayoutHelpers.RenderPopup("Transition",
                                      VNTagsConfig.GetConfig().GetTransitionNamesGUI(NullTransition),
                                      ref TransitionName,
                                      VNTagsConfig.GetConfig().GetTransitionByNameOrAlias,
                                      ref TransitionTag.GetTransitionRef(),
                                      Invalidate);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            // music
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            SceneResetFlag = EditorGUILayout.Toggle("Reset Scene", SceneResetFlag);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            ToggleVNUIFlag = EditorGUILayout.Toggle("Toggle Textbox", ToggleVNUIFlag);
            ;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.LabelField("Preview");
            EditorGUILayout.SelectableLabel(SerializedPreview, new GUIStyle(EditorStyles.textArea) { wordWrap = true, stretchHeight = true });

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }
    }
}