using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class ExportViewModel
    {
        public string ExportDisclaimerText { get; set; }
        public string EmailText { get; set; }
        public string EmailEntry { get; set; }
        public string ExportText { get; set; }
        public ICommand ExportCommand { get; set; }
        public ExportViewModel()
        {
            ExportDisclaimerText = Resource.ExportDisclaimer;
            EmailText = Resource.Email;
            ExportText = Resource.Export;
            ExportCommand = new Command(Export);
            EmailEntry = "";
        }

        public void Export()
        {
            if(Regex.IsMatch(EmailEntry, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
            {
                MessagingCenter.Send<string>("PopAfterExport", "PopAfterExport");
            }
            else
            {
                App.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidEmail, Resource.DisplayAlertOkay);
            }

        }
    }
}
