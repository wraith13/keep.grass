using keep.grass.Helpers;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace keep.grass.UWP
{
    class OmegaDomain : AlphaDomain
    {
        public override void UpdateAlerts(DateTime LastPublicActivity)
        {
            base.UpdateAlerts(LastPublicActivity);

            if
            (
                default(DateTime) == LastPublicActivity
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
                                Text = L["Last Stamp"] +": " +LastPublicActivity.ToString("yyyy-MM-dd HH:mm"),
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
                        ExpirationTime = LastPublicActivity.AddHours(24),
                    }
                );

                Debug.Write("BackgroundUpdateLastPublicActivityTask: " + typeof(BackgroundUpdateLastPublicActivityTask).FullName);
                var BackgroundUpdateTaskName = "UpdateLastPublicActivity";
                foreach(var task in BackgroundTaskRegistration.AllTasks.Where(i => i.Value.Name == BackgroundUpdateTaskName))
                {
                    task.Value.Unregister(true);
                }
                Task.Run
                (
                    async () => await BackgroundExecutionManager.RequestAccessAsync()
                )
                .ContinueWith
                (
                    t =>
                    {
                        var builder = new BackgroundTaskBuilder();
                        builder.Name = BackgroundUpdateTaskName;
                        builder.TaskEntryPoint = typeof(BackgroundUpdateLastPublicActivityTask).FullName;
                        builder.SetTrigger(new TimeTrigger(15, false));
                        builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                        builder.Register();
                    }
                );

                var BackgroundUpdateTaskName2 = "UpdateLastPublicActivity2";
                foreach (var task in BackgroundTaskRegistration.AllTasks.Where(i => i.Value.Name == BackgroundUpdateTaskName2))
                {
                    task.Value.Unregister(true);
                }
                Task.Run
                (
                    async () => await BackgroundExecutionManager.RequestAccessAsync()
                )
                .ContinueWith
                (
                    t =>
                    {
                        var builder = new BackgroundTaskBuilder();
                        builder.Name = BackgroundUpdateTaskName2;
                        builder.TaskEntryPoint = typeof(BackgroundUpdateLastPublicActivityTask).FullName;
                        builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
                        builder.Register();
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
        public override void CancelAllAlerts()
        {
            ToastNotificationManager.History.Clear();
            var Manager = ToastNotificationManager
                .CreateToastNotifier();
            foreach (var toast in Manager
                .GetScheduledToastNotifications())
            {
                Manager.RemoveFromSchedule(toast);
            }
        }

        public override Uri GetApplicationStoreUri()
        {
            return new Uri("https://www.microsoft.com/store/apps/9nblggh51p1m");
        }
    }
}
