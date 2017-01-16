using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public interface INotifyService
    {
        void LocalNotification(string title, string text, DateTime time);
    }
}
