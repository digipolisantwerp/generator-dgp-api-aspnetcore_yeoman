using System;
using System.Collections;
using System.Collections.Generic;

namespace StarterKit.Framework.Extensions
{
  public static class CollectionExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="mustExist"></param>
    /// <returns></returns>
    public static TValue GetValue<TValue>(this IDictionary dictionary, string key, bool mustExist = true)
    {
      if (dictionary.Contains(key))
      {
        return (TValue)dictionary[key];
      }
      else
      {
        if (mustExist) throw new KeyNotFoundException($"Key {key} not found in collection.");

        return default(TValue);
      }
    }

    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, bool mustExist = true)
    {
      if (dictionary.ContainsKey(key))
      {
        return (TValue)dictionary[key];
      }
      else
      {
        if (mustExist) throw new Exception($"Dictionary key {key} must exist.");

        return default(TValue);
      }
    }

  }
}
