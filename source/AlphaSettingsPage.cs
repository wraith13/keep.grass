using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;
using System.Diagnostics;

namespace keep.grass
{
	public class AlphaSettingsPage : ResponsiveContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

        VoidEntryCell UserNameCell = null;
		AlphaPickerCell LanguageCell = null;

		public AlphaSettingsPage()
		{
			Title = L["Settings"];
			UserNameCell = AlphaFactory.MakeEntryCell();
			UserNameCell.Label = L["User ID"];
			LanguageCell = AlphaFactory.MakePickerCell();
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaSettingsPage.Rebuild();");

			var Friends = AlphaFactory.MakeCircleImageCell
			(
				Text: L["Rivals"] /*+string.Format("({0})", Settings.GetFriendCount())*/,
				Command: new Command(o => Root.Navigation.PushAsync(new AlphaFriendsPage()))
			);
			
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
								new TableSection(L["Github Account"])
								{
									UserNameCell.AsCell(),
									Friends,
								},
								new TableSection(L["Notifications"])
								{
									AlphaFactory.MakeCircleImageCell
									(
										Text: L["Alert by Left Time"],
										Command: new Command(o => Root.Navigation.PushAsync(new AlphaLeftTimeSettingsPage()))
									),
									AlphaFactory.MakeCircleImageCell
									(
										Text: L["Daily Alert"],
										Command: new Command(o => Root.Navigation.PushAsync(new AlphaDailyAlertSettingsPage()))
									)
								},
								new TableSection(L["Language"])
								{
									LanguageCell
								},
								new TableSection(L["Information"])
								{
									AlphaFactory.MakeCircleImageCell
									(
										ImageSource: Root.GetApplicationImageSource(),
										Text: L["keep.grass"],
										Command: new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage()))
									),
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
										new TableSection(L["Github Account"])
										{
											UserNameCell.AsCell(),
											Friends,
										},
										new TableSection(L["Language"])
										{
											LanguageCell
										},
									},
								},
								new TableView
								{
									BackgroundColor = Color.White,
									Root = new TableRoot
									{
										new TableSection(L["Notifications"])
										{
											AlphaFactory.MakeCircleImageCell
											(
												Text: L["Alert by Left Time"],
												Command: new Command(o => Root.Navigation.PushAsync(new AlphaLeftTimeSettingsPage()))
											   ),
											AlphaFactory.MakeCircleImageCell
											(
												Text: L["Daily Alert"],
												Command: new Command(o => Root.Navigation.PushAsync(new AlphaDailyAlertSettingsPage()))
											)
										},
										new TableSection(L["Information"])
										{
											AlphaFactory.MakeCircleImageCell
											(
												ImageSource: Root.GetApplicationImageSource(),
												Text: L["keep.grass"],
												Command: new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage()))
											),
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

			UserNameCell.Text = Settings.UserName;

			var Language = Settings.Language ?? "";
			//LanguageCell.Items.Clear(); ２回目でこける。 Xamarin.Forms さん、もっと頑張って。。。
			foreach (var i in L.DisplayNames.Select(i => i.Value))
			{
				if (!LanguageCell.Items.Where(j => j == i).Any())
				{
					LanguageCell.Items.Add(i);
				}
			}
			LanguageCell.SelectedIndex = L.DisplayNames
				.Select(i => i.Key)
				.IndexOf(Language);
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
			var OldLanguage = L.Get();
			Settings.Language = L.DisplayNames.Keys.ElementAt(LanguageCell.SelectedIndex);
			if (OldLanguage != L.Get())
			{
				L.Update();
				Root.RebuildMainPage();
                IsChanged = true;
            }
            if (IsChanged)
            {
                Root.OnChangeSettings();
            }
        }
	}
}


