using System.Runtime.InteropServices;

namespace VN
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VNTagID
    {
        // The entire 32-bit ID
        [FieldOffset(0)]
        public uint ID;

        // The first 16 bits (line number)
        [FieldOffset(0)]
        public ushort LineNumber;

        // The following 16 bits (tag number)
        [FieldOffset(2)]
        public ushort TagNumber;

        public VNTagID(uint id)
        {
            LineNumber = 0;
            TagNumber  = 0;
            ID         = id;
        }

        public VNTagID(ushort lineNumber, ushort tagNumber)
        {
            ID              = 0;
            this.LineNumber = lineNumber;
            this.TagNumber  = tagNumber;
        }

        public static implicit operator uint(VNTagID tagID) => tagID.ID;

        public static implicit operator VNTagID(uint id) => new VNTagID(id);
    }
}