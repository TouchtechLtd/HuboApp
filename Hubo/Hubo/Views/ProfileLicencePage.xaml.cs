namespace Hubo
{
    using Xamarin.Forms;

    public partial class ProfileLicencePage : ContentPage
    {
        private readonly ProfileViewModel profileVM = new ProfileViewModel();

        public ProfileLicencePage()
        {
            InitializeComponent();
            BindingContext = profileVM;
            licenceList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}
