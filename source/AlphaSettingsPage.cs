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
		EntryCell UserNameCell = null;
		KeyValuePair<TimeSpan, SwitchCell>[] AlertSwitchCellList = null;

		public AlphaSettingsPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			Title = "Settings";
			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection("Github Account")
							{
								(
									UserNameCell = new EntryCell
									{
										Label = "User Name",
										Text = "",
									}
								),
							},
							new TableSection("Alerts")
							{
								(
									AlertSwitchCellList = Settings.AlertTimeSpanTable.Select
									(
										i => new KeyValuePair<TimeSpan, SwitchCell>
										(
											i,
											new SwitchCell
											{
												Text = Settings.AlertTimeSpanToDisplayName(i),
												On = Settings.GetAlert(i),
											}
										)
									)
									.ToArray()
								)
								.Select(i => i.Value)
							},
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


