﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using keep.grass.Helpers;
using RuyiJinguBang;

namespace keep.grass
{
    public class AlphaSettingsPage : ResponsiveContentPage
    {
        AlphaApp Root = AlphaFactory.MakeSureApp();
        Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
        AlphaDomain Domain = AlphaFactory.MakeSureDomain();

        AlphaCircleImageCell UserLabel = AlphaFactory.MakeCircleImageCell();
        AlphaTheme OldTheme;

        public AlphaSettingsPage()
        {
            Title = L["Settings"];

            UserLabel.Command = new Command
            (
                o => AlphaFactory
                    .MakeSureApp()
                    .ShowSelectUserPage
                    (
                        NewUser =>
                        {
                            Settings.UserName = NewUser;
                            Domain.UpdateLastPublicActivityCoreAsync(NewUser).LeavingThrown();
                            Root.OnChangeSettings();
                            ApplyUser(Settings.UserName);
                        }
                    )
            );
        }

        public void ApplyUser(string User)
        {
            if (!String.IsNullOrWhiteSpace(User))
            {
                if (UserLabel.Text != User || null == UserLabel.ImageSource)
                {
                    AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(User))
                        .ContinueWith(t => Device.BeginInvokeOnMainThread(() => UserLabel.ImageSource = t.Result));
                    UserLabel.Text = User;
                    //UserLabel.TextColor = Color.Default;
                }
            }
            else
            {
                UserLabel.ImageSource = null;
                UserLabel.Text = L["unspecified"];
            }
            ApplyUserLabelTheme(User);
        }
        public void ApplyUserLabelTheme(string User)
        {
            var UserLabelAnimation = "DefaultButtn";
            if (Content.AnimationIsRunning(UserLabelAnimation))
            {
                Content.AbortAnimation(UserLabelAnimation);
            }
            var Theme = AlphaTheme.Get();
            if (!String.IsNullOrWhiteSpace(User))
            {
                AlphaTheme.Apply(UserLabel, Theme);
            }
            else
            {
                Content.Animate
                (
                    UserLabelAnimation,
                    d =>
                    {
                        var Rate = Math.Abs(Math.Sin(d));
                        UserLabel.TextColor = ColorEx.MergeWithRate(Theme.AccentColor, Theme.BackgroundColor, Rate);
                        UserLabel.BackgroundColor = ColorEx.MergeWithRate(Theme.BackgroundColor, Theme.AccentColor, Rate);
                    },
                    0.0,
                    1000.0,
                    16,
                    (uint)2000000,
                    Easing.Linear
                );
            }
        }

