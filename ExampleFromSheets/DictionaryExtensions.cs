namespace ExampleFromSheets;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.TryGetValue(key, out var add))
        {
            return add;
        }
        
        // same as above but simpler
        // if (dictionary.ContainsKey(key))
        // {
        //     return dictionary[key];
        // }
        
        dictionary.Add(key, value);
        // dictionary[key] = value;
        
        return value;
    }
}