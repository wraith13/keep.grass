using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace keep.grass
{
	public class GitHub
	{
		static private string AtomUrlFormat = "https://github.com/{0}.atom";
		static public string GetAtomUrl(string Id)
		{
			return String.Format (AtomUrlFormat, Id);
		}

		static private string IconUrlFormat = "https://github.com/{0}.png";
		static public string GetIconUrl(string Id)
		{
			return String.Format (IconUrlFormat, Id);
		}

		static public async Task<DateTime> GetLastPublicActivityAsync(string Id)
		{
			return await Task.Factory.StartNew<DateTime>
			(
				() =>
				DateTime.Parse
				(
					XDocument
					.Load(GetAtomUrl(Id))
					.Descendants()
					.Where(i => i.Name.LocalName == "updated")
					.First()
					.Value
				)
			);
		}
	}
}

