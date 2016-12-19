using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class MenuPage : ContentPage
    {
        public ListView CopyList { get; }

        MenuViewModel menuVM = new MenuViewModel();

        public MenuPage()
        {
            InitializeComponent();
            Title = "menu";
            Icon = "Menu-25.png";
            BindingContext = menuVM;
            CopyList = MenuList;
        }
    }
}
