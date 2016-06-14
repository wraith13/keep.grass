﻿using System;
namespace keep.grass
{
	public class AlphaFactory
	{
		static AlphaFactory instance = null;

		public AlphaFactory()
		{
			instance = this;
		}
		public static AlphaFactory Get()
		{
			return instance;
		}

		public static AlphaApp makeApp()
		{
			return instance.makeCustomApp();
		}
		public virtual AlphaApp makeCustomApp()
		{
			return new AlphaApp();
		}

		public static AlphaMainPage makeMainPage(AlphaApp Root)
		{
			return instance.makeCustomMainPage(Root);
		}
		public virtual AlphaMainPage makeCustomMainPage(AlphaApp Root)
		{
			return new AlphaMainPage(Root);
		}

		public static AlphaSettingsPage makeSettingsPage(AlphaApp Root)
		{
			return instance.makeCustomSettingsPage(Root);
		}
		public virtual AlphaSettingsPage makeCustomSettingsPage(AlphaApp Root)
		{
			return new AlphaSettingsPage(Root);
		}

		public static AlphaActivityIndicatorTextCell makeActivityIndicatorTextCell()
		{
			return instance.makeCustomActivityIndicatorTextCell();
		}
		public virtual AlphaActivityIndicatorTextCell makeCustomActivityIndicatorTextCell()
		{
			return new AlphaActivityIndicatorTextCell();
		}

		public static AlphaCircleImageCell makeCircleImageCell()
		{
			return instance.makeCustomCircleImageCell();
		}
		public virtual AlphaCircleImageCell makeCustomCircleImageCell()
		{
			return new AlphaCircleImageCell();
		}
	}
}

