using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
    public class OmegaSettingsPage : ContentPage
    {
        AlphaApp Root;
        public Languages.AlphaLanguage L;
        EntryCell UserNameCell = null;
        KeyValuePair<TimeSpan, SwitchCell>[] AlertSwitchCellList = null;
        AlphaPickerCell LanguageCell = null;

        public OmegaSettingsPage(AlphaApp AppRoot)
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

            var Information = AlphaFactory.MakeCircleImageCell();
            Information.ImageSource = Root.GetApplicationImageSource();
            Information.Text = L["keep.grass"];
            Information.Command = new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage(Root)));

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
            foreach (var cell in AlertSwitchCellList)
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
            Settings.UserName = UserNameCell.Text.Trim();
            foreach (var cell in AlertSwitchCellList)
            {
                Settings.SetAlert(cell.Key, cell.Value.On);
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


