using System;
namespace keep.grass
{
	public class AlphaFactory
	{
		static AlphaFactory instance = null;

		public AlphaFactory()
		{
		}
		protected static void Init(AlphaFactory app)
		{
			instance = app;
		}
		public static void Init()
		{
			Init(new AlphaFactory());
		}
		public static AlphaFactory Get()
		{
			return instance;
		}

		public static AlphaApp MakeApp()
		{
			return instance.MakeOmegaApp();
		}
		public virtual AlphaApp MakeOmegaApp()
		{
			return new AlphaApp();
		}

		public static Languages.AlphaLanguage MakeLanguage(AlphaApp Root)
		{
			return instance.MakeOmegaLanguage(Root);
		}
		public virtual Languages.AlphaLanguage MakeOmegaLanguage(AlphaApp Root)
		{
			return new Languages.AlphaLanguage(Root);
		}

		public static AlphaMainPage MakeMainPage(AlphaApp Root)
		{
			return instance.MakeOmegaMainPage(Root);
		}
		public virtual AlphaMainPage MakeOmegaMainPage(AlphaApp Root)
		{
			return new AlphaMainPage(Root);
		}

		public static AlphaSettingsPage MakeSettingsPage(AlphaApp Root)
		{
			return instance.MakeOmegaSettingsPage(Root);
		}
		public virtual AlphaSettingsPage MakeOmegaSettingsPage(AlphaApp Root)
		{
			return new AlphaSettingsPage(Root);
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

