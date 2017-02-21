using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class EditShiftDetailsPage : ContentPage
    {
        EditShiftDetailsViewModel editShiftDetailsVM;

        public EditShiftDetailsPage(string instruction, DriveTable currentDrive = null, ShiftTable currentShift = null, BreakTable currentBreak = null)
        {
            InitializeComponent();
            editShiftDetailsVM = new EditShiftDetailsViewModel(instruction, currentDrive, currentShift, currentBreak);
            editShiftDetailsVM.Navigation = Navigation;
            BindingContext = editShiftDetailsVM;
            notePicker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            //Load details for Breaks
            if (instruction == "Breaks")
            {
                Title = Resource.BreaksText;
                //List<BreakTable> listOfBreaks = new List<BreakTable>();
                //listOfBreaks = editShiftDetailsVM.LoadBreaks();
                ////picker.Items.Add("2016-10-21 10:22 - 2016-10-21 14:32");
                //foreach (BreakTable breakItem in listOfBreaks)
                //{
                //    string StartTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.StartDate));
                //    string EndTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.EndDate));
                //    picker.Items.Add(StartTime + " - " + EndTime);
                //}
                //picker.Title = Resource.SelectBreak;
            }
            //Load details for Notes
            if (instruction == "Notes")
            {
                Title = Resource.NotesText;
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                //picker.Items.Add("2016-10-21 12:22 - Hit a Possom");
                foreach (NoteTable note in listOfNotes)
                {
                    if (note.Note.Length > 20)
                        notePicker.Items.Add(DateTime.Parse(note.Date).Date.ToString() + " - " + note.Note.Remove(20));
                    else
                        notePicker.Items.Add(DateTime.Parse(note.Date).Date.ToString() + " - " + note.Note);
                }
                notePicker.Title = Resource.SelectNote;
            }
            else if (instruction == "Drives")
            {
                Title = Resource.DriveText;
                List<VehicleTable> vehicles = new List<VehicleTable>();
                vehicles = editShiftDetailsVM.LoadVehicle();

                foreach (VehicleTable vehicle in vehicles)
                {
                    vehiclePicker.Items.Add(vehicle.Registration);
                }
                vehiclePicker.Title = Resource.SelectVehicle;

                if (editShiftDetailsVM.VehicleId != -1)
                    vehiclePicker.SelectedIndex = editShiftDetailsVM.VehicleId - 1;

                vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;

                breakList.ItemSelected += (sender, e) =>
                {
                    if (((ListView)sender).SelectedItem == null)
                        return;
                    ((ListView)sender).SelectedItem = null;
                    editShiftDetailsVM.SelectedBreak = (breakList.ItemsSource as ObservableCollection<BreakTable>).IndexOf(e.SelectedItem as BreakTable);
                    editShiftDetailsVM.EditBreakDetails("Breaks");
                };


            }
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.SelectedIndex = vehiclePicker.SelectedIndex;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.DisplayDetails(notePicker.SelectedIndex);
        }
    }
}
