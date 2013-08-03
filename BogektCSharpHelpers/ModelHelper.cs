using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BogektCSharpHelpers {
    public static class ModelHelper {
        public static object GetPropertyValue<TModel>(this TModel model, string propertyName)
            where TModel : class {
            if (model == null) return null;

            object obj = model;
            var propertyNameParts = propertyName.Split('.');

            foreach (var propertyNamePart in propertyNameParts) {
                if (obj == null) return null;

                if (!propertyNamePart.Contains("[")) {
                    var pi = obj.GetType().GetProperty(propertyNamePart);
                    if (pi == null) throw new ArgumentException("Invalid property");
                    obj = pi.GetValue(obj, null);
                } else {
                    var indexStart = propertyNamePart.IndexOf("[", StringComparison.InvariantCulture) + 1;
                    var collectionPropertyName = propertyNamePart.Substring(0, indexStart - 1);
                    var collectionElementIndex =
                        Int32.Parse(propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1));
                    var pi = obj.GetType().GetProperty(collectionPropertyName);
                    if (pi == null) return null;
                    var unknownCollection = pi.GetValue(obj, null);
                    if (unknownCollection.GetType().IsArray) {
                        object[] collectionAsArray = unknownCollection as Array[];
                        if (collectionAsArray != null) obj = collectionAsArray[collectionElementIndex];
                    } else {
                        var collectionAsList = unknownCollection as System.Collections.IList;
                        if (collectionAsList != null) {
                            obj = collectionAsList[collectionElementIndex];
                        } else {
                            throw new ArgumentException("Unsupported collection type");
                        }
                    }
                }
            }

            return obj;
        }

        public static TValueType GetPropertyValue<TModel, TValueType>(
              this TModel model
            , Expression<Func<TModel, TValueType>> expression)
            where TModel : class {
            return (TValueType) GetPropertyValue(model, GetPropertyName(expression));
        }

        public static TValueType TryGetPropertyValue<TModel, TValueType>(
              this TModel model
            , Expression<Func<TModel, TValueType>> expression
            , TValueType defaultValue = default(TValueType))
            where TModel : class {
            if (model == null) return defaultValue;
            var value = GetPropertyValue(model, GetPropertyName(expression));
            if (value == null) return defaultValue;
            return value.Equals(default(TValueType)) ? defaultValue : (TValueType) value;
        }

        public static string GetPropertyName<TModel, TValueType>(this Expression<Func<TModel, TValueType>> expression) {
            return System.Web.Mvc.ExpressionHelper.GetExpressionText(expression);
        }

        public static string GetPropertyName<TModel>(this Expression<Func<TModel, object>> expression) {
            var me = expression.Body as MemberExpression;
            if (me != null) return me.Member.Name;
            var ue = expression.Body as UnaryExpression;
            if (ue == null) return string.Empty;
            var name = ue.Operand.ToString();
            return name.Substring(name.IndexOf('.') + 1);
        }

        public static void SetPropertyValueByName<TModel, TPropertyType>(this TModel propertyObject,
            string propertyName, TPropertyType value) {
            var modelType = typeof (TModel);
            modelType.GetProperty(propertyName).SetValue(propertyObject, value, null);
        }

        public static TType ToTypeCast<TType>(object obj) {
            return (TType) obj;
        }

        public static MethodInfo GetMethodInfo<TModel>(this Expression<Action<TModel>> expression) {
            var member = expression.Body as MethodCallExpression;
            if (member != null)
                return member.Method;
            throw new ArgumentException("Expression is not a method", "expression");
        }

        public static TReturnType CallMethodWithInjectGenericTypes<TReturnType>(
              this MethodInfo callMI
            , Type[] injectTypes
            , object callInstance = null
            , params object[] @params) {
            var genericCallMI = callMI.MakeGenericMethod(injectTypes);
            return (TReturnType)genericCallMI.Invoke(callInstance, @params);
        }

        public static TModel[] ToSingleArray<TModel>(this TModel model) {
            return new[] { model };
        }
    }
}