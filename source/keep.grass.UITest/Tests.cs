using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace keep.grass.UITest
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void AppLaunches()
        {
            app.Screenshot("First screen.");
        }

        [Test]
        public void SettingsPage()
        {
            app.WaitForElement(c => c.Marked("action_bar_title").Text("keep.grass"));
            app.Tap(c => c.Text("設定"));
        }

        /*
        [Test]
        public void Repl()
        {
            app.Repl();
        }
        */
    }
}
