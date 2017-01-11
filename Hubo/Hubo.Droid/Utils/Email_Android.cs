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
using Android.Support.V4.Content;

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
            try
            {
                Context context = Android.App.Application.Context;

                var email = new Intent(Intent.ActionSendMultiple);
                email.SetType("message/rfc822");
                email.PutExtra(Intent.ExtraEmail, new string[] { mailTo });
                email.PutExtra(Intent.ExtraSubject, subject);

                List<IParcelable> uris = new List<IParcelable>();

                filePaths.ForEach(file =>
                {
                    var fileIn = new File(file);
                    if (!fileIn.Exists() || !fileIn.CanRead())
                    {
                        Toast.MakeText(context, "Attachment Error", ToastLength.Short);
                        return;
                    }
                    fileIn.SetReadable(true, false);
                    ParcelFileDescriptor.Open(fileIn, ParcelFileMode.ReadOnly);
                    var uri = FileProvider.GetUriForFile(Forms.Context.ApplicationContext, "triotech.hubo.droid.fileprovider", fileIn);
                    uris.Add(uri);
                });

                email.PutParcelableArrayListExtra(Intent.ExtraStream, uris);

                email.AddFlags(ActivityFlags.NewTask);
                email.AddFlags(ActivityFlags.GrantReadUriPermission);
                email.AddFlags(ActivityFlags.GrantWriteUriPermission);
                email.SetFlags(ActivityFlags.GrantReadUriPermission);
                email.SetFlags(ActivityFlags.GrantWriteUriPermission);

                context.StartActivity(Intent.CreateChooser(email, "Send Email...").AddFlags(ActivityFlags.NewTask));

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}