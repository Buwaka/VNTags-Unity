using System;
using UnityEngine;
using VNTags.Tags;
using VNTags.Utility;

namespace VNTags.Editor
{
    public class VNTagSceneSetupLine : VNTagScriptLine_base
    {
        public int           BackgroundIndex;
        public BackgroundTag BackgroundTag;
        public int           BGMIndex;

        public int               CharacterMask;
        public MultiCharacterTag CharacterTag;
        

        public string           TransitionName;
        public TransitionTag TransitionTag;

        public bool          SceneResetFlag = true;
        public SceneResetTag SceneResetTag  = null;

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
                    case SceneResetTag sceneResetTag when SceneResetTag == null:
                        SceneResetTag = sceneResetTag;
                        break;
                }
            }

            if (CharacterTag == null)
            {
                CharacterTag = ScriptableObject.CreateInstance<MultiCharacterTag>();
                Tags.AddLast(CharacterTag);
            }

            if (BackgroundTag == null)
            {
                BackgroundTag = ScriptableObject.CreateInstance<BackgroundTag>();
                Tags.AddLast(BackgroundTag);
            }

            if (TransitionTag == null)
            {
                TransitionTag                    = ScriptableObject.CreateInstance<TransitionTag>();
                TransitionTag.SetNone();
                Tags.AddLast(TransitionTag);
            }

            if (SceneResetTag == null)
            {
                SceneResetTag = ScriptableObject.CreateInstance<SceneResetTag>();
            }


            // extra tags
            // while(iterator.MoveNext())
            // {
            //     var tag = iterator.Current;
            //     if (tag is DialogueTag dTag)
            //     {
            //         Preview += dTag.Dialogue;
            //         ExtraTags.Add(dTag);
            //     }
            //     else
            //     {
            //         var dummy = ScriptableObject.CreateInstance<DummyTag>();
            //         dummy._init(VNTagID.GenerateID(lineNumber, (ushort)Tags.Count), RawLine);
            //         dummy.Deserialize(new VNTagDeserializationContext(), tag.StringRepresentation);
            //         ExtraTags.Add(dummy);
            //     }
            // }


            InitUIIndieces();
            Invalidate();
        }

        public override void Invalidate()
        {
            SerializedPreview = VNTagSerializer.SerializeLine(Tags);
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

            
            // TransitionIndex
            if (TransitionTag != null )
            {
                if (TransitionTag.Transition != null)
                {
                    TransitionName = TransitionTag.Transition.Name;
                }
                else
                {
                    TransitionName = IVNData.DefaultKeyword;
                }
            }


            // SceneResetFlag
            SceneResetFlag = SceneResetTag != null;
        }

        public override string Serialize()
        {
            if (SceneResetFlag)
            {
                Tags.AddUnique(SceneResetTag);
            }
            else
            {
                Tags.RemoveofType(SceneResetTag);
            }
            
            return base.Serialize();
        }
    }
}