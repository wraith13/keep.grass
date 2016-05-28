using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class SettingsPage : ContentPage
	{
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
								new EntryCell
								{
									Label = "User Name",
									Text = "wraith13",
								},
							},
							new TableSection("Alerts")
							{
								new SwitchCell
								{
									Text = "Just 24 hours later",
								},
								new SwitchCell
								{
									Text = "5 minites left",
								},
								new SwitchCell
								{
									Text = "10 minites left",
								},
								new SwitchCell
								{
									Text = "15 minites left",
								},
								new SwitchCell
								{
									Text = "30 minites left",
								},
								new SwitchCell
								{
									Text = "45 minites left",
								},
								new SwitchCell
								{
									Text = "1 hour left",
								},
								new SwitchCell
								{
									Text = "2 hours left",
								},
								new SwitchCell
								{
									Text = "3 hours left",
								},
								new SwitchCell
								{
									Text = "6 hours left",
								},
								new SwitchCell
								{
									Text = "9 hours left",
								},
								new SwitchCell
								{
									Text = "12 hours left",
								},
								new SwitchCell
								{
									Text = "18 hours left",
								},
							},
						}
					},
				},
			};
		}
	}
}


