using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;
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
        AlphaPickerCell ThemeCell = null;
        AlphaPickerCell LanguageCell = null;
        AlphaTheme OldTheme;

        public AlphaSettingsPage()
        {
            Title = L["Settings"];

            ThemeCell = AlphaFactory.MakePickerCell();
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
                AlphaTheme.Set(AlphaTheme.All.Keys.ElementAt(ThemeCell.SelectedIndex));
                var NewTheme = AlphaTheme.Get();
                if (OldTheme != AlphaTheme.Get())
                {
                    OldTheme = NewTheme;
                    AlphaTheme.Apply(Root.Navigation);
                    AlphaTheme.Apply(Root.Main);
                    Build();
                }
            };

            LanguageCell = AlphaFactory.MakePickerCell();

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
            var Theme = AlphaTheme.Get();
            if (!String.IsNullOrWhiteSpace(User))
            {
                AlphaTheme.Apply(UserLabel, Theme);
            }
            else
            {
                UserLabel.TextColor = Theme.BackgroundColor;
                UserLabel.BackgroundColor = Theme.AccentColor;
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
            ApplyUserLabelTheme(Settings.UserName);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            ApplyUser(Settings.UserName);

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
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            bool IsChanged = false;
            var OldLanguage = L.Get();
            Settings.Language = L.DisplayNames.Keys.ElementAt(LanguageCell.SelectedIndex);
            if (OldLanguage != L.Get())
            {
                L.Update();
                IsChanged = true;
            }

            if (IsChanged)
            {
                Root.RebuildMainPage();
                Root.OnChangeSettings();
            }
        }
    }
}


