using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaDailyAlertSettingsPage : ContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		KeyValuePair<TimeSpan, VoidSwitchCell>[] DailyAlertSwitchCellList = null;

		public AlphaDailyAlertSettingsPage()
		{
			Title = L["Notifications"];
			DailyAlertSwitchCellList = Settings.AlertDailyTimeTable.Select
			(
				i => new KeyValuePair<TimeSpan, VoidSwitchCell>
				(
					i,
                    AlphaFactory.MakeSwitchCell
                    (
                        Text: Settings.AlertDailyTimeToDisplayName(L, i),
                        On: Settings.GetDailyAlert(i)
                    )
				)
			)
			.ToArray();

			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection(L["Daily Alert"])
							{
								DailyAlertSwitchCellList
								.Select(i => i.Value.AsCell())
							},
						}
					},
				},
			};
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();

			foreach (var cell in DailyAlertSwitchCellList)
			{
				cell.Value.On = Settings.GetDailyAlert(cell.Key);
			}
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			foreach (var cell in DailyAlertSwitchCellList)
			{
				Settings.SetDailyAlert(cell.Key, cell.Value.On);
			}
			Root.OnChangeSettings();
		}
	}
}


