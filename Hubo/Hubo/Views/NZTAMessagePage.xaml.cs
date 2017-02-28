// <copyright file="NZTAMessagePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class NZTAMessagePage : ContentPage
    {
        private readonly NZTAMessageViewModel nztaMessageVM;

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
