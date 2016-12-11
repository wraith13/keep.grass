using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ImageCircle.Forms.Plugin.UWP;

namespace keep.grass.UWP
{
    public sealed partial class MainPage
    {
        public static AlphaApp App;

        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(MakeSureApp());
        }

        static public AlphaApp MakeSureApp()
        {
            if (null == App)
            {
                ImageCircleRenderer.Init();
                OmegaFactory.MakeSureInit();
                App = keep.grass.AlphaFactory.MakeSureApp();
            }
            return App;
        }
    }

    public class BackgroundUpdateLastPublicActivityTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var Deferral = taskInstance.GetDeferral();
            OmegaFactory.MakeSureInit();
            AlphaFactory.MakeSureDomain()
                .BackgroundUpdateLastPublicActivityAsync()
                .ContinueWith
                (
                    t =>
                    {
                        Deferral.Complete();
                    }
                );
        }
    }
}
