using Sharper.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Extensions
{
    public static class SqlDataReaderExt
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>> propInfoCache
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>>();

        private static ConcurrentDictionary<int, string> fieldNameCache
            = new ConcurrentDictionary<int, string>();

        public static T To<T>(this SqlDataReader reader) where T : new()
        {
            if (reader == null || !reader.HasRows)
            {
                return default(T);
            }
            var res = new T();

            var propInfos = GetFieldInfo<T>();
            Parallel.For(0, reader.FieldCount, i =>
            {
                var name = reader.GetName(i).ToLower();
                if (propInfos.ContainsKey(name))
                {
                    var prop = propInfos[name];
                    var value = reader.GetValue(i);
                    if (!Convert.IsDBNull(value))
                    {
                        if (prop.PropertyType.IsValueType && prop.PropertyType.IsGenericType
                            && prop.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                            && prop.PropertyType.GetGenericArguments()[0].IsEnum)
                        {
                            var enumValue = Enum.Parse(prop.PropertyType.GetGenericArguments()[0], value.ToString());
                            prop.SetValue(res, enumValue);
                        }
                        else
                        {
                            prop.SetValue(res, value);
                        }
                    }
                }
            });
            return res;
        }

        public static List<T> ToList<T>(this SqlDataReader reader) where T : new()
        {
            if (reader == null || !reader.HasRows)
            {
                return default(List<T>);
            }
            var res = new List<T>();
            do
            {
                res.Add(reader.To<T>());
            }
            while (reader.Read());

            return res;
        }

        private static ConcurrentDictionary<string, PropertyInfo> GetFieldFromCache<T>()
        {
            var fullName = typeof(T).FullName;
            if (!propInfoCache.ContainsKey(fullName))
            {
                var fieldNames = GetFieldInfo<T>();
                propInfoCache.TryAdd(fullName, fieldNames);
            }
            return propInfoCache[fullName];
        }

        private static ConcurrentDictionary<string, PropertyInfo> GetFieldInfo<T>()
        {
            var res = new ConcurrentDictionary<string, PropertyInfo>();
            var props = typeof(T).GetProperties();
            Parallel.ForEach(props, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, prop =>
            {
                var key = prop.GetFieldName();
                if (!res.ContainsKey(key))
                {
                    res.TryAdd(key, prop);
                }
            });
            return res;
        }

        private static string GetFieldName(this PropertyInfo property)
        {
            var code = property.GetHashCode();
            if (!fieldNameCache.ContainsKey(code))
            {
                var fieldName = property.Name.ToLower();
                var attr = property.GetCustomAttribute<DataFieldAttribute>();
                if (attr != null)
                {
                    fieldName = attr.Name;
                }

                if (!fieldNameCache.ContainsKey(code))
                {
                    fieldNameCache.TryAdd(code, fieldName);
                }
            }
            return fieldNameCache[code];
        }
    }
}
