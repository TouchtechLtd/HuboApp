using MessageUI;
using System;
using System.Collections.Generic;
using System.Text;
using Hubo.iOS;
using Xamarin.Forms;
using UIKit;
using Foundation;

[assembly: Dependency(typeof(Email_iOS))]

namespace Hubo.iOS
{
    class Email_iOS : IEmail
    {
        MFMailComposeViewController mailController;

        public Email_iOS()
        {

        }

        public bool Email(string mailTo, string subject, List<string> filePaths)
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                mailController = new MFMailComposeViewController();

                mailController.SetToRecipients(new string[] { mailTo });
                mailController.SetSubject(subject);

                foreach (NSString path in filePaths)
                {
                    NSData data = NSData.FromFile(path);
                    NSString name = (NSString)System.IO.Path.GetFileName(path);

                    mailController.AddAttachmentData(data, "text/csv", name);
                }

                mailController.Finished += MailController_Finished;

                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(mailController, true, null);

                //var rootController = ((AppDelegate)(UIApplication.SharedApplication.Delegate)).Window.RootViewController.ChildViewControllers[0].ChildViewControllers[1].ChildViewControllers[0];
                //var navController = rootController as UINavigationController;

                //if (navController != null)
                //    rootController = navController.VisibleViewController;
                //rootController.PresentViewController(mailController, true, null);

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
