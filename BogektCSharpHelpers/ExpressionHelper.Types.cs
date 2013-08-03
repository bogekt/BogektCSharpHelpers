using System;

namespace BogektCSharpHelpers {
    public static partial class ExpressionHelper {
        public class FilterParams<TEnum>
            where TEnum : struct {
            public string PropertyName { get; set; }
            public Type ValueType { get; set; }
            public object Value { get; set; }
            public TEnum FilterType { get; set; }
        }

        public enum Compares {
              EQ
            , GT
            , GTE
            , LT
            , LTE
        }

        public enum MethodCall {
            Contains
        }
    }
}