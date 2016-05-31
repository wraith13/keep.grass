using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class SettingsPage : ContentPage
	{
		EntryCell UserNameCell = null;
		KeyValuePair<TimeSpan, SwitchCell>[] AlertSwitchCellList = null;

		TimeSpan[] TimeSpanTable = new []
		{
			TimeSpan.FromTicks(0),
			TimeSpan.FromMinutes(5),
			TimeSpan.FromMinutes(10),
			TimeSpan.FromMinutes(15),
			TimeSpan.FromMinutes(30),
			TimeSpan.FromMinutes(45),
			TimeSpan.FromHours(1),
			TimeSpan.FromHours(2),
			TimeSpan.FromHours(3),
			TimeSpan.FromHours(6),
			TimeSpan.FromHours(9),
			TimeSpan.FromHours(12),
			TimeSpan.FromHours(18),
		};

		public string TimeSpanToSwitchLabelString(TimeSpan left)
		{
			if (TimeSpan.FromHours(1) < left)
			{
				return String.Format("{0} hours left", left.TotalHours);
			}
			else
			if (TimeSpan.FromHours(1) == left)
			{
				return "1 hour left";
			}
			else
			if (TimeSpan.FromMinutes(1) < left)
			{
				return String.Format("{0} minutes left", left.TotalMinutes);
			}
			else
			if (TimeSpan.FromMinutes(1) == left)
			{
				return "1 minute left";
			}
			else
			{
				return "Just 24 hours later";
			}
		}
		public string TimeSpanToSettingKey(TimeSpan left)
		{
			return String.Format("alert{0}{1}", left.Hours, left.Minutes);
		}

		public SettingsPage()
		{
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
									AlertSwitchCellList = TimeSpanTable.Select
									(
										i => new KeyValuePair<TimeSpan, SwitchCell>
										(
											i,
											new SwitchCell
											{
												Text = TimeSpanToSwitchLabelString(i),
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
				cell.Value.On = Settings.get(TimeSpanToSettingKey(cell.Key), false);
			}
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Settings.UserName = UserNameCell.Text;
			foreach(var cell in AlertSwitchCellList)
			{
				Settings.set(TimeSpanToSettingKey(cell.Key), cell.Value.On);
			}
		}
	}
}


