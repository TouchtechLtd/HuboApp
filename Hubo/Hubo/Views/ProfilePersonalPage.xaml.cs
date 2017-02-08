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

            firstName.ReturnType = ReturnType.Next;
            firstName.Next = lastName;

            lastName.ReturnType = ReturnType.Next;
            lastName.Next = email;

            email.ReturnType = ReturnType.Next;
            email.Next = password;

            password.ReturnType = ReturnType.Next;
            password.Next = licenseNumber;

            licenseNumber.ReturnType = ReturnType.Next;
            licenseNumber.Next = licenseVersion;

            licenseVersion.ReturnType = ReturnType.Next;
            licenseVersion.Next = endorsements;

            endorsements.ReturnType = ReturnType.Done;

            Device.OnPlatform(iOS: () => Grid.SetRow(activityLabel, 1));
            Device.OnPlatform(iOS: () => Grid.SetRowSpan(activityLabel, 6));
        }
    }
}
