using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaSettingsPage : ContentPage
	{
		AlphaApp Root;
		public Languages.AlphaLanguage L;
		EntryCell UserNameCell = null;
		KeyValuePair<TimeSpan, SwitchCell>[] AlertSwitchCellList = null;
		AlphaPickerCell LanguageCell = null;

		public AlphaSettingsPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			L = Root.L;
			Title = L["Settings"];
			UserNameCell = new EntryCell
			{
				Label = L["User ID"],
			};
			AlertSwitchCellList = Settings.AlertTimeSpanTable.Select
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
			LanguageCell = AlphaFactory.MakePickerCell();
			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection(L["Github Account"])
							{
								UserNameCell,
							},
							new TableSection(L["Notifications"])
							{
								AlertSwitchCellList.Select(i => i.Value)
							},
							new TableSection(L["Language"])
							{
								LanguageCell
							},
							new TableSection(L["Information"])
							{
								new TextCell
								{
									Text = L["keep.grass"],
									Command = new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage(Root))),
								}
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
			foreach(var cell in AlertSwitchCellList)
			{
				cell.Value.On = Settings.GetAlert(cell.Key);
			}

			var Language = Settings.Language ?? "";
			foreach (var i in L.DisplayNames.Select(i => i.Value))
			{
				LanguageCell.Items.Add(i);
			}
			LanguageCell.SelectedIndex = L.DisplayNames
				.Select(i => i.Key)
				.IndexOf(Language);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Settings.UserName = UserNameCell.Text;
			foreach(var cell in AlertSwitchCellList)
			{
				Settings.SetAlert(cell.Key, cell.Value.On);
			}
			Settings.Language = L.DisplayNames.Keys.ElementAt(LanguageCell.SelectedIndex);

			Root.OnChangeSettings();
			Root.ShowSettingsButtonOnToolbar();
		}
	}
}


