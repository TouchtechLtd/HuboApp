// <copyright file="ProfileAddressPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class ProfileAddressPage : ContentPage
    {
        public ProfileAddressPage()
        {
            InitializeComponent();

            address1.ReturnType = ReturnType.Next;
            address1.Next = address2;

            address2.ReturnType = ReturnType.Next;
            address2.Next = address3;

            address3.ReturnType = ReturnType.Next;
            address3.Next = postCode;

            postCode.ReturnType = ReturnType.Next;
            postCode.Next = city;

            city.ReturnType = ReturnType.Next;
            city.Next = country;

            country.ReturnType = ReturnType.Done;
        }
    }
}
