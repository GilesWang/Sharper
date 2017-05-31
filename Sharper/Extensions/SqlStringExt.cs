using Sharper.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Extensions
{
    public static class SqlStringExt
    {
        #region private
        private static readonly object _obj = new object();
        private static readonly ConcurrentDictionary<int, PropertyInfo[]> propertiesCache
            = new ConcurrentDictionary<int, PropertyInfo[]>();

        private static PropertyInfo[] GetPropertiesFromCache(Type t)
        {
            return propertiesCache.GetOrAdd(t.GetHashCode(), t.GetProperties());
        }
        #endregion

        #region generate sql
        public static string GetUpdateSql<T>(this T t, string tableName, string conditionColumnName, out List<SqlParameter> parameters)
            where T : IModel, new()
        {
            parameters = new List<SqlParameter>();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0} SET ", tableName);
            var props = GetPropertiesFromCache(t.GetType());
            string wherePart = string.Empty;
            foreach (var p in props)
            {
                var fieldAttr = p.GetCustomAttribute<FieldAttribute>();
                if (fieldAttr != null && !fieldAttr.Ignore)
                {
                    var propVal = p.GetValue(t, null);
                    object defaultValue = null;
                    var isValueType = p.PropertyType.IsValueType;
                    if (isValueType)
                    {
                        if ((!p.PropertyType.IsGenericType)
                            || (p.PropertyType.IsGenericType
                            && !p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                        {
                            defaultValue = 0;//非空值类型的默认值
                        }
                    }
                    var hasValue = propVal != defaultValue;
                    if (hasValue)
                    {
                        var fieldName = p.GetFieldName();
                        object propVal_new = null;
                        if (p.PropertyType == typeof(DateTime))
                        {
                            propVal_new = ((DateTime)propVal).Fixed();
                        }
                        else if (p.PropertyType == typeof(DateTime?))
                        {
                            propVal_new = ((DateTime?)propVal).Fixed();
                        }
                        else
                        {
                            propVal_new = propVal;
                        }

                        parameters.Add(new SqlParameter() { Value = propVal_new, ParameterName = fieldName });

                        if (fieldName.Equals(conditionColumnName.ToLower()))
                        {
                            wherePart = string.Format(" WHERE {0}=@{0}", fieldName);
                        }
                        else
                        {
                            sql.AppendFormat(" {0}=@{0},", fieldName);
                        }
                    }
                }
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(wherePart);
            return sql.ToString();
        }
        public static string GetInsertSql<T>(this T t, string tableName, out List<SqlParameter> parameters, out SqlParameter outputIdentity, bool insertIdentity = false)
            where T : IModel, new()
        {
            parameters = new List<SqlParameter>();
            StringBuilder sql = new StringBuilder();
            var props = GetPropertiesFromCache(t.GetType());
            StringBuilder paramPart = new StringBuilder();
            StringBuilder valuePart = new StringBuilder();
            foreach (var p in props)
            {
                var fieldAttr = p.GetCustomAttribute<FieldAttribute>();
                if (fieldAttr == null || fieldAttr.Ignore) continue;
                var propVal = p.GetValue(t, null);
                var fieldName = p.GetFieldName();
                if (!insertIdentity && (fieldName.ToLower() == "id" || fieldName.ToLower() == "sn")) continue;
                object defaultValue = null;//引用类型或可空值类型的默认值
                var IsValueType = p.PropertyType.IsValueType;
                if (IsValueType)
                {
                    if ((!p.PropertyType.IsGenericType)
                            || (p.PropertyType.IsGenericType && !p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                    {
                        defaultValue = 0;//非空值类型的默认值
                    }
                }
                var hasValue = propVal != defaultValue;
                if (hasValue)
                {
                    object propVal_new = null;
                    if (p.PropertyType == typeof(DateTime))
                    {
                        propVal_new = ((DateTime)propVal).Fixed();
                    }
                    else if (p.PropertyType == typeof(DateTime?))
                    {
                        propVal_new = ((DateTime?)propVal).Fixed();
                    }
                    else
                    {
                        propVal_new = propVal;
                    }

                    parameters.Add(new SqlParameter { Value = propVal_new, ParameterName = fieldName });
                    paramPart.AppendFormat("{0},", fieldName);
                    valuePart.AppendFormat(" @{0},", fieldName);
                }
            }
            paramPart.Remove(paramPart.Length - 1, 1);
            valuePart.Remove(valuePart.Length - 1, 1);
            sql.AppendFormat("Insert Into {0} ({1}) VALUES ({2});SELECT @id=scope_identity();", tableName, paramPart, valuePart);
            outputIdentity = new SqlParameter() { Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int, ParameterName = "id" };
            if (parameters != null)
            {
                parameters.Add(outputIdentity);
            }
            return sql.ToString();

        }
        public static string GetSelectSql<T>(this T t, string tableName, string conditionColumnName, object conditionColumnValue, out List<SqlParameter> parameters, bool include_invalid = false, string CompareOpration = "=", string logicDeleteField = "valid")
            where T : IModel, new()
        {
            parameters = new List<SqlParameter>();
            return "";
        }
        #endregion
    }
}
