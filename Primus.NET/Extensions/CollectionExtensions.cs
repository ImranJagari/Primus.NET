using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Primus.NET.Extensions
{
    public static class CollectionExtensions
    {
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
