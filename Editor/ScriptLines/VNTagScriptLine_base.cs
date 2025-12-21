using UnityEngine;
using VNTags.Tags;

namespace VNTags.Editor
{
    public class VNTagScriptLine_base
    {
        public bool Foldout = false;

        public VNTagScriptLine_base(string rawLine, ushort lineNumber)
        {
            LineNumber = lineNumber;
            RawLine    = rawLine;
            Tags       = new VNTagQueue(VNTagDeserializer.ParseLine(rawLine, lineNumber));
        }

        public VNTagScriptLine_base(VNTagScriptLine_base other)
        {
            LineNumber = other.LineNumber;
            RawLine    = other.RawLine;
            Tags       = other.Tags;
        }

        public string     RawLine           { get; protected set; }
        public string     Preview { get; protected set; }
        public string     SerializedPreview { get; protected set; }
        public ushort     LineNumber        { get; }
        public VNTagQueue Tags              { get; }
        
        public VNTagSerializationContext SerializationContext
        {
            get { return new VNTagSerializationContext(Tags); }
        }

        public CharacterTag GetMainCharacter()
        {
            foreach (var tag in Tags)
            {
                if (tag is CharacterTag cTag)
                {
                    return cTag;
                }
                else if(tag is DialogueTag)
                {
                    break;
                }
            }
            return null;
        }
        
        public ExpressionTag GetMainCharacterExpression()
        {
            foreach (var tag in Tags)
            {
                if (tag is ExpressionTag eTag)
                {
                    return eTag;
                }
                else if(tag is DialogueTag)
                {
                    break;
                }
            }
            return null;
        }
        
        public OutfitTag GetMainCharacterOutfit()
        {
            foreach (var tag in Tags)
            {
                if (tag is OutfitTag oTag)
                {
                    return oTag;
                }
                else if(tag is DialogueTag)
                {
                    break;
                }
            }
            return null;
        }

        public bool HasDialogueTags()
        {
            foreach (VNTag tag in Tags)
            {
                if (tag is DialogueTag)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSceneSetupLine()
        {
            return !HasDialogueTags();
        }

        public virtual void Invalidate() { }

        public virtual string Serialize()
        {
            return VNTagSerializer.SerializeLine(Tags);
        }

        public virtual void RenderLine(VNTagScript_Editor editor)
        {
            
        }
        
        public VNTagDeserializationContext CreateDeserializationContext(ushort tagNumber)
        {
            return new VNTagDeserializationContext(LineNumber, RawLine, tagNumber);
        }

        public VNTagDeserializationContext CreateDeserializationContext(IEditorTag editorTag)
        {
            return new VNTagDeserializationContext(LineNumber, RawLine, editorTag.GetID().TagNumber);
            //
            // if ((Tags == null) || (Tags.Count <= 0))
            // {
            //     Debug.LogError("VNTagEditLine: CreateDeserializationContext: tags is null or empty");
            //     return new VNTagDeserializationContext();
            // }
            //
            // ushort index = 0;
            // bool   found = false;
            // foreach (VNTag tag in Tags)
            // {
            //     if (tag is IEditorTag eTag && (eTag == editorTag))
            //     {
            //         found = true;
            //         break;
            //     }
            //
            //     index++;
            // }
            //
            // if (!found)
            // {
            //     Debug.LogError("VNTagEditLine: CreateDeserializationContext: tag not found, returning empty context");
            //     return new VNTagDeserializationContext();
            // }
            //
            // return new VNTagDeserializationContext(LineNumber, RawLine, index);
        }

        public VNTagDeserializationContext CreateDeserializationContext(VNTag tag)
        {
            return new VNTagDeserializationContext(LineNumber, RawLine, tag.ID.TagNumber);
            // if ((Tags == null) || (Tags.Count <= 0))
            // {
            //     Debug.LogError("VNTagEditLine: CreateDeserializationContext: tags is null or empty");
            //     return new VNTagDeserializationContext();
            // }
            //
            // int index = Tags.IndexOf(tag);
            // if (index == -1)
            // {
            //     Debug.LogError("VNTagEditLine: CreateDeserializationContext: tag not found, returning empty context");
            //     return new VNTagDeserializationContext();
            // }
            //
            // return new VNTagDeserializationContext(LineNumber, RawLine, (ushort)index);
        }
    }
}