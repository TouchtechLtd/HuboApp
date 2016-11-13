﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class AddShiftViewModel
    {
        public List<string> vehicles { get; set; }
        public INavigation Navigation { get; set; }
        public ICommand AddBreakButton { get; set; }
        public string StartShift { get; set; }
        public string EndShift { get; set; }
        public string StartBreak { get; set; }
        public string EndBreak { get; set; }
        public string AddBreakText { get; set; }
        public string SaveText { get; set; }
        public string Date { get; set; }
        public string Vehicle { get; set; }
        


        public AddShiftViewModel()
        {
            StartShift = Resource.StartShift;
            StartBreak = Resource.StartBreak;
            EndShift = Resource.EndShift;
            EndBreak = Resource.EndBreak;
            AddBreakText = Resource.AddBreak;
            SaveText = Resource.Save;
            Date = Resource.Date;
            Vehicle = Resource.Vehicle;
            AddBreakButton = new Command(AddBreak);
            vehicles = new List<string>();
            vehicles.Add("ELM324");
            vehicles.Add("FJG012");
            vehicles.Add("KDG956");
            vehicles.Add("PFW337");
            vehicles.Add("FDG777");
        }

        public void Save()
        {

        }

        private void AddBreak()
        {

        }
    }
}