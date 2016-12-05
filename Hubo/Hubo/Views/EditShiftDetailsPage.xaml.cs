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
            //editShiftDetailsVM.Load(instruction, currentShift);
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            //Load details for Breaks
            if(instruction=="Breaks")
            {
                Title = Resource.BreaksText;
                List<BreakTable> listOfBreaks = new List<BreakTable>();
                listOfBreaks = editShiftDetailsVM.LoadBreaks();
                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = editShiftDetailsVM.LoadNotes();
                foreach(BreakTable breakItem in listOfBreaks)
                {
                    string StartTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.StartTime));
                    string EndTime = string.Format("{0:hh:mm tt}", DateTime.Parse(breakItem.EndTime));
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
                foreach(NoteTable note in listOfNotes)
                {
                    picker.Items.Add(note.Location + " - " + note.Date);
                }
                picker.Title = Resource.SelectNote;
            }
            //Load details for Vehicles
            else if (instruction=="Vehicles")
            {
                Title = Resource.VehiclesText;
                List<VehicleInUseTable> usedVehicles = new List<VehicleInUseTable>();
                usedVehicles = editShiftDetailsVM.LoadVehicles();
                foreach(VehicleInUseTable vehicle in usedVehicles)
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
