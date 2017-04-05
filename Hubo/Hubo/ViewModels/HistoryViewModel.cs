// <copyright file="HistoryViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();

        private List<ShiftTable> listOfShifts = new List<ShiftTable>();
        private ObservableCollection<CategoryData> historyChartData;
        private ObservableCollection<CategoryData> historyChartData1;

        private bool shiftsAvailable;

        public HistoryViewModel()
        {
            HistoryChartData = new ObservableCollection<CategoryData>();
            HistoryChartData1 = new ObservableCollection<CategoryData>();

            // And time spent at work
            ExportText = Resource.Export;
            ExportCommand = new Command(Export);

            // Code to get shifts from the past week
            SelectedDate = DateTime.Now;
            listOfShifts = dbService.GetShifts(SelectedDate);

            List<string> number = new List<string>();
            foreach (ShiftTable shift in listOfShifts)
            {
                if (shift.EndDate != null)
                {
                    DateTime start = DateTime.Parse(shift.StartDate);
                    DateTime end = DateTime.Parse(shift.EndDate);

                    TimeSpan amountHoursWork = end - start;
                    int hoursWork = amountHoursWork.Hours;

                    int minsWork = amountHoursWork.Minutes;
                    minsWork = minsWork / 100;
                    string datePoint = start.Day + "/" + start.Month;

                    HistoryChartData.Add(new CategoryData { Category = datePoint, Value = hoursWork + minsWork });
                    HistoryChartData1.Add(new CategoryData { Category = datePoint, Value = (24 - hoursWork) + (0.6 - minsWork) });

                    if (!number.Contains(datePoint))
                    {
                        number.Add(datePoint);
                    }
                }
            }

            if (number.Count < 5)
            {
                for (int i = 0; i < (5 - number.Count); i++)
                {
                    HistoryChartData.Add(new CategoryData { Category = (i + 1) + "/3", Value = 0 });
                    HistoryChartData1.Add(new CategoryData { Category = (i + 1) + "/3", Value = 0 });
                }
            }

            if (HistoryChartData.Count > 0)
            {
                ShiftsAvailable = true;
            }
            else
            {
                ShiftsAvailable = false;
            }

            MaximumDate = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CategoryData> HistoryChartData
        {
            get
            {
                return historyChartData;
            }

            set
            {
                historyChartData = value;
                OnPropertyChanged("HistoryChartData");
            }
        }

        public ObservableCollection<CategoryData> HistoryChartData1
        {
            get
            {
                return historyChartData1;
            }

            set
            {
                historyChartData1 = value;
                OnPropertyChanged("HistoryChartData1");
            }
        }

        public bool ShiftsAvailable
        {
            get
            {
                return shiftsAvailable;
            }

            set
            {
                shiftsAvailable = value;
                OnPropertyChanged("ShiftsAvailable");
            }
        }

        public string ExportText { get; set; }

        public ICommand ExportCommand { get; set; }

        public INavigation Navigation { get; set; }

        public DateTime SelectedDate { get; set; }

        public DateTime MaximumDate { get; set; }

        public void UpdateShift()
        {
            HistoryChartData = new ObservableCollection<CategoryData>();
            HistoryChartData1 = new ObservableCollection<CategoryData>();

            listOfShifts = dbService.GetShifts(SelectedDate);

            List<string> number = new List<string>();

            foreach (ShiftTable shift in listOfShifts)
            {
                if (shift.EndDate != null)
                {
                    DateTime start = default(DateTime);
                    DateTime end = default(DateTime);

                    start = DateTime.Parse(shift.StartDate);
                    end = DateTime.Parse(shift.EndDate);

                    TimeSpan amountHoursWork = end - start;
                    int hoursWork = amountHoursWork.Hours;

                    int minsWork = amountHoursWork.Minutes;
                    minsWork = minsWork / 100;
                    string datePoint = start.Day + "/" + start.Month;

                    HistoryChartData.Add(new CategoryData { Category = datePoint, Value = hoursWork + minsWork });
                    HistoryChartData1.Add(new CategoryData { Category = datePoint, Value = (24 - hoursWork) + (0.6 - minsWork) });

                    if (!number.Contains(datePoint))
                    {
                        number.Add(datePoint);
                    }
                }
            }

            if (number.Count < 5)
            {
                for (int i = 0; i < (5 - number.Count); i++)
                {
                    HistoryChartData.Add(new CategoryData { Category = (i + 1) + "/3", Value = 0 });
                    HistoryChartData1.Add(new CategoryData { Category = (i + 1) + "/3", Value = 0 });
                }
            }

            if (HistoryChartData.Count > 0)
            {
                ShiftsAvailable = true;
            }
            else
            {
                ShiftsAvailable = false;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Export()
        {
            await UserDialogs.Instance.AlertAsync("Export Page under construction", "Coming Soon", Resource.Okay);

            //await Navigation.PushModalAsync(new ExportPage());
        }
    }

    public class CategoryData
    {
        public object Category { get; set; }

        public double Value { get; set; }
    }
}
