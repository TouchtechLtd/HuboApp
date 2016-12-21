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

                startBreakNote.ReturnType = ReturnType.Next;
                startBreakNote.Next = startBreakLocation;

                startBreakLocation.ReturnType = ReturnType.Next;
                startBreakLocation.Next = startBreakHubo;

                startBreakHubo.ReturnType = ReturnType.Next;
                startBreakHubo.Next = endBreakNote;

                endBreakNote.ReturnType = ReturnType.Next;
                endBreakNote.Next = endBreakLocation;

                endBreakLocation.ReturnType = ReturnType.Next;
                endBreakLocation.Next = endBreakHubo;

                endBreakHubo.ReturnType = ReturnType.Done;
            }
            else if (instuction == "Note")
            {
                Title = Resource.AddNote;

                noteDetail.ReturnType = ReturnType.Next;
                noteDetail.Next = noteLocation;

                noteLocation.ReturnType = ReturnType.Next;
                noteLocation.Next = noteHubo;

                noteHubo.ReturnType = ReturnType.Done;
            }

            addBreakNoteVM.InflatePage();
        }
    }
}
