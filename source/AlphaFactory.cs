using System;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace keep.grass
{
	public abstract class AlphaFactory
	{
		static AlphaFactory Instance = null;
		AlphaDomain Domain = null;
		AlphaApp App = null;
		Languages.AlphaLanguage Language = null;

		public AlphaFactory()
		{
		}
		protected static void Init(AlphaFactory Factory)
		{
			Instance = Factory;
		}
		public static AlphaFactory Get()
		{
			return Instance;
		}

		public static AlphaDomain MakeSureDomain()
		{
			return Instance.Domain ??
           		(Instance.Domain = Instance.MakeOmegaDomain());
		}
		public abstract AlphaDomain MakeOmegaDomain();

		public static AlphaApp GetApp()
		{
			return Instance.App;
		}
		public static AlphaApp SetApp(AlphaApp app)
		{
			return Instance.App = app;
		}
		public static AlphaApp MakeSureApp()
		{
			return Instance.App ??
           		SetApp(Instance.MakeOmegaApp());
		}
		public abstract AlphaApp MakeOmegaApp();

		public static Languages.AlphaLanguage MakeSureLanguage()
		{
			return Instance.Language ??
           		(Instance.Language = Instance.MakeOmegaLanguage());
		}
		public virtual Languages.AlphaLanguage MakeOmegaLanguage()
		{
			return new Languages.AlphaLanguage();
		}

		public static AlphaMainPage MakeMainPage()
		{
			return Instance.MakeOmegaMainPage();
		}
		public virtual AlphaMainPage MakeOmegaMainPage()
		{
			return new AlphaMainPage();
		}

		public static ContentPage MakeSettingsPage()
		{
			return Instance.MakeOmegaSettingsPage();
		}
		public virtual ContentPage MakeOmegaSettingsPage()
		{
			return new AlphaSettingsPage();
		}

		public static AlphaInfoPage MakeInfoPage()
		{
			return Instance.MakeOmegaInfoPage();
		}
		public virtual AlphaInfoPage MakeOmegaInfoPage()
		{
			return new AlphaInfoPage();
		}

		public static AlphaCircleGraph MakeCircleGraph()
		{
			return Instance.MakeOmegaCircleGraph();
		}
		public virtual AlphaCircleGraph MakeOmegaCircleGraph()
		{
			return new AlphaCircleGraph();
		}

		public static AlphaActivityIndicatorButton MakeActivityIndicatorButton()
		{
			return Instance.MakeOmegaActivityIndicatorButton();
		}
		public virtual AlphaActivityIndicatorButton MakeOmegaActivityIndicatorButton()
		{
			return new AlphaActivityIndicatorButton();
		}

		public static AlphaActivityIndicatorTextCell MakeActivityIndicatorTextCell()
		{
			return Instance.MakeOmegaActivityIndicatorTextCell();
		}
		public virtual AlphaActivityIndicatorTextCell MakeOmegaActivityIndicatorTextCell()
		{
			return new AlphaActivityIndicatorTextCell();
		}

		public static AlphaCircleImageCell MakeCircleImageCell()
		{
			return Instance.MakeOmegaCircleImageCell();
		}
		public static AlphaCircleImageCell MakeCircleImageCell(ImageSource ImageSource, string Text, Command Command, ImageSource OptionImageSource)
		{
			var result = MakeCircleImageCell(ImageSource, Text, Command);
			result.OptionImageSource = OptionImageSource;
			return result;
		}
		public static AlphaCircleImageCell MakeCircleImageCell(ImageSource ImageSource, string Text, Command Command)
		{
			var result = MakeCircleImageCell(Text, Command);
			result.ImageSource = ImageSource;
			return result;
		}
		public static AlphaCircleImageCell MakeCircleImageCell(string Text, Command Command)
		{
			var result = MakeCircleImageCell();
			result.Text = Text;
			result.Command = Command;
			return result;
		}
		public virtual AlphaCircleImageCell MakeOmegaCircleImageCell()
		{
			return new AlphaCircleImageCell();
		}

		public static AlphaPickerCell MakePickerCell()
		{
			return Instance.MakeOmegaPickerCell();
		}
		public virtual AlphaPickerCell MakeOmegaPickerCell()
		{
			return new AlphaPickerCell();
		}

        public static Image MakeCircleImage()
        {
            return Instance.MakeOmegaCircleImage();
        }
		public virtual Image MakeOmegaCircleImage()
        {
            return new CircleImage();
        }
        public static VoidEntryCell MakeEntryCell()
        {
            return Instance.MakeOmegaEntryCell();
        }
        public virtual VoidEntryCell MakeOmegaEntryCell()
        {
            return new AlphaEntryCell();
        }
        public static VoidSwitchCell MakeSwitchCell()
        {
            return Instance.MakeOmegaSwitchCell();
        }
        public static VoidSwitchCell MakeSwitchCell(string Text, bool On)
        {
            var result = MakeSwitchCell();
            result.Text = Text;
            result.On = On;
            return result;
        }
        public virtual VoidSwitchCell MakeOmegaSwitchCell()
        {
            return new AlphaSwitchCell();
        }

        public static async Task<ImageSource> MakeImageSourceFromUrl(string Url)
        {
            return await Instance.MakeOmegaImageSourceFromUrl(Url);
        }
        public virtual async Task<ImageSource> MakeOmegaImageSourceFromUrl(string Url)
        {
            //return ImageSource.FromUri(new Uri(Url));
			//	↑こちらのコードでも良いが、効率化の為に SkiaSharp 用のバイナリと同じモノを使い回す
            try
            {
				var Binary = await AlphaImageProxy.Get(Url);
				return ImageSource.FromStream(() => new System.IO.MemoryStream(Binary));
			}
			catch (Exception err)
			{
				Debug.WriteLine(err);
				return null;
			}
		}
    }
}

