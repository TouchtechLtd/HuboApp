﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class ProfileViewModel
    {
        public ICommand SaveAndExit { get; set; }
        public ICommand CancelAndExit { get; set; }
        public INavigation Navigation { get; set; }

        public ProfileViewModel()
        {
            SaveAndExit = new Command(SaveAndPop);
            CancelAndExit = new Command(CancelAndPop);
        }

        private void CancelAndPop(object obj)
        {
            Navigation.PopModalAsync();
        }

        private void SaveAndPop(object obj)
        {
            //TODO: Implement save of details written in
            Navigation.PopModalAsync();
        }
    }
}
