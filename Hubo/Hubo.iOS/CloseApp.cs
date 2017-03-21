// <copyright file="CloseApp.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System.Threading;
using Hubo.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApp))]

namespace Hubo.iOS
{
    public class CloseApp : ICloseApplication
    {
        public void CloseApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}
