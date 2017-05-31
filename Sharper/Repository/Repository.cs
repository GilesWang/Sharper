using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Sharper.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.Attributes;

namespace Sharper.Repository
{
    public class Repository<T> : IRepository<T> where T : IModel, IValidate, new()
    {
        #region 字段
        private string tableName = string.Empty;
        private string connectionString = string.Empty;
        private string identityColumnName = string.Empty;
        #endregion

        public Repository()
        {
            var t = typeof(T);
            tableName = t.Name;
            var attrs = t.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr is TableAttribute)
                {
                    var tableAttr = attr as TableAttribute;
                    if (!string.IsNullOrWhiteSpace(tableAttr.ConnectionString))
                    {
                        connectionString = tableAttr.ConnectionString;
                    }
                    if (!string.IsNullOrWhiteSpace(tableAttr.ConnectionStringName))
                    {
                        //connectionString = ConfigurationManager.ConnectionStrings[tableAttr.ConnectionStringName].ConnectionString;
                    }
                    if (!string.IsNullOrWhiteSpace(tableAttr.TableName))
                    {
                        tableName = tableAttr.TableName;
                    }
                    if (!string.IsNullOrWhiteSpace(tableAttr.Identity))
                    {
                        identityColumnName = tableAttr.Identity;
                    }
                    break;
                }
            }
        }

        public int Add(T t, bool insertIdentity = false)
        {
            var result = -1;
            t.ValidateInsertModel();
            List<SqlParameter> paras = null;
            SqlParameter outputParameter = null;
            var insertSql = t.GetInsertSql(tableName, out paras, out outputParameter);
            return result;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuBySP(string spName, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public T Find(int value, bool includeValid = false)
        {
            throw new NotImplementedException();
        }

        public List<T> FindAll(bool includeInValid = false)
        {
            throw new NotImplementedException();
        }

        public List<T> FindBy(string name, object value, bool includeInValid = false, string compareOperation = "=")
        {
            throw new NotImplementedException();
        }

        public List<T> FindBySql(string sql, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public void Save(T t)
        {
            throw new NotImplementedException();
        }
    }
}
