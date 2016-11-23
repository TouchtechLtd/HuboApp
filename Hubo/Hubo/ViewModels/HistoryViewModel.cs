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
        public ObservableCollection<ChartDataPoint> HistoryChartData {get;set;}
        public ObservableCollection<ChartDataPoint> HistoryChartData1 {get;set;}
        public ObservableCollection<ChartDataPoint> HistoryChartData2 {get;set; }
        public string EditShiftText { get; set; }
        public string ExportText { get; set; }
        public ICommand EditShiftCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public INavigation Navigation { get; set; }
        public DateTime SelectedDate { get; set; }

        public HistoryViewModel()
        {
            this.HistoryChartData = new ObservableCollection<ChartDataPoint>();
            this.HistoryChartData.Add(new ChartDataPoint("1", 12));
            this.HistoryChartData.Add(new ChartDataPoint("2", 12));
            this.HistoryChartData.Add(new ChartDataPoint("3", 12));
            this.HistoryChartData.Add(new ChartDataPoint("4", 12));
            this.HistoryChartData.Add(new ChartDataPoint("5", 12));
            this.HistoryChartData.Add(new ChartDataPoint("6", 12));
            this.HistoryChartData.Add(new ChartDataPoint("7", 12));
            this.HistoryChartData.Add(new ChartDataPoint("8", 12));
            this.HistoryChartData.Add(new ChartDataPoint("9", 12));
            this.HistoryChartData.Add(new ChartDataPoint("10", 12));

            this.HistoryChartData1 = new ObservableCollection<ChartDataPoint>();
            this.HistoryChartData1.Add(new ChartDataPoint("1", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("2", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("3", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("4", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("5", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("6", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("7", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("8", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("9", 10));
            this.HistoryChartData1.Add(new ChartDataPoint("10", 10));

            this.HistoryChartData2 = new ObservableCollection<ChartDataPoint>();
            this.HistoryChartData2.Add(new ChartDataPoint("1", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("2", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("3", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("4", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("5", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("6", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("7", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("8", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("9", 2));
            this.HistoryChartData2.Add(new ChartDataPoint("10", 2));

            EditShiftText = Resource.EditShift;
            ExportText = Resource.Export;
            ExportCommand = new Command(Export);
            EditShiftCommand = new Command(EditShift);
            SelectedDate = DateTime.Now;
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
