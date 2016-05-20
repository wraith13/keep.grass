using System;

namespace keep.grass
{
	public class Grass
	{
		static private string AtomUrlFormat = "https://github.com/{0}.atom";
		static public string AtomUrl(string Id)
		{
			return String.Format (AtomUrlFormat, Id);
		}
		static public DateTime GetLastPublicActivity(string Id)
		{
		}
	}
}

