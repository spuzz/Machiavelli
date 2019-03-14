using System;
using System.Collections.Generic;
using System.Linq;

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        Random rand = new Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }

    public static IEnumerable<TKey> RandomKeys<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        Random rand = new Random();
        List<TKey> keys = Enumerable.ToList(dict.Keys);
        int size = dict.Count;
        while (true)
        {
            yield return keys[rand.Next(size)];
        }
    }

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.RandomElementUsing<T>(new Random());
    }

    public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
    {
        int index = rand.Next(0, enumerable.Count());
        return enumerable.ElementAt(index);
    }

}