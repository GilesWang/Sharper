using System;

namespace Sharper.Attributes
{
    /// <summary>
    /// 标志Model对应的数据库table以及其他的一些信息
    /// </summary>
    public class TableAttribute:Attribute
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 执行该表时需要用到的链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 连接串在web.config的名称
        /// </summary>
        public string ConnectionStringName { get; set; }
        /// <summary>
        /// 标识列名称
        /// </summary>
        public string Identity { get; set; }
    }
}
