namespace lsif_debug
{
    internal static class ReadOnlyDictionaryExtensions
    {
        public static IEnumerable<TValue> GetOrEmpty<TKey, TValue>(this IReadOnlyDictionary<TKey, HashSet<TValue>> dict, TKey key)
        {
            if (!dict.TryGetValue(key, out var values))
            {
                return Array.Empty<TValue>();
            }

            return values;
        }
    }
}
