using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;
using System.Diagnostics;

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
                var User = e.Item as ListItem;
                if (null != User)
                {
                    User.IsSeledted = !User.IsSeledted;
                }
            };
            List.ItemsSource = Settings.GetFriendList()
                .Select(i => ListItem.Make(i));
            /*
			FriendNameCellList = Enumerable.Range(0, MaxFriendCount)
         		.Select
               	(
               		i =>
					{
						var Cell = AlphaFactory.MakeEntryCell();
						Cell.Label = L["User ID"];
						return Cell;
					}
              	)
             	.ToArray();
         	*/
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
                                List.ItemsSource = Settings.GetFriendList()
                                    .Select(i => ListItem.Make(i));
                                Root.OnChangeSettings();
                            }
                        )
                ),
            };
            DeleteButton = new Button
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = L["Delete"],
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
		}
		/*protected override void OnAppearing()
		{
			base.OnAppearing();

			for (var i = 0; i < FriendNameCellList.Count(); ++i)
			{
				FriendNameCellList[i].Text = Settings.GetFriend(i);
			}
		}*/
        /*
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			var IsChanged = false;
			var OldFriendCount = Settings.GetFriendCount();

			var NewFriendList = new List<string>();
			for (var i = 0; i < FriendNameCellList.Count(); ++i)
			{
				var NewFriend = FriendNameCellList[i].Text.Trim();
				if
				(
					!string.IsNullOrWhiteSpace(NewFriend) &&
					!NewFriendList.Select(f => f.ToLower()).Contains(NewFriend.ToLower())
				)
				{
					NewFriendList.Add(NewFriend);
				}
			}
			for (var i = 0; i < FriendNameCellList.Count(); ++i)
			{
				var NewFriend = NewFriendList.Skip(i).FirstOrDefault("");
				if (Settings.GetFriend(i) != NewFriend)
				{
					Settings.SetFriend(i, NewFriend);
					//これ相当のデータが Friend ごとに必要なんじゃない？
					//Settings.IsValidUserName = false;
					IsChanged = true;
				}
			}
			if (IsChanged)
			{
				if (OldFriendCount != Settings.GetFriendCount())
				{
					Root.RebuildMainPage();
				}
				Root.OnChangeSettings();
			}
		}
        */
	}
}


