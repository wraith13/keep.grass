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
		VoidEntryCell[] FriendNameCellList = null;

		public AlphaFriendsPage()
		{
			Title = L["Rivals"];
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
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaSettingsPage.Rebuild();");

			if (Width <= Height || FriendNameCellList.Count() < 6)
			{
				Content = new StackLayout
				{
					Children =
					{
						new TableView
						{
							Root = new TableRoot
							{
								new TableSection(L["Github Account"])
								{
									FriendNameCellList.Select(i => i.AsCell()),
								},
							},
						},
					},
				};
			}
			else
			{
				var HalfCount = FriendNameCellList.Count() /2;
				Content = new StackLayout
				{
					Children =
					{
						new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Spacing = 1.0,
							BackgroundColor = Color.Gray,
							Children =
							{
								new TableView
								{
									BackgroundColor = Color.White,
									Root = new TableRoot
									{
										new TableSection(L["Github Account"])
										{
											FriendNameCellList.Take(HalfCount).Select(i => i.AsCell()),
										},
									},
								},
								new TableView
								{
									BackgroundColor = Color.White,
									Root = new TableRoot
									{
										new TableSection(L["Github Account"])
										{
											FriendNameCellList.Skip(HalfCount).Select(i => i.AsCell()),
										},
									},
								},
							},
						},
					},
				};
			}
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();

			for (var i = 0; i < FriendNameCellList.Count(); ++i)
			{
				FriendNameCellList[i].Text = Settings.GetFriend(i);
			}
		}
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
	}
}


