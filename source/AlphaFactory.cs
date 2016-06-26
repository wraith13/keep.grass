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
			return instance.MakeCustomApp();
		}
		public virtual AlphaApp MakeCustomApp()
		{
			return new AlphaApp();
		}

		public static Languages.AlphaLanguage MakeLanguage(AlphaApp Root)
		{
			return instance.MakeCustomLanguage(Root);
		}
		public virtual Languages.AlphaLanguage MakeCustomLanguage(AlphaApp Root)
		{
			return new Languages.AlphaLanguage(Root);
		}

		public static AlphaMainPage MakeMainPage(AlphaApp Root)
		{
			return instance.MakeCustomMainPage(Root);
		}
		public virtual AlphaMainPage MakeCustomMainPage(AlphaApp Root)
		{
			return new AlphaMainPage(Root);
		}

		public static AlphaSettingsPage MakeSettingsPage(AlphaApp Root)
		{
			return instance.MakeCustomSettingsPage(Root);
		}
		public virtual AlphaSettingsPage MakeCustomSettingsPage(AlphaApp Root)
		{
			return new AlphaSettingsPage(Root);
		}

		public static AlphaActivityIndicatorTextCell MakeActivityIndicatorTextCell()
		{
			return instance.MakeCustomActivityIndicatorTextCell();
		}
		public virtual AlphaActivityIndicatorTextCell MakeCustomActivityIndicatorTextCell()
		{
			return new AlphaActivityIndicatorTextCell();
		}

		public static AlphaCircleImageCell MakeCircleImageCell()
		{
			return instance.MakeCustomCircleImageCell();
		}
		public virtual AlphaCircleImageCell MakeCustomCircleImageCell()
		{
			return new AlphaCircleImageCell();
		}

		public static AlphaPickerCell MakeAlphaPickerCell()
		{
			return instance.MakeCustomAlphaPickerCell();
		}
		public virtual AlphaPickerCell MakeCustomAlphaPickerCell()
		{
			return new AlphaPickerCell();
		}
	}
}

