using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Extensions
{
    public static class SqlDateTiemExt
    {
        private readonly static DateTime MinSqlDateTime = DateTime.Parse("1753/1/1");

        public static DateTime Fixed(this DateTime dateTime)
        {
            return dateTime < MinSqlDateTime ? MinSqlDateTime : dateTime;
        }

        public static DateTime? Fixed(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            return Fixed(dateTime.Value);
        }
    }
}
