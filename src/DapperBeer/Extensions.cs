namespace DapperBeer;

public static class Extensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.TryGetValue(key, out var add))
            return add;
        
        dictionary.Add(key, value);
        return value;
    }
}