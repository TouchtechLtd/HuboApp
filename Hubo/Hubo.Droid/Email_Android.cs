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
=======
            try
            {
                Context context = Android.App.Application.Context;

                var email = new Intent(Intent.ActionSendMultiple);
                email.AddFlags(ActivityFlags.NewTask);
                email.AddFlags(ActivityFlags.GrantReadUriPermission);
                email.AddFlags(ActivityFlags.GrantWriteUriPermission);
                email.SetType("message/rfc822");
                email.PutExtra(Intent.ExtraEmail, new string[] { mailTo });
                email.PutExtra(Intent.ExtraSubject, subject);

                List<IParcelable> uris = new List<IParcelable>();

                int name = 0;
                filePaths.ForEach(file =>
                {
                        var fileIn = new File(file);
                        if (!fileIn.Exists() && !fileIn.CanRead())
                        {
                            Toast.MakeText(context, "Attachment Error", ToastLength.Short);
                            return;
                        }
                        var bytes = System.IO.File.ReadAllBytes(fileIn.Path);
                        var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/Download/" + fileIn.Name;
                        System.IO.File.WriteAllBytes(externalPath, bytes);

                    name++;
                        var filePath = new File(externalPath);
                        filePath.SetReadable(true, false);
                        ParcelFileDescriptor.Open(filePath, ParcelFileMode.ReadOnly);
                        var uri = Android.Net.Uri.FromFile(filePath);
                        uris.Add(uri);
>>>>>>> feature/HUBOM-135-implement-the-research-to-expo
                });

                email.PutParcelableArrayListExtra(Intent.ExtraStream, uris);

<<<<<<< HEAD
                context.StartActivity(email);

                return true;
            //}
            //catch
            //{
            //    return false;
            //}
=======
                context.StartActivity(Intent.CreateChooser(email, "Send Email...").AddFlags(ActivityFlags.NewTask));

                return true;
        }
            catch
            {
                return false;
            }
>>>>>>> feature/HUBOM-135-implement-the-research-to-expo
        }
    }
}