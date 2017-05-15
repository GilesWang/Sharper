using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Attributes
{
    public class DataFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public DataFieldAttribute()
        {

        }
        public DataFieldAttribute(string name)
        {
            Name = name;
        }
    }
}
