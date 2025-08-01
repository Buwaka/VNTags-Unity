namespace VNTags.Utility
{
    public class Pair<TKey, TValue>
    {
        public TKey   Key;
        public TValue Value;

        public Pair(TKey key, TValue value)
        {
            Key   = key;
            Value = value;
        }
    }
}