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

		const int MaxFriendCount = 5;
		VoidEntryCell[] FriendNameCellList = null;

		public AlphaFriendsPage()
		{
			Title = L["Friends"];
			FriendNameCellList = Enumerable.Range(0, MaxFriendCount)
         		.Select(i => AlphaFactory.MakeEntryCell())
             	.ToArray();
			//UserNameCell.Label = L["User ID"];
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaSettingsPage.Rebuild();");

			if (Width <= Height)
			{
				Content = new StackLayout
				{
					Children =
					{
						new TableView
						{
							Root = new TableRoot
							{
								new TableSection(L["Friends"])
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
										new TableSection(L["Friends"])
										{
											FriendNameCellList.Select(i => i.AsCell()),
										},
									},
								},
								new TableView
								{
									BackgroundColor = Color.White,
									Root = new TableRoot
									{
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

			UserNameCell.Text = Settings.UserName;
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			bool IsChanged = false;
			var NewUserName = UserNameCell.Text.Trim();
			if (Settings.UserName != NewUserName)
			{
				Settings.UserName = NewUserName;
				Settings.IsValidUserName = false;
				IsChanged = true;
			}
			if (IsChanged)
			{
				Root.OnChangeSettings();
			}
		}
	}
}


