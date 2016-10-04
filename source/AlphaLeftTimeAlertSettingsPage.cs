using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaLeftTimeSettingsPage : ContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		KeyValuePair<TimeSpan, VoidSwitchCell>[] LeftTimeAlertSwitchCellList = null;

		public AlphaLeftTimeSettingsPage()
		{
			Title = L["Alert by Left Time"];
			LeftTimeAlertSwitchCellList = Settings.AlertTimeSpanTable.Select
			(
				i => new KeyValuePair<TimeSpan, VoidSwitchCell>
				(
					i,
                    AlphaFactory.MakeSwitchCell
                    (
                        Text: Settings.AlertTimeSpanToDisplayName(L, i),
                        On: Settings.GetAlert(i)
                    )
				)
			)
			.ToArray();

			Content = new StackLayout
			{ 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection()
							{
								LeftTimeAlertSwitchCellList
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

			foreach(var cell in LeftTimeAlertSwitchCellList)
			{
				cell.Value.On = Settings.GetAlert(cell.Key);
			}
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			bool IsChanged = false;
			foreach(var cell in LeftTimeAlertSwitchCellList)
			{
				if (Settings.GetAlert(cell.Key) != cell.Value.On)
				{
					Settings.SetAlert(cell.Key, cell.Value.On);
					IsChanged = true;
				}
			}
			if (IsChanged)
			{
				Root.OnChangeSettings();
			}
		}
	}
}


