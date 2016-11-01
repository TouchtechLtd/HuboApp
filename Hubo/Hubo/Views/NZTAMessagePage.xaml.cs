﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class NZTAMessagePage : ContentPage
    {
        NZTAMessageViewModel nztaMessageVM = new NZTAMessageViewModel();

        public NZTAMessagePage()
        {
            InitializeComponent();
            nztaMessageVM.Navigation = Navigation;
            BindingContext = nztaMessageVM;
        }
    }
}
