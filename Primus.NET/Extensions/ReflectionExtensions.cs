using System;
using System.Reflection;

namespace Primus.NET.Extensions
{
    public static class ReflectionExtensions
    {
        public class T
        {
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return type.FindInterfaces(new TypeFilter(FilterByName), interfaceType).Length > 0;
        }

        private static bool FilterByName(Type typeObj, object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }

        public static bool IsDerivedFromGenericType(this Type type, Type genericType)
        {
            return !(type == typeof(object)) && !(type == null) && (type.IsGenericType && type.GetGenericTypeDefinition() == genericType || type.BaseType.IsDerivedFromGenericType(genericType));
        }

        private static bool IsSimilarType(this Type thisType, Type type)
        {
            if (thisType.IsByRef)
            {
                thisType = thisType.GetElementType();
            }
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }
            bool result;
            if (thisType.IsArray && type.IsArray)
            {
                result = thisType.GetElementType().IsSimilarType(type.GetElementType());
            }
            else
            {
                if (thisType == type || (thisType.IsGenericParameter || thisType == typeof(T)) && (type.IsGenericParameter || type == typeof(T)))
                {
                    result = true;
                }
                else
                {
                    if (thisType.IsGenericType && type.IsGenericType)
                    {
                        Type[] genericArguments = thisType.GetGenericArguments();
                        Type[] genericArguments2 = type.GetGenericArguments();
                        if (genericArguments.Length == genericArguments2.Length)
                        {
                            for (int i = 0; i < genericArguments.Length; i++)
                            {
                                if (!genericArguments[i].IsSimilarType(genericArguments2[i]))
                                {
                                    result = false;
                                    return result;
                                }
                            }
                            result = true;
                            return result;
                        }
                    }
                    result = false;
                }
            }
            return result;
        }
    }
}