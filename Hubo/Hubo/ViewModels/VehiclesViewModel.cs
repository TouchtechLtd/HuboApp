using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hubo
{
    class VehiclesViewModel : INotifyPropertyChanged
    {
        public string SearchVehiclesPlaceholder { get; set; }
        public string RegistrationText { get; set; }
        public string MakeText { get; set; }
        public string ModelText { get; set; }
        public string CompanyText { get; set; }
        public string SwitchText { get; set; }
        public string EditVehicleText { get; set; }
        public string AddVehicleText { get; set; }
        public ICommand EditVehicleCommand { get; set; }
        public ICommand AddVehicleCommand { get; set; }
        public ICommand SearchVehiclesCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public VehiclesViewModel()
        {
            SearchVehiclesPlaceholder = Resource.SearchVehiclesPlaceholder;
            RegistrationText = Resource.RegistrationText;
            MakeText = Resource.MakeText;
            ModelText = Resource.ModelText;
            CompanyText = Resource.CompanyText;
            SwitchText = Resource.SwitchTextInActive;
            EditVehicleText = Resource.EditVehicleText;
            AddVehicleText = Resource.AddVehicleText;
        }

        public void ToggleSwitch(bool toggle)
        {
            if(toggle)
            {
                SwitchText = Resource.SwitchTextActive;
            }
            else
            {
                SwitchText = Resource.SwitchTextInActive;
            }
            OnPropertyChanged("SwitchText");
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
