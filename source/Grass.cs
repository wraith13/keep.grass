using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace keep.grass
{
	public class Grass
	{
		static private string AtomUrlFormat = "https://github.com/{0}.atom";
		static public string AtomUrl(string Id)
		{
			return String.Format (AtomUrlFormat, Id);
		}
		static public async Task<DateTime> GetLastPublicActivityAsync(string Id)
		{
			return await Task.Factory.StartNew<DateTime>
			(
				() =>
				DateTime.Parse
				(
					XDocument
					.Load(AtomUrl(Id))
					.Descendants()
					.Where(i => i.Name.LocalName == "updated")
					.First()
					.Value
				)
			);
		}
	}
}

