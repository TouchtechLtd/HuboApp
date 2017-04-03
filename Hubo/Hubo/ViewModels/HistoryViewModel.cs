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
        private bool isShiftActive;

        public HistoryViewModel()
        {
            HistoryChartData = new ObservableCollection<CategoryData>();
            HistoryChartData1 = new ObservableCollection<CategoryData>();

            // And time spent at work
            EditShiftText = Resource.EditShift;
            ExportText = Resource.Export;
            ExportCommand = new Command(Export);

            // EditShiftCommand = new Command(async () => await EditShift(), () => IsShiftActive);

            // Code to get shifts from the past week
            SelectedDate = DateTime.Now;
            listOfShifts = dbService.GetShifts(SelectedDate);

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
                }

                if (shift.ActiveShift)
                {
                    IsShiftActive = true;
                }
                else
                {
                    IsShiftActive = false;
                }
            }

            MaximumDate = DateTime.Now;

            //MessagingCenter.Subscribe<string>("ShiftEdited", "ShiftEdited", (sender) =>
            //{
            //    UpdateShift();
            //});
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

        public string EditShiftText { get; set; }

        public string ExportText { get; set; }

        public bool IsShiftActive
        {
            get
            {
                return isShiftActive;
            }

            set
            {
                isShiftActive = value;
                OnPropertyChanged("IsShiftActive");
                //((Command)EditShiftCommand).ChangeCanExecute();
            }
        }

        public ICommand EditShiftCommand { get; set; }

        public ICommand ExportCommand { get; set; }

        public INavigation Navigation { get; set; }

        public DateTime SelectedDate { get; set; }

        public DateTime MaximumDate { get; set; }

        public void UpdateShift()
        {
            HistoryChartData = new ObservableCollection<CategoryData>();
            HistoryChartData1 = new ObservableCollection<CategoryData>();

            listOfShifts = dbService.GetShifts(SelectedDate);

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
                }

                if (shift.ActiveShift)
                {
                    IsShiftActive = true;
                }
                else
                {
                    IsShiftActive = false;
                }
            }

            OnPropertyChanged("HistoryChartData");
            OnPropertyChanged("HistoryChartData1");
        }

        internal void CheckActiveShift()
        {
            listOfShifts = dbService.GetShifts(SelectedDate);

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
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // private async Task EditShift()
        // {
        //    // listOfShifts = dbService.GetShifts(SelectedDate);
        //    ShiftTable currentShift = dbService.GetCurrentShift();
        //    listOfShifts = new List<ShiftTable>
        //    {
        //        currentShift
        //    };
        //    if (currentShift == null)
        //    {
        //        await UserDialogs.Instance.AlertAsync(Resource.NoShiftsFound, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
        //    }
        //    else
        //    {
        //        await Navigation.PushModalAsync(new EditShiftPage(listOfShifts));
        //    }
        // }
        private void Export()
        {
            Navigation.PushModalAsync(new ExportPage());
        }
    }

    public class CategoryData
    {
        public object Category { get; set; }

        public double Value { get; set; }
    }
}
