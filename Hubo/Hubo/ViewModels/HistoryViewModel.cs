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


        private ObservableCollection<ChartDataPoint> histroyChartData;
        private ObservableCollection<ChartDataPoint> histroyChartData1;
        public ObservableCollection<ChartDataPoint> HistoryChartData
        {
            get { return histroyChartData; }
            set { histroyChartData = value; }
        }

        public ObservableCollection<ChartDataPoint> HistoryChartData1
        {
            get { return histroyChartData1; }
            set { histroyChartData1 = value; }
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
            listOfShifts = DbService.GetShiftsWeek(SelectedDate);

            foreach(ShiftTable shift in listOfShifts)
            {
                if(!(shift.EndTime == null))
                {
                    DateTime start = new DateTime();
                    DateTime end = new DateTime();

                    start = DateTime.Parse(shift.StartTime);
                    end = DateTime.Parse(shift.EndTime);

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
        }

        public void UpdateShift()
        {
            while (HistoryChartData.Any())
                HistoryChartData.RemoveAt(HistoryChartData.Count - 1);

            while (HistoryChartData1.Any())
                HistoryChartData1.RemoveAt(HistoryChartData1.Count - 1);

            listOfShifts = DbService.GetShiftsWeek(SelectedDate);

            foreach (ShiftTable shift in listOfShifts)
            {
                if (!(shift.EndTime == null))
                {
                    DateTime start = new DateTime();
                    DateTime end = new DateTime();

                    start = DateTime.Parse(shift.StartTime);
                    end = DateTime.Parse(shift.EndTime);

                    TimeSpan amountHoursWork = end - start;
                    int hoursWork = amountHoursWork.Hours;

                    int minsWork = amountHoursWork.Minutes;
                    minsWork = minsWork / 100;
                    string datePoint = start.Day + "/" + start.Month;

                    HistoryChartData.Add(new ChartDataPoint(datePoint, hoursWork + minsWork));
                    HistoryChartData1.Add(new ChartDataPoint(datePoint, (24 - hoursWork) + (0.6 - minsWork)));
                }
            }

            OnPropertyChanged("HistoryChartData");
            OnPropertyChanged("HistoryChartData1");
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
            Navigation.PushAsync(new EditShiftPage(SelectedDate));
        }

        private void Export()
        {
            Navigation.PushAsync(new ExportPage());
        }
    }
}
