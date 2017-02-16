using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class EditShiftDetailsPage : ContentPage
    {
        EditShiftDetailsViewModel editShiftDetailsVM;

        public EditShiftDetailsPage(string instruction, ShiftTable currentShift)
        {
            InitializeComponent();
            editShiftDetailsVM = new EditShiftDetailsViewModel(instruction, currentShift);
            editShiftDetailsVM.Navigation = Navigation;
            BindingContext = editShiftDetailsVM;
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            //Load details for Breaks
            if(instruction=="Breaks")
            {
                Title = Resource.BreaksText;
                List<BreakTable> listOfBreaks = new List<BreakTable>();
                listOfBreaks = editShiftDetailsVM.LoadBreaks();
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                //picker.Items.Add("2016-10-21 10:22 - 2016-10-21 14:32");
                foreach (BreakTable breakItem in listOfBreaks)
                {
                    string StartTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.StartDate));
                    string EndTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.EndDate));
                    picker.Items.Add(StartTime + " - " + EndTime);
                }
                picker.Title = Resource.SelectBreak;
            }
            //Load details for Notes
            else if (instruction=="Notes")
            {
                Title = Resource.NotesText;
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                //picker.Items.Add("2016-10-21 12:22 - Hit a Possom");
                foreach (NoteTable note in listOfNotes)
                {
                    picker.Items.Add(note.Date + " - " + note.Note.Remove(0, 20));
                }
                picker.Title = Resource.SelectNote;
            }
            //Load details for Vehicles
            else if (instruction=="Vehicles")
            {
                Title = Resource.VehiclesText;
                List<DriveTable> usedVehicles = new List<DriveTable>();
                usedVehicles = editShiftDetailsVM.LoadVehicles();
                //picker.Items.Add("DB4501");
                foreach (DriveTable vehicle in usedVehicles)
                {
                    VehicleTable vehicleInfo = editShiftDetailsVM.LoadVehicleInfo(vehicle);
                    picker.Items.Add(vehicleInfo.Registration);
                }
                picker.Title = Resource.SelectVehicle;
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.DisplayDetails(picker.SelectedIndex);
        }
    }
}
