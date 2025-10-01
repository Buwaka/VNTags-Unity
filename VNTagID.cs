using System.Runtime.InteropServices;

namespace VNTags
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VNTagID
    {
        // The entire 32-bit ID
        [FieldOffset(0)] public uint ID;

        // The first 16 bits (line number)
        [FieldOffset(0)] public ushort LineNumber;

        // The following 16 bits (tag number)
        [FieldOffset(2)] public ushort TagNumber;

        public VNTagID(uint id)
        {
            LineNumber = 0;
            TagNumber  = 0;
            ID         = id;
        }

        public VNTagID(ushort lineNumber, ushort tagNumber)
        {
            ID         = 0;
            LineNumber = lineNumber;
            TagNumber  = tagNumber;
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public static implicit operator uint(VNTagID tagID)
        {
            return tagID.ID;
        }

        public static implicit operator int(VNTagID tagID)
        {
            return (int)tagID.ID;
        }

        public static implicit operator VNTagID(uint id)
        {
            return new VNTagID(id);
        }

        public static implicit operator VNTagID(int id)
        {
            return new VNTagID((uint)id);
        }

        /// <summary>
        ///     the ID is a 32 bit number,
        ///     the first 16 bits are the line number,
        ///     the following 16 bits are the tag number
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="tagNumber"></param>
        /// <returns></returns>
        public static VNTagID GenerateID(ushort lineNumber, ushort tagNumber)
        {
            return new VNTagID(lineNumber, tagNumber);
        }
    }
}