using System;
using System.Collections.Generic;
using UnityEngine;
using VNTags.Tags;

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


        public VNTagSerializationContext SerializationContext
        {
            get { return new VNTagSerializationContext(Tags); }
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
            do
            {
                VNTag tag = iterator.Current;
                if (tag is CharacterTag characterTag && (_characterChangeTag == null))
                {
                    _characterChangeTag = characterTag;
                    if (_characterChangeTag.Character != null)
                    {
                        Preview += _characterChangeTag.Character.Name + ": ";
                    }
                }
                else if (tag is ExpressionTag expressionTag && (_expressionChangeTag == null))
                {
                    _expressionChangeTag = expressionTag;
                }
                else if (tag is OutfitTag outfitTag && (_outfitChangeTag == null))
                {
                    _outfitChangeTag = outfitTag;
                }
                else if (tag is BackgroundTag backgroundTag && (_backgroundChangeTag == null))
                {
                    _backgroundChangeTag = backgroundTag;
                }
                else
                {
                    break;
                }
            } while (iterator.MoveNext());

            // extra tags
            while (iterator.MoveNext())
            {
                VNTag tag = iterator.Current;
                if (tag is DialogueTag dTag)
                {
                    Preview += dTag.Dialogue;
                    ExtraTags.Add(dTag);
                }
                else
                {
                    var dummy = ScriptableObject.CreateInstance<DummyTag>();
                    dummy._init(VNTagID.GenerateID(LineNumber, (ushort)Tags.Count), RawLine);
                    dummy.Deserialize(new VNTagDeserializationContext(), tag.StringRepresentation);
                    ExtraTags.Add(dummy);
                }
            }

            iterator.Dispose();

            InitUIIndieces();
            Invalidate();
        }

        public VNTagDeserializationContext CreateDeserializationContext(ushort tagNumber)
        {
            return new VNTagDeserializationContext(LineNumber, RawLine, tagNumber);
        }

        public VNTagDeserializationContext CreateDeserializationContext(IEditorTag editorTag)
        {
            if ((Tags == null) || (Tags.Count <= 0))
            {
                Debug.LogError("VNTagEditLine: CreateDeserializationContext: tags is null or empty");
                return new VNTagDeserializationContext();
            }

            ushort index = 0;
            bool   found = false;
            foreach (VNTag tag in Tags)
            {
                if (tag is IEditorTag eTag && (eTag == editorTag))
                {
                    found = true;
                    break;
                }

                index++;
            }

            if (!found)
            {
                Debug.LogError("VNTagEditLine: CreateDeserializationContext: tag not found, returning empty context");
                return new VNTagDeserializationContext();
            }

            return new VNTagDeserializationContext(LineNumber, RawLine, index);
        }

        public VNTagDeserializationContext CreateDeserializationContext(VNTag tag)
        {
            if ((Tags == null) || (Tags.Count <= 0))
            {
                Debug.LogError("VNTagEditLine: CreateDeserializationContext: tags is null or empty");
                return new VNTagDeserializationContext();
            }

            int index = Tags.IndexOf(tag);
            if (index == -1)
            {
                Debug.LogError("VNTagEditLine: CreateDeserializationContext: tag not found, returning empty context");
                return new VNTagDeserializationContext();
            }

            return new VNTagDeserializationContext(LineNumber, RawLine, (ushort)index);
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
            if ((_characterChangeTag != null) && (_characterChangeTag.Character != null))
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
                if ((_expressionChangeTag != null) && (_expressionChangeTag.Expression != null))
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
                if ((_outfitChangeTag != null) && (_outfitChangeTag.Outfit != null))
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
                    if (backgrounds[i].text.Equals(_backgroundChangeTag.Background.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        BackgroundIndex = i;
                        break;
                    }
                }
            }
        }
    }
}