        public override void Build()
        {
            base.Build();
            Debug.WriteLine("AlphaSettingsPage.Rebuild();");

            var Friends = AlphaFactory.MakeCircleImageCell
            (
                Text: L["Rivals"] /*+string.Format("({0})", Settings.GetFriendCount())*/,
                Command: new Command(o => Root.Navigation.PushAsync(new AlphaFriendsPage()))
            );

            var ThemeCell = AlphaFactory.MakePickerCell();
            //ThemeCell.Items.Clear(); ２回目でこける。 Xamarin.Forms さん、もっと頑張って。。。
            foreach (var i in AlphaTheme.All.Keys)
            {
                if (!ThemeCell.Items.Where(j => j == i).Any())
                {
                    ThemeCell.Items.Add(L[i]);
                }
            }
            OldTheme = AlphaTheme.Get();
            ThemeCell.SelectedIndex = AlphaTheme.All.Values
                .IndexOf(OldTheme);
            ThemeCell.Picker.Unfocused += (sender, e) => 
            {
                var ThemeName = AlphaTheme.All.Keys.ElementAt(ThemeCell.SelectedIndex);
                AlphaTheme.Set(ThemeName);
                var NewTheme = AlphaTheme.Get();
                if (OldTheme != AlphaTheme.Get())
                {
                    OldTheme = NewTheme;
                    AlphaTheme.Apply(Root.Navigation);
                    AlphaTheme.Apply(Root.Main);
                    Build();
                    Analytics.TrackEvent(
                        name: "[Changed] Theme",
                        properties: new Dictionary<string, string> { { "Category", "Settings" }, { "Theme", ThemeName } }
                    );
                }
            };

            var LanguageCell = AlphaFactory.MakePickerCell();
            var Language = Settings.Language ?? "";
            //LanguageCell.Items.Clear(); ２回目でこける。 Xamarin.Forms さん、もっと頑張って。。。
            foreach (var i in L.DisplayNames.Select(i => i.Value))
            {
                if (!LanguageCell.Items.Where(j => j == L[i]).Any())
                {
                    LanguageCell.Items.Add(L[i]);
                }
            }
            LanguageCell.SelectedIndex = L.DisplayNames
                .Select(i => i.Key)
                .IndexOf(Language);
            LanguageCell.Picker.Unfocused += (sender, e) =>
            {
                var OldLanguage = L.Get();
                Settings.Language = L.DisplayNames.Keys.ElementAt(LanguageCell.SelectedIndex);
                if (OldLanguage != L.Get())
                {
                    L.Update();
                    Root.OnChangeSettings();
                    Root.RebuildMainPage();
                    Build();
                    Analytics.TrackEvent(
                        name: "[Changed] Language",
                        properties: new Dictionary<string, string>
                        {
                            { "Category", "Settings" },
                            { "Language", string.IsNullOrEmpty(Settings.Language) ? "default": Settings.Language }
                        }
                    );
                }
            };

            if (Width <= Height)
            {
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
                                    UserLabel,
                                    Friends,
                                },
                                new TableSection(L["Notifications"])
                                {
                                    AlphaFactory.MakeCircleImageCell
                                    (
                                        Text: L["Alert by Left Time"],
                                        Command: new Command(o => Root.Navigation.PushAsync(new AlphaLeftTimeSettingsPage()))
                                    ),
                                    AlphaFactory.MakeCircleImageCell
                                    (
                                        Text: L["Daily Alert"],
                                        Command: new Command(o => Root.Navigation.PushAsync(new AlphaDailyAlertSettingsPage()))
                                    )
                                },
                                new TableSection(L["Theme"])
                                {
                                    ThemeCell
                                },
                                new TableSection(L["Language"])
                                {
                                    LanguageCell
                                },
                                new TableSection(L["Information"])
                                {
                                    AlphaFactory.MakeCircleImageCell
                                    (
                                        ImageSource: Root.GetApplicationImageSource(),
                                        Text: L["keep.grass"],
                                        Command: new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage()))
                                    ),
                                },
                            },
                        },
                    },
                };
            }
            else
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 1.0,
                            BackgroundColor = Color.Gray,
                            Children =
                            {
                                new TableView
                                {
                                    BackgroundColor = Color.White,
                                    Root = new TableRoot
                                    {
                                        new TableSection(L["Github Account"])
                                        {
                                            UserLabel,
                                            Friends,
                                        },
                                        new TableSection(L["Theme"])
                                        {
                                            ThemeCell
                                        },
                                        new TableSection(L["Language"])
                                        {
                                            LanguageCell
                                        },
                                    },
                                },
                                new TableView
                                {
                                    BackgroundColor = Color.White,
                                    Root = new TableRoot
                                    {
                                        new TableSection(L["Notifications"])
                                        {
                                            AlphaFactory.MakeCircleImageCell
                                            (
                                                Text: L["Alert by Left Time"],
                                                Command: new Command(o => Root.Navigation.PushAsync(new AlphaLeftTimeSettingsPage()))
                                               ),
                                            AlphaFactory.MakeCircleImageCell
                                            (
                                                Text: L["Daily Alert"],
                                                Command: new Command(o => Root.Navigation.PushAsync(new AlphaDailyAlertSettingsPage()))
                                            )
                                        },
                                        new TableSection(L["Information"])
                                        {
                                            AlphaFactory.MakeCircleImageCell
                                            (
                                                ImageSource: Root.GetApplicationImageSource(),
                                                Text: L["keep.grass"],
                                                Command: new Command(o => Root.Navigation.PushAsync(AlphaFactory.MakeInfoPage()))
                                            ),
                                        },
                                    },
                                },
                            },
                        },
                    },
                };
            }
            AlphaTheme.Apply(this);
            ApplyUser(Settings.UserName);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}


