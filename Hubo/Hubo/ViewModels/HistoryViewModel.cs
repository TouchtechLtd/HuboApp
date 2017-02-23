using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class HistoryViewModel : INotifyPropertyChanged
    {
        DatabaseService DbService = new DatabaseService();

        private ObservableCollection<ChartDataPoint> _historyChartData;
        private ObservableCollection<ChartDataPoint> _historyChartData1;

        public ObservableCollection<ChartDataPoint> HistoryChartData
        {
            get { return _historyChartData; }
            set
            {
                _historyChartData = value;
                OnPropertyChanged("HistoryChartData");
            }
        }

        public ObservableCollection<ChartDataPoint> HistoryChartData1
        {
            get { return _historyChartData1; }
            set
            {
                _historyChartData1 = value;
                OnPropertyChanged("HistoryChartData1");
            }
        }
        List<ShiftTable> listOfShifts = new List<ShiftTable>();

        public string EditShiftText { get; set; }
        public string ExportText { get; set; }
        public ICommand EditShiftCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public INavigation Navigation { get; set; }
        public DateTime SelectedDate { get; set; }
        public DateTime MaximumDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryViewModel()
        {
            HistoryChartData = new ObservableCollection<ChartDataPoint>();
            HistoryChartData1 = new ObservableCollection<ChartDataPoint>();
            //And time spent at work

            EditShiftText = Resource.EditShift;
            ExportText = Resource.Export;
            ExportCommand = new Command(Export);
            EditShiftCommand = new Command(EditShift);

            //Code to get shifts from the past week
            SelectedDate = DateTime.Now;
            listOfShifts = DbService.GetShifts(SelectedDate);

            foreach (ShiftTable shift in listOfShifts)
            {
                if (!(shift.EndDate == null))
                {
                    DateTime start = DateTime.Parse(shift.StartDate);
                    DateTime end = DateTime.Parse(shift.EndDate);

                    TimeSpan amountHoursWork = end - start;
                    int hoursWork = amountHoursWork.Hours;

                    int minsWork = amountHoursWork.Minutes;
                    minsWork = minsWork / 100;
                    string datePoint = start.Day + "/" + start.Month;

                    HistoryChartData.Add(new ChartDataPoint(datePoint, hoursWork + minsWork));
                    HistoryChartData1.Add(new ChartDataPoint(datePoint, (24 - hoursWork) + (0.6 - minsWork)));
                }
            }
            MaximumDate = DateTime.Now;

            MessagingCenter.Subscribe<string>("ShiftEdited", "ShiftEdited", (sender) =>
            {
                UpdateShift();
            });
        }

        public void UpdateShift()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                HistoryChartData = new ObservableCollection<ChartDataPoint>();
                HistoryChartData1 = new ObservableCollection<ChartDataPoint>();

                listOfShifts = DbService.GetShifts(SelectedDate);

                foreach (ShiftTable shift in listOfShifts)
                {
                    if (!(shift.EndDate == null))
                    {
                        DateTime start = new DateTime();
                        DateTime end = new DateTime();

                        start = DateTime.Parse(shift.StartDate);
                        end = DateTime.Parse(shift.EndDate);

                        TimeSpan amountHoursWork = end - start;
                        int hoursWork = amountHoursWork.Hours;

                        int minsWork = amountHoursWork.Minutes;
                        minsWork = minsWork / 100;
                        string datePoint = start.Day + "/" + start.Month;

                        HistoryChartData.Add(new ChartDataPoint(datePoint, hoursWork + minsWork));
                        HistoryChartData1.Add(new ChartDataPoint(datePoint, (24 - hoursWork) + (0.6 - minsWork)));
                    }
                }

            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void EditShift()
        {
            listOfShifts = DbService.GetShifts(SelectedDate);

            if (listOfShifts.Count == 0)
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.NoShiftsFound, Resource.DisplayAlertOkay);
            else
                Navigation.PushModalAsync(new EditShiftPage(listOfShifts));
        }

        private void Export()
        {
            Navigation.PushModalAsync(new ExportPage());
        }
    }
}
