using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class ProfileCompanyPage : ContentPage
    {
        public ProfileCompanyPage()
        {
            InitializeComponent();

            name.ReturnType = ReturnType.Next;
            name.Next = address;

            address.ReturnType = ReturnType.Next;
            address.Next = companyEmail;

            companyEmail.ReturnType = ReturnType.Next;
            companyEmail.Next = phone;

            phone.ReturnType = ReturnType.Done;
        }
    }
}
