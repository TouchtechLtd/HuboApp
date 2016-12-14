using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Xamarin.Forms;
using Hubo.Droid;

[assembly: Dependency(typeof(Email_Android))]

namespace Hubo.Droid
{
    
    class Email_Android : IEmail
    {
        public Email_Android()
        {

        }

        public bool Email(string mailTo, string subject, List<string> filePaths)
        {
            //try
            //{
                Context context = Android.App.Application.Context;

                var email = new Intent(Intent.ActionSendMultiple);
                email.SetType("message/rfc822");
                email.PutExtra(Intent.ExtraEmail, mailTo);
                email.PutExtra(Intent.ExtraSubject, subject);

                var uris = new List<IParcelable>();
                filePaths.ForEach(file =>
                {
                    var fileIn = new File(file);
                    var uri = Android.Net.Uri.FromFile(fileIn);
                    uris.Add(uri);
                });

                email.PutParcelableArrayListExtra(Intent.ExtraStream, uris);
                email.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(email);

                return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
    }
}