using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class AddManBreakNotePage : ContentPage
    {
        AddManBreakNoteViewModel addBreakNoteVM;
        public AddManBreakNotePage(string instuction)
        {
            InitializeComponent();
            addBreakNoteVM = new AddManBreakNoteViewModel(instuction);
            addBreakNoteVM.Navigation = Navigation;
            BindingContext = addBreakNoteVM;
            if (instuction == "Break")
            {
                Title = Resource.AddBreak;
            }
            else if (instuction == "Note")
            {
                Title = Resource.AddNote;
            }

            addBreakNoteVM.InflatePage();
        }
    }
}
