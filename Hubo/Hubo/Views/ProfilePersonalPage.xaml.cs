using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
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

            phone.ReturnType = ReturnType.Next;
            phone.Next = address1;

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

            Device.OnPlatform(iOS: () => Grid.SetRow(activityLabel, 1));
            Device.OnPlatform(iOS: () => Grid.SetRowSpan(activityLabel, 6));
        }
    }
}
