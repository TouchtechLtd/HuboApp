// <copyright file="Email_iOS.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Foundation;
using Hubo.iOS;
using MessageUI;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Email_iOS))]

namespace Hubo.iOS
{
    internal class Email_iOS : IEmail
    {
        private MFMailComposeViewController mailController;

        public Email_iOS()
        {
        }

        public bool Email(string mailTo, string subject, List<string> filePaths)
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                this.mailController = new MFMailComposeViewController();

                this.mailController.SetToRecipients(new string[] { mailTo });
                this.mailController.SetSubject(subject);

                foreach (NSString path in filePaths)
                {
                    NSData data = NSData.FromFile(path);
                    NSString name = (NSString)System.IO.Path.GetFileName(path);

                    this.mailController.AddAttachmentData(data, "text/csv", name);
                }

                this.mailController.Finished += this.MailController_Finished;

                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(this.mailController, true, null);

                return true;
            }

            return false;
        }

        private void MailController_Finished(object sender, MFComposeResultEventArgs e)
        {
            e.Controller.DismissViewController(true, null);
        }
    }
}
