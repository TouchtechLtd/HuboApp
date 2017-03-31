[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    public partial class EndShiftConfirmPage : ContentPage
    {
        private readonly EndShiftConfirmViewModel endShiftConfirmVm = new EndShiftConfirmViewModel();

        public EndShiftConfirmPage()
        {
            InitializeComponent();
            BindingContext = endShiftConfirmVm;
        }
    }
}
