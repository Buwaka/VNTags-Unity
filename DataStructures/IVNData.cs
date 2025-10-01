using System.Collections;

namespace VNTags
{
    public interface IVNData : IEqualityComparer
    {
        public string Name { get; }

        public string[] Alias { get; }

        public string DataType { get; }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x is IVNData dataX && y is IVNData dataY)
            {
                return dataX.Name.Equals(dataY.Name) && dataX.DataType.Equals(dataY.DataType);
            }

            if (x is IVNData X && y is string sY)
            {
                return sY.Equals(X.Name);
            }

            if (y is IVNData Y && x is string sX)
            {
                return sX.Equals(Y.Name);
            }

            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return GetHashCode(obj);
        }
    }
}