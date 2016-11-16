﻿using System;
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

                test1.Active = 0;
                test2.Active = 0;
                test3.Active = 0;
                test4.Active = 0;

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

        internal List<string> GetChecklist()
        {
            List<string> questions = new List<string>();
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            return questions;

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
            activeVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Active] == 1");
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
            //Check there is an active shift, or check of theres multiple active shifts
            if(activeShifts.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "NO ACTIVE SHIFTS", "OK");
                return false;
            }
            else if(activeShifts.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "MORE THAN ACTIVE SHIFT", "OK");
                return false;
            }

            ShiftTable activeShift = activeShifts[0];
            activeShift.TimeEnd = DateTime.Now.TimeOfDay.ToString();
            activeShift.ActiveShift = 0;
            db.Update(activeShift);
            return true;
            

            //Check if a vehicle has been assigned


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
