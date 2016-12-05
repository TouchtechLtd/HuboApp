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

        public EditShiftDetailsPage(int instruction, ShiftTable currentShift)
        {
            InitializeComponent();
            editShiftDetailsVM = new EditShiftDetailsViewModel(instruction, currentShift);
            editShiftDetailsVM.Navigation = Navigation;
            BindingContext = editShiftDetailsVM;
            //editShiftDetailsVM.Load(instruction, currentShift);
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;
            if(instruction==1)
            {
                Title = Resource.BreaksText;
            }
            else if(instruction==2)
            {
                Title = Resource.NotesText;
            }
            else if(instruction==3)
            {
                Title = Resource.VehiclesText;
            }
            //Load details for Breaks
            if(instruction==1)
            {
                List<BreakTable> listOfBreaks = new List<BreakTable>();
                listOfBreaks = editShiftDetailsVM.LoadBreaks();
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                foreach(BreakTable breakItem in listOfBreaks)
                {
                    string StartTime = breakItem.StartTime.ToString();
                    string EndTime = breakItem.EndTime.ToString();
                    StartTime = StartTime.Substring(11);
                    EndTime = EndTime.Substring(11);
                    picker.Items.Add(StartTime + "-" + EndTime);
                }
                picker.Title = "Select a break period";
            }
            //Load details for Notes
            else if (instruction==2)
            {
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                foreach(NoteTable note in listOfNotes)
                {
                    picker.Items.Add(note.Location + " - " + note.Date);
                }
                picker.Title = "Select a note";
            }
            //Load details for Vehicles
            else if (instruction==3)
            {
                List<VehicleInUseTable> usedVehicles = new List<VehicleInUseTable>();
                usedVehicles = editShiftDetailsVM.LoadVehicles();
                foreach(VehicleInUseTable vehicle in usedVehicles)
                {
                    VehicleTable vehicleInfo = editShiftDetailsVM.LoadVehicleInfo(vehicle);
                    picker.Items.Add(vehicleInfo.Registration);
                }
                picker.Title = "Select a vehicle";
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.DisplayDetails(picker.SelectedIndex);
        }
    }
}
