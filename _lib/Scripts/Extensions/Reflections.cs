using System;
using System.Linq;
using System.Reflection;

namespace SQLib.Extensions
{
    public static class Reflections
    {
        /// <summary>
        /// Calls a instanced generic method on the invoker using types defined by System.Type variables (runtime). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type[] types, object[] parameters = default(object[]))
        {
            return typeof(T)
                .GetMethods()
                .First(m => m.Name == methodName && m.IsGenericMethod)
                .MakeGenericMethod(types)
                .Invoke(invoker, parameters);
        }
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type type) => invoker.InvokeGeneric(methodName, new Type[] { type }, default(object[]));
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type type, object parameter = default(object)) => invoker.InvokeGeneric(methodName, new Type[] { type }, new object[] { parameter });
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type type, object[] parameters = default(object[])) => invoker.InvokeGeneric(methodName, new Type[] { type }, parameters);
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type[] types) => invoker.InvokeGeneric(methodName, types, default(object[]));
        public static object InvokeGeneric<T>(this T invoker, string methodName, Type[] types, object parameter = default(object)) => invoker.InvokeGeneric(methodName, types, new object[] { parameter });

        /// <summary>
        /// Calls a static generic method using types defined by Type variables (runtime). Used by calling typeof(StaticClass).InvokeGeneric(...)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeGeneric(this Type invoker, string methodName, Type[] types, object[] parameters = default(object[]))
        {
            return invoker
                .GetMethods()
                .First(m => m.Name == methodName && m.IsGenericMethod)
                .MakeGenericMethod(types)
                .Invoke(null, parameters);
        }

        public static object InvokeGeneric(this Type invoker, string methodName, Type type, object[] parameters = default(object[])) => invoker.InvokeGeneric(methodName, new Type[] { type }, parameters);
        public static object InvokeGeneric(this Type invoker, string methodName, Type[] types, object parameter = default(object)) => invoker.InvokeGeneric(methodName, types, new object[] { parameter });
        public static object InvokeGeneric(this Type invoker, string methodName, Type type, object parameter = default(object)) => invoker.InvokeGeneric(methodName, new Type[] { type }, new object[] { parameter });

        /// <summary>
        /// Copies the member values from one object to another based on matching member name and type
        /// </summary>
        public static void CopyFields(this object source, object target)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            FieldInfo[] sourceInfos = source.GetType().GetFields(flags);

            foreach (FieldInfo targetInfo in target.GetType().GetFields(flags))
            {
                foreach (FieldInfo sourceInfo in sourceInfos)
                {
                    if (sourceInfo.Name == targetInfo.Name && sourceInfo.FieldType == targetInfo.FieldType)
                    {
                        targetInfo.SetValue(target, sourceInfo.GetValue(source));
                    }
                }
            }
        }
    }

}