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

            foreach (ShiftTable shift in listOfShifts)
            {
                //Format and add shifts to picker
                if (shift.EndDate == null)
                {
                    shift.EndDate = "Current";
                }
                DateTime shiftStart = DateTime.Parse(shift.StartDate);
                DateTime shiftEnd = DateTime.Parse(shift.EndDate);

                shiftPicker.Items.Add(string.Format("{0:dd/MM}", shiftStart) + ") " + string.Format("{0:hh:mm tt}", shiftStart) + " - " + string.Format("{0:hh:mm tt}", shiftEnd));
            }

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftVM.LoadInfoFromShift(listOfShifts[shiftPicker.SelectedIndex]);
        }
    }
}
