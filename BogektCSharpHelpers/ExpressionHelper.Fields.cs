using System.Reflection;

namespace BogektCSharpHelpers {
    public static partial class ExpressionHelper {
        private static readonly MethodInfo BuildCompareLambdaMI =
            typeof (ExpressionHelper).GetMethod("BuildCompareLambda", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo BuildMethodCallLambdaMI =
            typeof (ExpressionHelper).GetMethod("BuildMethodCallLambda", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly string ComparesTypeName = typeof (Compares).FullName;
        private static readonly string MethodCallTypeName = typeof (MethodCall).FullName;
    }
}