using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class AddShiftViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<VehicleTable> vehicles { get; set; }

        public INavigation Navigation { get; set; }
        public ICommand AddButton { get; set; }

        public string StartShift { get; set; }
        public string DashIcon { get; set; }
        public string AddText { get; set; }
        public string SaveText { get; set; }
        public string Date { get; set; }
        public string Vehicle { get; set; }
        public int EndShiftRow { get; set; }
        public string LocationText { get; set; }
        public string LocationStartData { get; set; }
        public string LocationEndData { get; set; }
        public string DriveText { get; set; }
        public string DriveStartData { get; set; }
        public string DriveEndData { get; set; }
        public string HuboText { get; set; }
        public string HuboStartData { get; set; }
        public string HuboEndData { get; set; }

        DatabaseService DbService = new DatabaseService();

        public AddShiftViewModel()
        {
            StartShift = Resource.Shift;
            DashIcon = Resource.Dash;
            DriveText = Resource.Drive;
            LocationText = Resource.Location;
            HuboText = Resource.HuboEquals;
            AddText = Resource.Add;
            SaveText = Resource.Save;
            Date = Resource.Date;
            Vehicle = Resource.Vehicle;
            AddButton = new Command(AddBreak);
            EndShiftRow = 4;
        }

        internal List<VehicleTable> GetVehicles()
        {
            vehicles = DbService.GetVehicles();
            return vehicles;
        }

        public void Save()
        {

        }

        private void AddBreak()
        {
            //if(EndShiftRow==4)
            //{
            //    EndShiftRow = 6;
            //}
            //else if(EndShiftRow==6)
            //{
            //    EndShiftRow = 8;
            //}
            //OnPropertyChanged("EndShiftRow");
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
