using MessageUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubo.iOS
{
    class Email_iOS : IEmail
    {
        MFMailComposeViewController mailController;
        string results;

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


                mailController.Finished += MailController_Finished;

                return Convert.ToBoolean(results);
            }
            return false;
        }

        private void MailController_Finished(object sender, MFComposeResultEventArgs e)
        {
            results = e.Result.ToString();
            e.Controller.DismissViewController(true, null);
        }
    }
}
