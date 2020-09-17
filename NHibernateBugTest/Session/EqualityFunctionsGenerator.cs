using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernateBugTest
{

    public static class ObjectEqualityFunction
    {
        public static readonly Type TypeOfBool = typeof(bool);
        public static readonly MethodInfo MethodEquals = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        public static readonly MethodInfo MethodGetHashCode = typeof(object).GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public);
        public static readonly MethodInfo MethodToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public);
        public static readonly MethodInfo MethodConcat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string), typeof(string) });
    }


    public class EqualityFunctionsGenerator<TObject>
    {
        public static readonly Type TypeOfTObject = typeof(TObject);

        protected EqualityFunctionsGenerator()
        {
        }

        public static Func<TObject, TObject, bool> CreateEqualityComparer()
        {
            var x = Expression.Parameter(TypeOfTObject, "x");
            var y = Expression.Parameter(TypeOfTObject, "y");

            var result = (Expression)Expression.Constant(true, ObjectEqualityFunction.TypeOfBool);

            foreach (var property in GetProperties())
            {
                var comparison = CreatePropertyComparison(property, x, y);
                result = Expression.AndAlso(result, comparison);
            }

            return Expression.Lambda<Func<TObject, TObject, bool>>(result, x, y).Compile();
        }

        private static Expression CreatePropertyComparison(PropertyInfo property, Expression x, Expression y)
        {
            var type = property.PropertyType;

            var propertyOfX = GetPropertyValue(x, property);
            var propertyOfY = GetPropertyValue(y, property);

            return (type.IsValueType) ?
                                         CreateValueTypeComparison(propertyOfX, propertyOfY) :
                                         CreateReferenceTypeComparison(propertyOfX, propertyOfY);
        }

        private static Expression GetPropertyValue(Expression obj, PropertyInfo property)
        {
            return Expression.Property(obj, property);
        }

        private static Expression CreateReferenceTypeComparison(Expression x, Expression y)
        {
            return Expression.Call(ObjectEqualityFunction.MethodEquals, x, y);
        }

        private static Expression CreateValueTypeComparison(Expression x, Expression y)
        {
            return Expression.Equal(x, y);
        }

        public static IEnumerable<PropertyInfo> GetProperties()
        {
            return TypeOfTObject.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public static Func<TObject, int> CreateGetHashCode()
        {
            var obj = Expression.Parameter(TypeOfTObject, "obj");
            var result = (Expression)Expression.Constant(0);

            foreach (var property in GetProperties())
            {
                var hash = CreatePropertyGetHashCode(obj, property);
                result = Expression.ExclusiveOr(result, hash);
            }

            return Expression.Lambda<Func<TObject, int>>(result, obj).Compile();
        }

        private static Expression CreatePropertyGetHashCode(Expression obj, PropertyInfo property)
        {
            var type = property.PropertyType;

            var propertyOfObj = GetPropertyValue(obj, property);

            return type.IsValueType ?
                                        CreateValueTypeGetHashCode(propertyOfObj) :
                                        CreateReferenceTypeGetHashCode(propertyOfObj);
        }

        private static Expression CreateReferenceTypeGetHashCode(Expression value)
        {
            return Expression.Condition(
                Expression.Equal(Expression.Constant(null), value),
                Expression.Constant(0),
                Expression.Call(value, ObjectEqualityFunction.MethodGetHashCode));
        }

        private static Expression CreateValueTypeGetHashCode(Expression value)
        {
            return Expression.Call(value, ObjectEqualityFunction.MethodGetHashCode);
        }

        public static Func<TObject, string> CreateToString()
        {
            var obj = Expression.Parameter(TypeOfTObject, "obj");
            var result = (Expression)Expression.Constant(null);

            foreach (var property in GetProperties())
            {
                var toString = CreatePropertyToString(obj, property);
                if (result is ConstantExpression)
                {
                    result = toString;
                }
                else
                {
                    result = Expression.Call(ObjectEqualityFunction.MethodConcat, result,
                             Expression.Constant("~"), toString);
                }
            }

            return Expression.Lambda<Func<TObject, string>>(result, obj).Compile();
        }

        private static Expression CreatePropertyToString(Expression obj, PropertyInfo property)
        {
            var type = property.PropertyType;

            var propertyOfObj = GetPropertyValue(obj, property);

            return type.IsValueType ?
                                        CreateValueTypeToString(propertyOfObj) :
                                        CreateReferenceTypeToString(propertyOfObj);
        }

        private static Expression CreateReferenceTypeToString(Expression value)
        {
            return Expression.Condition(
                Expression.Equal(Expression.Constant(null), value),
                Expression.Constant("null"),
                Expression.Call(value, ObjectEqualityFunction.MethodToString));
        }

        private static Expression CreateValueTypeToString(Expression value)
        {
            return Expression.Call(value, ObjectEqualityFunction.MethodToString);
        }
    }
}