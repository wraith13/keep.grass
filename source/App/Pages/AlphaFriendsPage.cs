using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;

namespace keep.grass.App
{
    public class AlphaFriendsPage : ResponsiveContentPage
    {
        AlphaApp Root = AlphaFactory.MakeSureApp();
        AlphaLanguage L = AlphaFactory.MakeSureLanguage();

        const int MaxFriendCount = 8;
        //VoidEntryCell[] FriendNameCellList = null;
        ListView List;
        Button AddButton;
        Button DeleteButton;
        bool IsChanged = false;

        static public class ListItem
        {
            public static object Make(string User)
            {
                return new
                {
                    ImageSourceUrl = GitHub.GetIconUrl(User),
                    Text = User,
                };
            }
        }

        public AlphaFriendsPage()
        {
            Title = L["Rivals"];
            List = new ListView
            {
                ItemTemplate = new DataTemplateEx(AlphaFactory.GetGitHubUserCellType())
                    .SetBindingList("ImageSourceUrl", "Text"),
            };
            List.ItemTapped += (sender, e) =>
            {
                DeleteButton.IsEnabled = null != e.Item;
            };
            AddButton = new Button
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = L["Add"],
                Command = new Command
                (
                    o => AlphaFactory
                        .MakeSureApp()
                        .ShowSelectUserPage
                        (
                            NewUser => AddUser(NewUser),
                            Settings.GetFriendList()
                                .Concat(Settings.UserName)
                                .Where(i => !string.IsNullOrWhiteSpace(i))
                                .ToList()
                        )
                ),
            };
            DeleteButton = new Button
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = L["Delete"],
                Command = new Command
                (
                    o =>
                    {
                        var User = List.SelectedItem.GetValue<string>("Text");
                        if (null != User)
                        {
                            DeleteUser(User);
                        }
                    }
                ),
            };
        }
        public void AddUser(string NewUser)
        {
            DeleteUser(NewUser);
            Settings.SetFriend(Settings.GetFriendCount(), NewUser);
            IsChanged = true;
            UpdateList();
            AlphaFactory.MakeSureDomain().UpdateLastPublicActivityCoreAsync(NewUser).LeavingThrown();
        }
        public void DeleteUser(string TargetUser)
        {
            var Index = Settings.GetFriendList().IndexOf(TargetUser);
            if (0 <= Index)
            {
                int FriendCount = Settings.GetFriendCount();
                for (var i = Index; i < FriendCount + 1; ++i)
                {
                    Settings.SetFriend(i, Settings.GetFriend(i + 1));
                }
                Settings.SetFriend(FriendCount, "");
                IsChanged = true;
                UpdateList();
            }
        }

        public override void Build()
        {
            base.Build();
            Debug.WriteLine("AlphaSettingsPage.Rebuild();");

            var ButtonFrame = new Grid()
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                //BackgroundColor = Color.White,
            }
            .HorizontalJustificate
            (
                AddButton,
                DeleteButton
            );

            Content = new StackLayout
            {
                Children =
                {
                    List,
                    ButtonFrame,
                },
            };
            UpdateList();
            AlphaTheme.Apply(this);
        }
        public void UpdateList()
        {
            var Source = Settings.GetFriendList();
            List.ItemsSource = Source.Select(i => ListItem.Make(i));
            AddButton.IsEnabled = Source.Count() < MaxFriendCount;
            DeleteButton.IsEnabled = false;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (IsChanged)
            {
                Root.RebuildMainPage();
                Root.OnChangeSettings();
            }
        }
    }
}


