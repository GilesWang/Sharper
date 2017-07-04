using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Attributes
{
    /// <summary>
    /// 字段属性
    /// </summary>
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Ignore { get; set; }
    }
}
