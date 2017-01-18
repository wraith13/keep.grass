using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaFriendsPage : ResponsiveContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		const int MaxFriendCount = 8;
        //VoidEntryCell[] FriendNameCellList = null;
        ListView List;
        Button AddButton;
        Button DeleteButton;
        bool IsChanged = false;

        public class ListItem
        {
            public ImageSource ImageSource { get; set; }
            public string Text { get; set; }
            public bool IsSeledted { get; set; }

            public static ListItem Make(string User)
            {
                return new ListItem
                {
                    ImageSource = GitHub.GetIconUrl(User),
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
                    .SetBindingList("ImageSource", "Text"),
            };
            List.ItemTapped += (sender, e) =>
            {
                DeleteButton.IsEnabled = null != (e.Item as ListItem);
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
                            NewUser =>
                            {
                                Settings.SetFriend(Settings.GetFriendCount(), NewUser);
                                UpdateList();
                                IsChanged = true;
                            },
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
                        var SelectedItem = List.SelectedItem as ListItem;
                        if (null != SelectedItem)
                        {
                            var OldFriendList = Settings.GetFriendList();
                            for (var i = OldFriendList.IndexOf(SelectedItem.Text); i < OldFriendList.Count(); ++i)
                            {
                                var NewFriend = OldFriendList.Skip(i +1).FirstOrDefault("");
                                Settings.SetFriend(i, NewFriend);
                            }
                            IsChanged = true;
                            UpdateList();
                        }
                    }
                ),
            };
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaSettingsPage.Rebuild();");

            var ButtonFrame = new Grid()
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.White,
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


