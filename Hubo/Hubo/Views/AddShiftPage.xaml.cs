using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hubo;
using Xamarin.Forms;

namespace Hubo
{
    public partial class AddShiftPage : ContentPage
    {
        AddShiftViewModel addShiftVM = new AddShiftViewModel();

        public AddShiftPage()
        {
            InitializeComponent();
            BindingContext = addShiftVM;
            addShiftVM.Navigation = Navigation;
            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "Add Shift";
            ToolbarItems.Add(topLeftText);
            addButton.Clicked += AddButton_Clicked;
            Title = Resource.AddShiftText;
            addShiftVM.FullGrid = grid;

            startLocation.ReturnType = ReturnType.Next;
            startLocation.Next = endLocation;

            endLocation.ReturnType = ReturnType.Next;
            endLocation.Completed += EndLocation_Completed;
        }

        private void EndLocation_Completed(object sender, EventArgs e)
        {
            addShiftVM.SaveButton.Execute(null);
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            string[] buttons = new string[] { Resource.Break, Resource.NoteText, Resource.DriveText };
            var action = await DisplayActionSheet(Resource.AddBreakNote, Resource.Cancel, null, buttons);

            if (action != null && action != "Cancel")
            {
                addShiftVM.Add = action;
                await Navigation.PushModalAsync(new NavigationPage(new AddManBreakNotePage(action)));
            }
        }
    }
}
