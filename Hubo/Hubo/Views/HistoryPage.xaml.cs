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
        DatabaseService DbService = new DatabaseService();
        public HistoryPage()
        {
            InitializeComponent();
            HistoryViewModel historyVM = new HistoryViewModel();
            historyVM.Navigation = Navigation;
            BindingContext = historyVM;
            Title = Resource.HistoryText;
            LoadTip();
        }

        public async void LoadTip()
        {
            if (DbService.ShowTip("HistoryViewModel"))
            {
                bool tipResult = await DisplayAlert(Resource.Tip, Resource.HistoryTip, Resource.GotIt, Resource.DontShowAgain);
                if (!tipResult)
                {
                    DbService.HideTip("HistoryViewModel");
                }
            }

        }
    }
}
