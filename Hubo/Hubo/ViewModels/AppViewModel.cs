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
        RestService restService;

        public AppViewModel()
        {
            //LoadLoadingText();
        }

        private async void LoadTips()
        {
            //TODO: import tips from portal
            restService = new RestService();

            if (await restService.ImportTips())
            {

            }
        }

        private async void LoadLoadingText()
        {
            restService = new RestService();

            if (await restService.ImportLoadText())
            {
                LoadTips();
            }
            else
            {
                LoadTips();
            }
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
