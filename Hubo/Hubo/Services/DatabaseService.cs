using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;

namespace Hubo
{
    public class DatabaseService
    {
        public SQLiteConnection db;

        public DatabaseService()
        {
            db = DependencyService.Get<ISQLite>().GetConnection();
            db.CreateTable<UserTable>();
            db.CreateTable<VehicleTable>();
            db.CreateTable<BreakTable>();
            db.CreateTable<NoteTable>();
            db.CreateTable<ShiftTable>();
            CreateTestData();
        }

        private void CreateTestData()
        {
            List<VehicleTable> vehiclesList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");
            if(vehiclesList.Count==0)
            {
                VehicleTable test1 = new VehicleTable();
                VehicleTable test2 = new VehicleTable();
                VehicleTable test3 = new VehicleTable();
                VehicleTable test4 = new VehicleTable();
                test1.Company = "BD14";
                test2.Company = "BD14";
                test3.Company = "BD14";
                test4.Company = "BD14";

                test1.Make = "Mack";
                test2.Make = "Freightliner";
                test3.Make = "Western Star";
                test4.Make = "Kenworth";

                test1.Model = "Example 1";
                test2.Model = "Example 2";
                test3.Model = "Example 3";
                test4.Model = "Example 4";

                test1.Registration = "BSK474";
                test2.Registration = "YHG072";
                test3.Registration = "HED889";
                test4.Registration = "LWP127";

                test1.VehicleActive = 0;
                test2.VehicleActive = 0;
                test3.VehicleActive = 0;
                test4.VehicleActive = 0;

                db.Insert(test1);
                db.Insert(test2);
                db.Insert(test3);
                db.Insert(test4);
            }
        }

        internal bool CheckLoggedIn()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if(list.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal ObservableCollection<VehicleTable> GetVehicles()
        {
            List<VehicleTable> vehiclesList = new List<VehicleTable>();
            vehiclesList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");
            ObservableCollection<VehicleTable> vehiclesCollection = new ObservableCollection<VehicleTable>();
            foreach(VehicleTable vehicle in vehiclesList)
            {
                vehiclesCollection.Add(vehicle);
            }
            return vehiclesCollection;
        }

        internal bool CheckActiveShift()
        {
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if(shiftList.Count==0)
            {
                return false;
            }
            else if(shiftList.Count==1)
            {
                return true;
            }
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE SHIFT DISCOVERED", Resource.DisplayAlertOkay);
            return false;

        }

        internal VehicleTable GetCurrentVehicle()
        {
            VehicleTable currentVehicle;
            List<VehicleTable> currentVehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [VehicleActive] == 1");
            currentVehicle = currentVehicleList[0];
            return currentVehicle;
        }

        internal bool VehicleActive()
        {
            List<VehicleTable> currentVehicles = new List<VehicleTable>();
            currentVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [VehicleActive] == 1");
            if(currentVehicles.Count==0)
            {
                return false;
            }
            else if(currentVehicles.Count==1)
            {
                return true;
            }
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE VEHICLE ACTIVE", Resource.DisplayAlertOkay);
            return false;
        }

        internal bool StopBreak()
        {
            List<BreakTable> currentBreaks = new List<BreakTable>();
            currentBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if ((currentBreaks.Count == 0) || (currentBreaks.Count > 1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET ACTIVE BREAK", Resource.DisplayAlertOkay);
                return false;
            }
            BreakTable currentBreak = currentBreaks[0];
            currentBreak.EndTime = DateTime.Now.TimeOfDay.ToString();
            currentBreak.ActiveBreak = 0;
            db.Update(currentBreak);
            return true;
        }

        internal void InsertVehicle(VehicleTable vehicleToAdd)
        {
            db.Insert(vehicleToAdd);
            MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
        }

        internal bool Login(UserTable user)
        {
            db.Insert(user);
            return true;
        }

        internal bool StartBreak()
        {
            BreakTable newBreak = new BreakTable();
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if(CheckActiveShiftIsCorrect(activeShifts))
            {
                newBreak.ShiftKey = activeShifts[0].Key;
                newBreak.StartTime = DateTime.Now.TimeOfDay.ToString();
                newBreak.Date = DateTime.Now.ToString();
                newBreak.ActiveBreak = 1;
                db.Insert(newBreak);
                return true;
            }
            return false;
            
        }

        internal List<string> GetChecklist()
        {
            List<string> questions = new List<string>();
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            return questions;
        }

        internal void SetVehicleActiveOrInactive(VehicleTable currentVehicle)
        {
            List<VehicleTable> activeVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [VehicleActive] == 1");
            foreach(VehicleTable vehicle in activeVehicles)
            {
                vehicle.VehicleActive = 0;
                db.Update(vehicle);
            }
            db.Update(currentVehicle);
        }


        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
            MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal void StartShift()
        {
            ShiftTable newShift = new ShiftTable();
            List<VehicleTable> activeVehicles = new List<VehicleTable>();
            activeVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [VehicleActive] == 1");
            if(activeVehicles.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "MORE THAN ONE ACTIVE VEHICLE", "OK");
            }
            else if(activeVehicles.Count==1)
            {
                newShift.VehicleKey = activeVehicles[0].Key;
            }
            newShift.Date = DateTime.Now.ToString();
            newShift.ActiveShift = 1;
            newShift.TimeStart = DateTime.Now.TimeOfDay.ToString();
            db.Insert(newShift);
        }

        internal bool StopShift()
        {
            //Get current shift
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

            if(CheckActiveShiftIsCorrect(activeShifts))
            {
                if(activeShifts[0].VehicleKey == 0)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Please select a vehicle before ending your shift", Resource.DisplayAlertOkay);
                    return false;
                }
                ShiftTable activeShift = activeShifts[0];
                activeShift.TimeEnd = DateTime.Now.TimeOfDay.ToString();
                activeShift.ActiveShift = 0;
                db.Update(activeShift);
                return true;
            }
            return false;
        }

        private bool CheckActiveShiftIsCorrect(List<ShiftTable> activeShifts)
        {
            if (activeShifts.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "NO ACTIVE SHIFTS", "OK");
                return false;
            }
            else if (activeShifts.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "MORE THAN ACTIVE SHIFT", "OK");
                return false;
            }
            return true;
        }

        internal UserTable GetUserInfo()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            return list[0];
        }

        internal void UpdateUserInfo(UserTable user)
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
            db.Insert(user);
        }
    }
}
