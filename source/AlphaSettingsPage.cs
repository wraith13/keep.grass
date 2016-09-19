using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaSettingsPage : ContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

        VoidEntryCell UserNameCell = null;
		KeyValuePair<TimeSpan, SwitchCell>[] LeftTimeAlertSwitchCellList = null;
		KeyValuePair<TimeSpan, SwitchCell>[] DailyAlertSwitchCellList = null;
		AlphaPickerCell LanguageCell = null;

		public AlphaSettingsPage()
		{
			Title = L["Settings"];
            UserNameCell = AlphaFactory.MakeEntryCell();
            UserNameCell.Label = L["User ID"];
			LeftTimeAlertSwitchCellList = Settings.AlertTimeSpanTable.Select
			(
				i => new KeyValuePair<TimeSpan, SwitchCell>
				(
					i,
					new SwitchCell
					{
						Text = Settings.AlertTimeSpanToDisplayName(L, i),
						On = Settings.GetAlert(i),
					}
				)
			)
			.ToArray();
			DailyAlertSwitchCellList = Settings.AlertDailyTimeTable.Select
			(
				i => new KeyValuePair<TimeSpan, SwitchCell>
				(
					i,
					new SwitchCell
					{
						Text = Settings.AlertDailyTimeToDisplayName(L, i),
						On = Settings.GetAlert(i),
					}
				)
			)
			.ToArray();
			LanguageCell = AlphaFactory.MakePickerCell();

			var Information = AlphaFactory.MakeCircleImageCell();
			Information.ImageSource = Root.GetApplicationImageSource();
			Information.Text = L["keep.grass"];
			Information.Command = new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage()));

			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection(L["Github Account"])
							{
								UserNameCell.AsCell(),
							},
							new TableSection(L["Notifications"])
							{
								new []
								{
									LeftTimeAlertSwitchCellList,
									DailyAlertSwitchCellList
								}
								.SelectMany(i => i)
								.Select(i => i.Value)
							},
							new TableSection(L["Language"])
							{
								LanguageCell
							},
							new TableSection(L["Information"])
							{
								Information
							}
						}
					},
				},
			};
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();

			UserNameCell.Text = Settings.UserName;
			foreach(var cell in LeftTimeAlertSwitchCellList)
			{
				cell.Value.On = Settings.GetAlert(cell.Key);
			}
			foreach (var cell in DailyAlertSwitchCellList)
			{
				cell.Value.On = Settings.GetDailyAlert(cell.Key);
			}

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
			var NewUserName = UserNameCell.Text.Trim();
			if (Settings.UserName != NewUserName)
			{
				Settings.UserName = NewUserName;
				Settings.IsValidUserName = false;
			}
			foreach(var cell in LeftTimeAlertSwitchCellList)
			{
				Settings.SetAlert(cell.Key, cell.Value.On);
			}
			foreach (var cell in DailyAlertSwitchCellList)
			{
				Settings.SetDailyAlert(cell.Key, cell.Value.On);
			}
			var OldLanguage = L.Get();
			Settings.Language = L.DisplayNames.Keys.ElementAt(LanguageCell.SelectedIndex);
			if (OldLanguage != L.Get())
			{
				L.Update();
				Root.RebuildMainPage();
			}
			Root.OnChangeSettings();
		}
	}
}


