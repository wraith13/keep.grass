using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

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
			return DateTime.Parse
			(
				XDocument
					.Load(AtomUrl(Id))
					.Descendants()
					.Where(i => i.Name.LocalName == "updated")
					.First()
					.Value
			);
		}
	}
}

