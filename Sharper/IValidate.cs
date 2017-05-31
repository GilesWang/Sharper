using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper
{
    public interface IValidate
    {
        void ValidateInsertModel();
        void ValidateUpdateModel();
    }
}
