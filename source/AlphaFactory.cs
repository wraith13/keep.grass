using System;
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

		public static App makeApp()
		{
			return instance.makeCustomApp();
		}

		public virtual App makeCustomApp()
		{
			return new App();
		}
	}
}

