using System.Collections;

namespace VNTags
{
    public interface IVNData : IEqualityComparer
    {
        public const string DefaultKeyword = "default";
        public       string Name { get; }

        public string[] Alias { get; }

        public string DataType { get; }

        /// <summary>
        ///     basically an empty instance, this is useful for when the null value is used, so differentiating between choosing
        ///     none and null is important
        ///     for example: a transition tag with a null value will use the default transition,
        ///     so in that instance when we want an empty transition tag that does nothing,  we can use the NoneData to signal
        ///     this.
        ///     This works because the EqualityComparer is written so only the contents of the data is checked, instead of the
        ///     default reference check
        /// </summary>
        public IVNData NoneData
        {
            get;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x is IVNData dataX && y is IVNData dataY)
            {
                return (dataX.GetType() == dataY.GetType()) && dataX.Name.Equals(dataY.Name) && dataX.DataType.Equals(dataY.DataType);
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

    public static class VNDataExtensions
    {
        public static bool IsNone(this IVNData data)
        {
            return (data != null) && (data == data.NoneData);
        }
    }
}