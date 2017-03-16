// <copyright file="Close_Application.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System.Threading;
using Hubo.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(Close_Application))]

namespace Hubo.iOS
{
    public class Close_Application : ICloseApplication
    {
        public void CloseApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}
