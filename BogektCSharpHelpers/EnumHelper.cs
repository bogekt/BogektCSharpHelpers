using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace BogektCSharpHelpers {
    public static class EnumHelper {
        private static readonly Regex WhitespaceInsertRegex = new Regex(@"(?!^)([A-Z])");

        public static string S(this Enum @enum) {
            return @enum.ToString();
        }

        public static string SW(this Enum @enum) {
            return WhitespaceInsertRegex.Replace(@enum.ToString(), " $1");
        }

        public static IEnumerable<SelectListItem> ToMvcSelectList(
              this Enum @enum
            , SelectListItem defaultValue = null
            , bool needTextStringWhitespaced = true
            , bool uppercaseText = false) {
            var values = Enum.GetValues(@enum.GetType()).Cast<int>().ToArray();
            var names = Enum.GetNames(@enum.GetType());
            var list = new List<SelectListItem>(values.Length);
            if (defaultValue != null) list.Add(defaultValue);
            var selectedValue = @enum.ToString();
            return ToMvcSelectList(values, names, selectedValue, defaultValue, needTextStringWhitespaced, uppercaseText);
        }

        public static IEnumerable<SelectListItem> ToMvcSelectList<TEnum>(
              this IEnumerable<TEnum> @enums
            , TEnum? selectedEnum = null
            , SelectListItem defaultValue = null
            , bool needTextStringWhitespaced = true
            , bool uppercaseText = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible {
            var enumerable = @enums.ToList();
            var values = enumerable.Cast<int>().ToArray();
            var names = enumerable.Select(@enum => @enum.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray();
            var selectedValue = selectedEnum != null ? selectedEnum.ToString() : string.Empty;
            return ToMvcSelectList(values, names, selectedValue, defaultValue, needTextStringWhitespaced, uppercaseText);
        }

        private static IEnumerable<SelectListItem> ToMvcSelectList(
              int[] values
            , string[] names
            , string selectedValue
            , SelectListItem defaultValue = null
            , bool needTextStringWhitespaced = true
            , bool uppercaseText = false) {
            var list = new List<SelectListItem>(values.Length);
            var isNotNullDefault = defaultValue != null;
            if (isNotNullDefault) list.Add(defaultValue);
            var needCheckSelected = !isNotNullDefault && !string.IsNullOrWhiteSpace(selectedValue);
            list.AddRange(
                values.Cast<object>()
                    .Select((t, i) => new SelectListItem {
                          Selected = needCheckSelected && selectedValue == names[i]
                        , Text = names[i].TextFormat(needTextStringWhitespaced, uppercaseText)
                        , Value = values.GetValue(i).ToString()
                    }));
            return list;
        }

        private static string TextFormat(
              this string text
            , bool needTextStringWhitespaced = true
            , bool uppercaseText = false) {
            var tempText = needTextStringWhitespaced ? WhitespaceInsertRegex.Replace(text, " $1") : text;
            tempText = uppercaseText ? tempText.ToUpperInvariant() : tempText;
            return tempText;
        }
    }
}