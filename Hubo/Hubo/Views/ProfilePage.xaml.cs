namespace Hubo
{
    using Xamarin.Forms;

    public partial class ProfilePage : TabbedPage
    {

        private readonly ProfileViewModel profileVM = new ProfileViewModel();

        public ProfilePage()
        {
            InitializeComponent();
            profileVM.Navigation = Navigation;
            BindingContext = profileVM;
            ToolbarItem done = new ToolbarItem();
            ToolbarItem cancel = new ToolbarItem();
            done.Text = Resource.Save;
            cancel.Text = Resource.Cancel;
            done.Command = profileVM.SaveAndExit;
            cancel.Command = profileVM.CancelAndExit;
            ToolbarItems.Add(done);
            ToolbarItems.Add(cancel);
            Title = Resource.ProfileText;
        }

    }
}
