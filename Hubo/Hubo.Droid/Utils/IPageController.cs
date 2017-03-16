// <copyright file="IPageController.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public interface IPageController
    {
        bool IgnoresContainerArea { get; set; }

        ObservableCollection<Element> InternalChildren { get; }

        Rectangle GetContainerArea();

        void SetContainerArea(Rectangle value);

        void SendAppearing();

        void SendDisappearing();
    }
}