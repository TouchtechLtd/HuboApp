// <copyright file="CloseApp.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Android.App;
using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApp))]

namespace Hubo.Droid
{
    public class CloseApp : ICloseApplication
    {
        public void CloseApplication()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}