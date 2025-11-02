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
    }
}