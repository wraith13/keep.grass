using System;
using Xamarin.Forms;

namespace keep.grass
{
	public abstract class AlphaFactory
	{
		static AlphaFactory instance = null;

		public AlphaFactory()
		{
		}
		protected static void Init(AlphaFactory app)
		{
			instance = app;
		}
		public static AlphaFactory Get()
		{
			return instance;
		}

		public static AlphaApp MakeApp()
		{
			return instance.MakeOmegaApp();
		}
		public abstract AlphaApp MakeOmegaApp();

		public static Languages.AlphaLanguage MakeLanguage(AlphaApp Root)
		{
			return instance.MakeOmegaLanguage();
		}
		public virtual Languages.AlphaLanguage MakeOmegaLanguage()
		{
			return new Languages.AlphaLanguage();
		}

		public static AlphaMainPage MakeMainPage(AlphaApp Root)
		{
			return instance.MakeOmegaMainPage(Root);
		}
		public virtual AlphaMainPage MakeOmegaMainPage(AlphaApp Root)
		{
			return new AlphaMainPage(Root);
		}

		public static ContentPage MakeSettingsPage(AlphaApp Root)
		{
			return instance.MakeOmegaSettingsPage(Root);
		}
		public virtual ContentPage MakeOmegaSettingsPage(AlphaApp Root)
		{
			return new AlphaSettingsPage(Root);
		}

		public static AlphaInfoPage MakeInfoPage(AlphaApp Root)
		{
			return instance.MakeOmegaInfoPage(Root);
		}
		public virtual AlphaInfoPage MakeOmegaInfoPage(AlphaApp Root)
		{
			return new AlphaInfoPage(Root);
		}

		public static AlphaActivityIndicatorTextCell MakeActivityIndicatorTextCell()
		{
			return instance.MakeOmegaActivityIndicatorTextCell();
		}
		public virtual AlphaActivityIndicatorTextCell MakeOmegaActivityIndicatorTextCell()
		{
			return new AlphaActivityIndicatorTextCell();
		}

		public static AlphaCircleImageCell MakeCircleImageCell()
		{
			return instance.MakeOmegaCircleImageCell();
		}
		public virtual AlphaCircleImageCell MakeOmegaCircleImageCell()
		{
			return new AlphaCircleImageCell();
		}

		public static AlphaPickerCell MakePickerCell()
		{
			return instance.MakeOmegaPickerCell();
		}
		public virtual AlphaPickerCell MakeOmegaPickerCell()
		{
			return new AlphaPickerCell();
		}
	}
}

