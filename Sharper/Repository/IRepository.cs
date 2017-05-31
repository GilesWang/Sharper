using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Repository
{
    public interface IRepository<T> where T:new()
    {
        int Add(T t, bool insertIdentity = false);
        void Save(T t);
        List<T> FindBy(string name, object value, bool includeInValid = false, string compareOperation = "=");
        List<T> FindAll(bool includeInValid = false);
        List<T> FindBySql(string sql, List<SqlParameter> parameters);
        T Find(int value, bool includeValid = false);
        List<T> ExecuBySP(string spName, List<SqlParameter> parameters);
        void Delete(int id);
    }
}
