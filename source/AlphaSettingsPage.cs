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
		Cell

		public AlphaSettingsPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			L = Root.L;
			Title = L["Settings"];
			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection(L["Github Account"])
							{
								(
									UserNameCell = new EntryCell
									{
										Label = L["User ID"],
									}
								),
							},
							new TableSection(L["Notifications"])
							{
								(
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
									.ToArray()
								)
								.Select(i => i.Value)
							},
							/*
							new TableSection(L["Language"])
							{
								new EntryCell
								{
								}
							}
							*/
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
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Settings.UserName = UserNameCell.Text;
			foreach(var cell in AlertSwitchCellList)
			{
				Settings.SetAlert(cell.Key, cell.Value.On);
			}
			Root.OnChangeSettings();
			Root.ShowSettingsButtonOnToolbar();
		}
	}
}


