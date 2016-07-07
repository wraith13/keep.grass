using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Windows.UI.Notifications;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;

using keep.grass.Helpers;

namespace keep.grass.UWP
{
    public class OmegaApp : AlphaApp
    {
        public OmegaApp()
        {
        }

        public override string getLanguage()
        {
            return Windows.System.UserProfile.GlobalizationPreferences.Languages[0].Split('-')[0];
        }

        public override void UpdateAlerts()
        {
            base.UpdateAlerts();

            if
            (
                String.IsNullOrWhiteSpace(Settings.UserName) ||
                null == Main.LastPublicActivity
            )
            {
            }
            else
            {
                var binding = new TileBinding()
                {
                    Branding = TileBranding.NameAndLogo,
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage
                        {
                            Source = "Assets/TileBackGround.png",
                        },
                        Children =
                        {
                            new TileText()
                            {
                                Text = Settings.UserName,
                                Style = TileTextStyle.Body,
                            },
                            new TileText()
                            {
                                Text = L["Last Stamp: "] + Main.LastPublicActivity.Value.ToString("yyyy-MM-dd HH:mm"),
                                Wrap = true,
                                Style = TileTextStyle.CaptionSubtle,
                            },
                        },
                    },
                };
                ;
                TileUpdateManager.CreateTileUpdaterForApplication().Update
                (
                    new TileNotification
                    (
                        new TileContent()
                        {
                            Visual = new TileVisual()
                            {
                                TileMedium = binding,
                                TileWide = binding,
                                TileLarge = binding
                            }
                        }
                        .GetXml()
                    )
                    {
                        ExpirationTime = Main.LastPublicActivity.Value.AddHours(24),
                    }
                );
            }
        }
        string MakeToastId(int id)
        {
            return "Toast" + id.ToString();
        }

        public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
        {
            //  こんなコードを自前で用意しない為に Plugin.LocalNotifications を導入したのだが。。。
            //  UWP にも対応してるハズなのになぜか動作してくれない。

            CancelAlert(id);

            ToastNotificationManager.CreateToastNotifier().AddToSchedule
            (
                new ScheduledToastNotification
                (
                    new ToastContent
                    {
                        Visual = new ToastVisual
                        {
                            TitleText = new ToastText()
                            {
                                Text = title,
                            },
                            BodyTextLine1 = new ToastText()
                            {
                                Text = body,
                            },
                        },
                    }
                    .GetXml(),
                    notifyTime
                )
                {
                    Id = MakeToastId(id),
                }
            );
        }
        public override void CancelAlert(int id)
        {
            var ToastId = MakeToastId(id);
            var Manager = ToastNotificationManager
                .CreateToastNotifier();
            foreach (var toast in Manager
                .GetScheduledToastNotifications()
                .Where(t => t.Id == ToastId))
            {
                Manager.RemoveFromSchedule(toast);
            }
        }
    }
}
