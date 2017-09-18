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
        string user = "chomado";
        string rival = "wraith13";

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
        public void Scenario()
        {
            //  select user
            app.WaitForElement(c => c.Marked("action_bar_title").Text("keep.grass"));
            app.Screenshot("MainPage::Init");
            app.Tap(c => c.Text("Settings"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Settings"));
            app.Screenshot("SettingsPage::Init");
            app.Tap(c => c.Text("unspecified"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Select a user"));
            app.Screenshot("SelectUserPage::Init");
            app.EnterText(c => c.Marked("search_src_text"), user);
            app.Tap(c => c.Text("Search"));
            app.WaitForElement(c => c.Class("FormsTextView").Text(user));
            app.Screenshot("SelectUserPage::Search");
            app.Tap(c => c.Class("FormsTextView").Text(user));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Settings"));
            app.Screenshot("SettingsPage::SelectedUser");
            app.Tap(c => c.Marked("up"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("keep.grass"));
            app.Screenshot("MainPage::SelectedUser");

            //  select rival
            app.Tap(c => c.Text("Settings"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Settings"));
            app.Screenshot("SettingsPage::Init2");
            app.Tap(c => c.Text("Rivals"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Rivals"));
            app.Screenshot("FriendsPage::Init");
            app.Tap(c => c.Text("Add"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Select a user"));
            app.Screenshot("SelectUserPage::Init2");
            app.EnterText(c => c.Marked("search_src_text"), rival);
            app.Tap(c => c.Text("Search"));
            app.WaitForElement(c => c.Class("FormsTextView").Text(rival));
            app.Screenshot("SelectUserPage::Search2");
            app.Tap(c => c.Class("FormsTextView").Text(rival));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Rivals"));
            app.Screenshot("FriendsPage::SelectedRival");
            app.Tap(c => c.Marked("up"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Settings"));
            app.Screenshot("SettingsPage::SelectedRival");
            app.Tap(c => c.Marked("up"));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("keep.grass"));
            app.Screenshot("MainPage::SelectedRival");

            //  show details
            app.Tap(c => c.Class("SKCanvasView").Index(0));
            app.WaitForElement(c => c.Marked("action_bar_title").Text("chomado"));
            app.Screenshot("DetailPage");
            //app.Tap(c => c.Text("Last Activity Stamp").Parent("TextCellRenderer_TextCellView").Sibling("View"));
            //app.WaitForElement(c => c.Marked("action_bar_title").Text("Activity"));
            //app.Screenshot("FeedPage");
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
