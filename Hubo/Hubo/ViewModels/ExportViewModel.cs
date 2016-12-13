using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using CsvHelper;
using PCLStorage;

namespace Hubo
{
    class ExportViewModel
    {
        DatabaseService DbService = new DatabaseService();

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

        public async void Export()
        {
            EmailEntry = EmailEntry.Trim();
            if(Regex.IsMatch(EmailEntry, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
            {
                //var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "export.csv");
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("ExportFolder", CreationCollisionOption.OpenIfExists);
                IFile exportFile = await folder.CreateFileAsync("exportFile.csv", CreationCollisionOption.ReplaceExisting);

                using (Stream fileName = ToStream(Path.Combine(exportFile.Path, exportFile.Name)))
                {
                    using (var sw = new StreamWriter(fileName))
                    {
                        var writer = new CsvWriter(sw);

                        IEnumerable<ExportData> compiledData = DbService.GetExportData();

                        writer.WriteRecords(compiledData);
                    }
                }

                MessagingCenter.Send<string>("PopAfterExport", "PopAfterExport");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidEmail, Resource.DisplayAlertOkay);
            }

        }

        public static Stream ToStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
