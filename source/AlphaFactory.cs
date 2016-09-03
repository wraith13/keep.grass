using System;
using Xamarin.Forms;

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

		public static AlphaDomain MakeDomain()
		{
			return Instance.Domain ??
           		(Instance.Domain = Instance.MakeOmegaDomain());
		}
		public virtual AlphaDomain MakeOmegaDomain()
		{
			return new AlphaDomain();
		}

		public static AlphaApp MakeApp()
		{
			return Instance.App ??
           		(Instance.App = Instance.MakeOmegaApp());
		}
		public abstract AlphaApp MakeOmegaApp();

		public static Languages.AlphaLanguage MakeLanguage()
		{
			return Instance.Language ??
           		(Instance.Language = Instance.MakeOmegaLanguage());
		}
		public virtual Languages.AlphaLanguage MakeOmegaLanguage()
		{
			return new Languages.AlphaLanguage();
		}

		public static AlphaMainPage MakeMainPage(AlphaApp Root)
		{
			return Instance.MakeOmegaMainPage(Root);
		}
		public virtual AlphaMainPage MakeOmegaMainPage(AlphaApp Root)
		{
			return new AlphaMainPage(Root);
		}

		public static ContentPage MakeSettingsPage(AlphaApp Root)
		{
			return Instance.MakeOmegaSettingsPage(Root);
		}
		public virtual ContentPage MakeOmegaSettingsPage(AlphaApp Root)
		{
			return new AlphaSettingsPage(Root);
		}

		public static AlphaInfoPage MakeInfoPage(AlphaApp Root)
		{
			return Instance.MakeOmegaInfoPage(Root);
		}
		public virtual AlphaInfoPage MakeOmegaInfoPage(AlphaApp Root)
		{
			return new AlphaInfoPage(Root);
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
	}
}

