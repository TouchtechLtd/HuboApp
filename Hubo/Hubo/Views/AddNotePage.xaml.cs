using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class AddNotePage : ContentPage
    {
        AddNoteViewModel addNoteVM = new AddNoteViewModel();
        public AddNotePage()
        {
            InitializeComponent();
            BindingContext = addNoteVM;
            datePicker.Date = DateTime.Now;
            timePicker.Time = DateTime.Now.TimeOfDay;
            
        }
    }
}
