using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Test
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform, String PathToApk)
        {
            //download the NUnit 3 adapter for running tests in Visual Studio from https://visualstudiogallery.msdn.microsoft.com/0da0f6bd-9bb6-4ae3-87a8-537788622f2d
            if (platform == Platform.Android)
            {
                try
                {
                    return ConfigureApp.Android.ApkFile(PathToApk).EnableLocalScreenshots().StartApp();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Mono Shared Runtime"))
                    {
                        throw new Exception("If you want to debug the tests uncheck 'Use Shared Runtime' in Android project > Properties > Android Options > Packaging and change the APK path in Configuration to point to the Debug folder.");
                    } else
                    {
                        throw e;
                    }
                }
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}

