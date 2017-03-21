// <copyright file="MenuPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel menuVM = new MenuViewModel();

        public MenuPage()
        {
            InitializeComponent();
            Title = "menu";
            Icon = "Menu-25.png";
            BindingContext = menuVM;
            CopyList = MenuList;
        }

        public ListView CopyList { get; }
    }
}
