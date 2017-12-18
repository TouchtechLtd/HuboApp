// <copyright file="ProfilePersonalPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class ProfilePersonalPage : ContentPage
    {
        public ProfilePersonalPage()
        {
            InitializeComponent();

            userName.ReturnType = ReturnType.Next;
            userName.Next = firstName;

            firstName.ReturnType = ReturnType.Next;
            firstName.Next = lastName;

            lastName.ReturnType = ReturnType.Next;
            lastName.Next = email;

            email.ReturnType = ReturnType.Next;
            email.Next = phone;

            phone.ReturnType = ReturnType.Done;
        }
    }
}
