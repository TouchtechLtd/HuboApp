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
        public bool AllowBack;
        public AddNotePage(int instruction)
        {
            InitializeComponent();
            addNoteVM.Navigation = Navigation;
            BindingContext = addNoteVM;
            if(instruction==1)
            {
                AllowBack = true;
            }
            else
            {
                AllowBack = false;
            }
            addNoteVM.Load(instruction);
        }

        protected override bool OnBackButtonPressed()
        {
            if(AllowBack)
            {
                return base.OnBackButtonPressed();
            }
            return false;
        }
    }
}
