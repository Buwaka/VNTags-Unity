using System;
using System.Collections.Generic;
using UnityEngine;
using VNTags.Tags;

namespace VNTags.Editor
{
    public class VNTagScriptLine
    {
        private readonly ushort        _lineNumber;
        private readonly VNTagQueue    Tags;
        private          BackgroundTag _backgroundChangeTag;


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


        public VNTagScriptLine(string rawLine, ushort lineNumber)
        {
            Preview     = lineNumber + ": ";
            _lineNumber = lineNumber;
            RawLine     = rawLine;
            Tags        = new VNTagQueue(VNTagDeserializer.ParseLine(RawLine, lineNumber));

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

        public VNTagDeserializationContext CreateDeserializationContext(ushort tagNumber)
        {
            return new VNTagDeserializationContext(_lineNumber, RawLine, tagNumber);
        }

        public VNTagDeserializationContext CreateDeserializationContext(VNTag tag)
        {
            if (Tags == null || Tags.Count <= 0)
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
            
            return new VNTagDeserializationContext(_lineNumber, RawLine, (ushort)index);
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
                //todo
                var backgrounds = VNTagsConfig.GetConfig().AllBackgrounds;
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
}