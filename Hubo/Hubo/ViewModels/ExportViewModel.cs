// <copyright file="ExportViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using CsvHelper;
    using PCLStorage;
    using Xamarin.Forms;
    using XLabs;

    internal class ExportViewModel
    {
        private readonly DatabaseService dbService = new DatabaseService();

        public ExportViewModel()
        {
            ExportDisclaimerText = Resource.ExportDisclaimer;
            EmailText = Resource.Email;
            ExportText = Resource.Export;
            ExportCommand = new RelayCommand(async () => await Export());
            EmailEntry = string.Empty;
        }

        public string ExportDisclaimerText { get; set; }

        public string EmailText { get; set; }

        public string EmailEntry { get; set; }

        public string ExportText { get; set; }

        public ICommand ExportCommand { get; set; }

        public async Task Export()
        {
            EmailEntry = EmailEntry.Trim();
            if (Regex.IsMatch(EmailEntry, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
            {
                bool emailClient;
                List<string> filePaths = new List<string>();

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("ExportFolder", CreationCollisionOption.OpenIfExists);

                IFile exportShift = await folder.CreateFileAsync("exportShift.csv", CreationCollisionOption.ReplaceExisting);
                IFile exportVehicle = await folder.CreateFileAsync("exportVehicle.csv", CreationCollisionOption.ReplaceExisting);

                IEnumerable<ExportShift> compiledShiftData = dbService.GetExportShift();
                IEnumerable<ExportBreak> compiledBreakData = dbService.GetExportBreak();
                IEnumerable<ExportNote> compiledNoteData = dbService.GetExportNote();
                IEnumerable<ExportVehicle> compiledVehicleData = dbService.GetExportVehicle();

                using (Stream file = await exportShift.OpenAsync(FileAccess.ReadAndWrite))
                using (TextWriter sw = new StreamWriter(file))
                using (var writer = new CsvWriter(sw))
                {
                    writer.WriteRecords(compiledShiftData);
                }

                filePaths.Add(exportShift.Path);

                if (compiledBreakData != null)
                {
                    IFile exportBreak = await folder.CreateFileAsync("exportBreak.csv", CreationCollisionOption.ReplaceExisting);

                    using (Stream file = await exportBreak.OpenAsync(FileAccess.ReadAndWrite))
                    using (TextWriter sw = new StreamWriter(file))
                    using (var writer = new CsvWriter(sw))
                    {
                        writer.WriteRecords(compiledBreakData);
                    }

                    filePaths.Add(exportBreak.Path);
                }

                if (compiledNoteData != null)
                {
                    IFile exportNote = await folder.CreateFileAsync("exportNote.csv", CreationCollisionOption.ReplaceExisting);

                    using (Stream file = await exportNote.OpenAsync(FileAccess.ReadAndWrite))
                    using (TextWriter sw = new StreamWriter(file))
                    using (var writer = new CsvWriter(sw))
                    {
                        writer.WriteRecords(compiledNoteData);
                    }

                    filePaths.Add(exportNote.Path);
                }

                using (Stream file = await exportVehicle.OpenAsync(FileAccess.ReadAndWrite))
                using (TextWriter sw = new StreamWriter(file))
                using (var writer = new CsvWriter(sw))
                {
                    writer.WriteRecords(compiledVehicleData);
                }

                filePaths.Add(exportVehicle.Path);

                emailClient = DependencyService.Get<IEmail>().Email(EmailEntry, "Last 7 Days of Shifts", filePaths);

                if (emailClient)
                {
                    MessagingCenter.Send<string>("PopAfterExport", "PopAfterExport");
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.EmailError, Resource.Alert, Resource.DisplayAlertOkay);
                    return;
                }
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync(Resource.InvalidEmail, Resource.Alert, Resource.DisplayAlertOkay);
            }
        }
    }
}
