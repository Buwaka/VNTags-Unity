using System;

namespace VNTags.Utility
{
    public class TypeCodeUtil
    {
        public static object GetDefaultInstance(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return default(bool);
                case TypeCode.Byte:
                    return default(byte);
                case TypeCode.SByte:
                    return default(sbyte);
                case TypeCode.Char:
                    return default(char);
                case TypeCode.Decimal:
                    return default(decimal);
                case TypeCode.Double:
                    return default(double);
                case TypeCode.Single:
                    return default(float);
                case TypeCode.Int16:
                    return default(short);
                case TypeCode.Int32:
                    return default(int);
                case TypeCode.Int64:
                    return default(long);
                case TypeCode.UInt16:
                    return default(ushort);
                case TypeCode.UInt32:
                    return default(uint);
                case TypeCode.UInt64:
                    return default(ulong);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.String:
                    return string.Empty;
                case TypeCode.Object:
                    return null;
                default:
                    throw new NotSupportedException($"TypeCode '{typeCode}' is not supported for default instance creation.");
            }
        }
    }
}