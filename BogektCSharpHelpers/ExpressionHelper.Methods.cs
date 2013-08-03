using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BogektCSharpHelpers {
    public static partial class ExpressionHelper {
        private static Tuple<MemberExpression, ConstantExpression, ParameterExpression> CreateExpressionsTuple
            <TModel, TPropType>(
              string propertyName
            , TPropType propertyValue) {
            var parameterExpression = Expression.Parameter(typeof (TModel), "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyName);
            var nullablePropertyBaseType = Nullable.GetUnderlyingType(propertyExpression.Type);
            var nullableValueBaseType = Nullable.GetUnderlyingType(typeof (TPropType));
            var constantExpression = Expression.Constant(
                propertyValue
                , nullablePropertyBaseType == null
                    ? nullableValueBaseType ?? typeof (TPropType)
                    : propertyExpression.Type);
            return new Tuple<MemberExpression, ConstantExpression, ParameterExpression>(
                propertyExpression
                , constantExpression
                , parameterExpression);
        }

        private static Expression<Func<TModel, bool>> BuildCompareLambda<TModel, TPropType>(
              this string propertyName
            , TPropType comapreValue
            , Compares compare = Compares.EQ) {
            var exprTuple = CreateExpressionsTuple<TModel, TPropType>(propertyName, comapreValue);
            BinaryExpression binaryExpression;
            switch (compare) {
                default:
                case Compares.EQ:
                    binaryExpression = Expression.Equal(exprTuple.Item1, exprTuple.Item2);
                    break;
                case Compares.GT:
                    binaryExpression = Expression.GreaterThan(exprTuple.Item1, exprTuple.Item2);
                    break;
                case Compares.GTE:
                    binaryExpression = Expression.GreaterThanOrEqual(exprTuple.Item1, exprTuple.Item2);
                    break;
                case Compares.LT:
                    binaryExpression = Expression.LessThan(exprTuple.Item1, exprTuple.Item2);
                    break;
                case Compares.LTE:
                    binaryExpression = Expression.LessThanOrEqual(exprTuple.Item1, exprTuple.Item2);
                    break;
            }
            return Expression.Lambda<Func<TModel, bool>>(binaryExpression, exprTuple.Item3);
        }

        private static Expression<Func<TModel, bool>> BuildMethodCallLambda<TModel, TPropType>(
              this string propertyName
            , TPropType callValue
            , MethodCall methodCall = MethodCall.Contains) {
            var exprTuple = CreateExpressionsTuple<TModel, TPropType>(propertyName, callValue);
            MethodCallExpression methodCallExpression;
            switch (methodCall) {
                default:
                case MethodCall.Contains:
                    methodCallExpression = Expression.Call(exprTuple.Item1, methodCall.ToString(), null, exprTuple.Item2);
                    break;
            }
            return Expression.Lambda<Func<TModel, bool>>(methodCallExpression, exprTuple.Item3);
        }

        public static IQueryable<TModel> GetWithAppliedFilterFor<TModel, TEnum>(
              this IQueryable<TModel> queryable
            , FilterParams<TEnum> filterParams)
            where TEnum : struct {
            if (filterParams.Value == null) return queryable;
            var enumType = typeof (TEnum).FullName;
            Expression<Func<TModel, bool>> expression = null;
            if (enumType.Equals(ComparesTypeName)) {
                expression = BuildCompareLambdaMI.CallMethodWithInjectGenericTypes<Expression<Func<TModel, bool>>>(
                    new[] {typeof (TModel), filterParams.ValueType}
                    , @params: new[] {filterParams.PropertyName, filterParams.Value, filterParams.FilterType});
            } else if (enumType.Equals(MethodCallTypeName)) {
                expression = BuildMethodCallLambdaMI.CallMethodWithInjectGenericTypes<Expression<Func<TModel, bool>>>(
                    new[] {typeof (TModel), filterParams.ValueType}
                    , @params: new[] {filterParams.PropertyName, filterParams.Value, filterParams.FilterType});
            }
            return queryable.Where(expression);
        }

        public static IQueryable<TModel> GetWithAppliedFiltersFor<TModel, TEnum>(
              this IQueryable<TModel> queryable
            , IEnumerable<FilterParams<TEnum>> filtersParams)
            where TEnum : struct {
            foreach (var filterParams in filtersParams) {
                queryable = queryable.GetWithAppliedFilterFor(filterParams);
            }
            return queryable;
        }

        public static FilterParams<TEnum> BuildFilterParams<TValueType, TEnum>(
              this string propertyName
            , TValueType value
            , TEnum filterType = default(TEnum)
            , Type valueType = null)
            where TEnum : struct {
            return new FilterParams<TEnum> {
                  PropertyName = propertyName
                , FilterType = filterType
                , Value = value
                , ValueType = valueType ?? typeof (TValueType)
            };
        }
    }
}