using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;

namespace keep.grass.App
{
    public class AlphaLeftTimeSettingsPage : ResponsiveContentPage
    {
        AlphaApp Root = AlphaAppFactory.MakeSureApp();
        AlphaLanguage L = AlphaDomainFactory.MakeSureLanguage();

        KeyValuePair<TimeSpan, VoidSwitchCell>[] LeftTimeAlertSwitchCellList = null;

        public AlphaLeftTimeSettingsPage()
        {
            Title = L["Alert by Left Time"];
            LeftTimeAlertSwitchCellList = Settings.AlertTimeSpanTable.Select
            (
                i => new KeyValuePair<TimeSpan, VoidSwitchCell>
                (
                    i,
                    AlphaAppFactory.MakeSwitchCell
                    (
                        Text: Settings.AlertTimeSpanToDisplayName(L, i),
                        On: Settings.GetAlert(i)
                    )
                )
            )
            .ToArray();

        }
        public IList<View> GetColumns(int ColumnCount)
        {
            var result = new List<View>();
            var ElementPerColumn = LeftTimeAlertSwitchCellList.Count() / ColumnCount;
            result.AddRange
            (
                Enumerable.Range(0, ColumnCount).Select
                (
                    index => new TableView
                    {
                        BackgroundColor = Color.White,
                        Root = new TableRoot
                        {
                            new TableSection()
                            {
                                LeftTimeAlertSwitchCellList
                                    .Skip(ElementPerColumn *index)
                                    .Take(ElementPerColumn)
                                    .Select(i => i.Value.AsCell())
                            },
                        }
                    }
                )
            );
            return result;
        }
        public override void Build()
        {
            base.Build();
            Debug.WriteLine("AlphaLeftTimeSettingsPage.Rebuild();");

            var StackContent = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 1.0,
                BackgroundColor = Color.Gray,
            };
            foreach (var i in GetColumns((Width <= Height && Width < 640) ? 1 : 2))
            {
                StackContent.Children.Add(i);
            }
            Content = StackContent;
            AlphaThemeStatic.Apply(this);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            foreach (var cell in LeftTimeAlertSwitchCellList)
            {
                cell.Value.On = Settings.GetAlert(cell.Key);
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            bool IsChanged = false;
            foreach (var cell in LeftTimeAlertSwitchCellList)
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


