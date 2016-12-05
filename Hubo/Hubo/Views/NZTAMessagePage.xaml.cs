using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class NZTAMessagePage : ContentPage
    {
        NZTAMessageViewModel nztaMessageVM;

        public NZTAMessagePage(int instruction)
        {
            InitializeComponent();
            nztaMessageVM = new NZTAMessageViewModel(instruction);
            nztaMessageVM.Navigation = Navigation;
            BindingContext = nztaMessageVM;
            Title = Resource.NZTA;
        }
    }
}
