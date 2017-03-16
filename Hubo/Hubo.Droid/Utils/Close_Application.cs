// <copyright file="Close_Application.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Android.App;
using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Close_Application))]

namespace Hubo.Droid
{
    internal class Close_Application : ICloseApplication
    {
        public void CloseApplication()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}