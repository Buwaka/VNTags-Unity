namespace VNTags.Utility
{
    public class ImmutablePair<TKey, TValue>
    {
        public ImmutablePair(TKey key, TValue value)
        {
            Key   = key;
            Value = value;
        }

        public TKey   Key   { get; }
        public TValue Value { get; }

        public static implicit operator ImmutablePair<TKey, TValue>(Pair<TKey, TValue> pair)
        {
            if (pair == null)
            {
                return null;
            }

            return new ImmutablePair<TKey, TValue>(pair.Key, pair.Value);
        }
    }
}