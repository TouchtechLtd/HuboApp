using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class EditShiftPage : ContentPage
    {
        EditShiftViewModel editShiftVM = new EditShiftViewModel();
        List<ShiftTable> listOfShifts = new List<ShiftTable>();
        public EditShiftPage(DateTime selectedDate)
        {
            InitializeComponent();
            editShiftVM.Navigation = Navigation;
            BindingContext = editShiftVM;
            Title = Resource.EditShift;
            shiftPicker.Title = Resource.ShiftPickerTitle;

            listOfShifts = editShiftVM.Load(selectedDate);

            foreach(ShiftTable shift in listOfShifts)
            {
                //Format and add shifts to picker
                if(shift.EndTime==null)
                {
                    shift.EndTime = "Current";
                }
                shiftPicker.Items.Add(shift.StartTime + " - " + shift.EndTime);
            }

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftVM.LoadInfoFromShift(listOfShifts[shiftPicker.SelectedIndex]);
        }
    }
}
