using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class HistoryPage : ContentPage
    {
        HistoryViewModel historyVM = new HistoryViewModel();
        public HistoryPage()
        {
            InitializeComponent();
            historyVM.Navigation = Navigation;
            BindingContext = historyVM;
        }
    }
}
