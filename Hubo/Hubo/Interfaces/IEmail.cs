using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public interface IEmail
    {
        bool Email(string mailTo, string subject, List<string> filePaths);
    }
}
