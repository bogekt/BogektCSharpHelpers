using System;
using System.Collections.Generic;
using System.Globalization;

namespace BogektCSharpHelpers {
    public static class DictionaryHelper {
        public static bool TryAdd<TEnum>(
              this IDictionary<string, object> dictionary
            , TEnum key
            , object value
            , bool withReplaceOnExist = true)
            where TEnum
                : struct
                , IComparable
                , IFormattable
                , IConvertible {
            return TryAdd<string, object>(
                dictionary
                , key.ToString(CultureInfo.InvariantCulture)
                , value
                , withReplaceOnExist);
        }

        public static TValue TryGet<TValue>(
              this IDictionary<string, object> dictionary
            , Enum key
            , TValue defaultValue = default(TValue)) {
            return (TValue) TryGet(dictionary, key.ToString(), defaultValue);
        }

        public static bool TryAdd<TKey, TValue>(
              this IDictionary<TKey, TValue> dictionary
            , TKey key
            , TValue value
            , bool withReplaceOnExist = true) {
            if (dictionary.ContainsKey(key)) {
                if (!withReplaceOnExist) return false;
                dictionary[key] = value;
                return true;
            }
            dictionary.Add(key, value);
            return true;
        }

        public static TValue TryGet<TKey, TValue>(
              this IDictionary<TKey, TValue> dictionary
            , TKey key
            , TValue defaultValue = default(TValue)) {
            return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }
    }
}