using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hubo;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Test
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;
        String pathToApk;

        public Tests(Platform platform)
        {
            this.platform = platform;
            string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string dir = new FileInfo(currentFile).Directory.Parent.Parent.Parent.FullName;
            pathToApk = Path.Combine(dir, Configuration.APKpath);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform, pathToApk);
        }

        //[UNIT]
        //[TestCase(2, 2, 4)]
        //[TestCase(0, 0, 0)]
        //[TestCase(31, 11, 42)]
        //public void TestCaseSucceeds(int a, int b, int sum)
        //{
        //    Assert.That(a + b, Is.EqualTo(sum));
        //}

        //[UI]
        //[Test]
        //public void TestWelcomeEntry()
        //{
        //    AppResult[] results = app.Query(c => c.Marked("WelcomeEntry"));
        //    Assert.AreEqual(Resource.WelcomeEntry, results[0].Text);
        //    app.ClearText(c => c.Marked("WelcomeEntry"));
        //    app.EnterText(c => c.Marked("WelcomeEntry"), "Whoot!");
        //}

        //[Ignore("Remove to view the tree")]
        //[UI]
        //[Test]
        //public void TestRepl()
        //{
        //    app.Repl();
        //}
        [UI]
        [Test]
        public void TestNavigateToHomePageFromLogin()
        {
            app.Tap(c => c.Marked("LoginButton"));
            app.EnterText(c => c.Marked("Username"), "User");
            app.Tap(c => c.Marked("LoginButton"));
            app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
            app.ClearText(c => c.Marked("Username"));
            app.EnterText(c => c.Marked("Password"), "Pass");
            app.Tap(c => c.Marked("LoginButton"));
            app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
            app.EnterText(c => c.Marked("Username"), "User");
            app.Tap(c => c.Marked("LoginButton"));
            app.WaitForElement(Resource.NZTADisclaimer);
            app.Tap(c => c.Marked("NZTAButton"));
            app.WaitForElement(Resource.WelcomeEntry);
        }

        [UI]
        [Test]
        public void TestNavigateToAllMenuPagesFromLogin()
        {
            app.Tap(c => c.Marked("LoginButton"));
            app.EnterText(c => c.Marked("Username"), "User");
            app.Tap(c => c.Marked("LoginButton"));
            app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
            app.ClearText(c => c.Marked("Username"));
            app.EnterText(c => c.Marked("Password"), "Pass");
            app.Tap(c => c.Marked("LoginButton"));
            app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
            app.EnterText(c => c.Marked("Username"), "User");
            app.Tap(c => c.Marked("LoginButton"));
            app.WaitForElement(Resource.NZTADisclaimer);
            app.Tap(c => c.Marked("NZTAButton"));

            //Open drawer menu
            app.TapCoordinates(120, 120);

            //Open Settings page
            app.TapCoordinates(120, 120);

            app.WaitForElement("SettingsPage");

            //Close Settings page
            app.TapCoordinates(120, 120);
            app.TapCoordinates(120, 120);

            app.Tap(x => x.Marked("Profile"));
            app.WaitForElement("ProfilePage");

            app.TapCoordinates(120, 120);
            app.TapCoordinates(120, 120);

            app.Tap(x => x.Marked("Vehicles"));
            app.WaitForElement("VehiclesPage");

            app.TapCoordinates(120, 120);
            app.TapCoordinates(120, 120);

            app.Tap(x => x.Marked("History"));
            app.WaitForElement("HistoryPage");

            app.TapCoordinates(120, 120);
            app.TapCoordinates(120, 120);

            app.Tap(x => x.Marked("Add Shift"));
            app.WaitForElement("AddShiftPage");

            app.TapCoordinates(120, 120);
            app.TapCoordinates(120, 120);

            app.Tap(x => x.Marked("Sign Out"));
            app.WaitForElement("SignOutPage");


        }
        //[UI]
        //[Test]
        //public void TestNavigateToNZTAPageFromRegister()
        //{
        //    app.Tap(c => c.Marked("RegisterButton"));
        //    //app.tap(c => c.marked("registerbutton"));
        //    //app.tap(c => c.marked(resource.displayalertokay));
        //    app.EnterText(c => c.Marked("FirstName"), "User");
        //    app.Tap(c => c.Marked("RegisterButton"));
        //    app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
        //    app.EnterText(c => c.Marked("LastName"), "User");
        //    app.Tap(c => c.Marked("RegisterButton"));
        //    app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
        //    app.EnterText(c => c.Marked("Email"), "User");
        //    app.Tap(c => c.Marked("RegisterButton"));
        //    app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
        //    app.EnterText(c => c.Marked("Password"), "User");
        //    app.Tap(c => c.Marked("RegisterButton"));
        //    app.Tap(c => c.Marked(Resource.DisplayAlertOkay));
        //    app.WaitForElement("NZTA PAGE");
        //}
    }
}

public class UNIT : CategoryAttribute
{ }

public class UI : CategoryAttribute
{ }

