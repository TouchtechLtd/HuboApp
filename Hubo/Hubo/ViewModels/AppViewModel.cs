using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class AppViewModel : INotifyPropertyChanged
    {
        //TODO add functions to call when app starts/sleeps/resumes
        public event PropertyChangedEventHandler PropertyChanged;
        DatabaseService db = new DatabaseService();

        public AppViewModel()
        {
           
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


}
