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
        EditShiftDetailsViewModel EditShiftDetailsVM = new EditShiftDetailsViewModel();
        public EditShiftDetailsPage(int instruction, ShiftTable currentShift)
        {
            InitializeComponent();
            BindingContext = EditShiftDetailsVM;
            EditShiftDetailsVM.Load(instruction, currentShift);
        }
    }
}
