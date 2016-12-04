using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class HistoryViewModel
    {
        DatabaseService DbService = new DatabaseService();
        public ObservableCollection<ChartDataPoint> HistoryChartData {get;set;}
        public ObservableCollection<ChartDataPoint> HistoryChartData1 {get;set;}
        List<ShiftTable> listOfShifts = new List<ShiftTable>();

        public string EditShiftText { get; set; }
        public string ExportText { get; set; }
        public ICommand EditShiftCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public INavigation Navigation { get; set; }
        public DateTime SelectedDate { get; set; }
        public DateTime MaximumDate { get; set; }

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
            //listOfShifts = DbService.GetShiftsWeek(SelectedDate);

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
