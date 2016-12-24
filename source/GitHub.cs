using System;
using System.Net.Http;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace keep.grass
{
	public class GitHub
	{
		public class Feed
		{
			public class Link
			{
				public string Type { get; set; }
				public string Rel { get; set; }
				public string Href { get; set; }
			}
			public class Entry
			{
				public string Id { get; set; }
				public DateTime Published { get; set; }
				public DateTime Updated { get; set; }
				public IEnumerable<Link> LinkList { get; set; }
				public string Title { get; set; }
			}

			public string Id { get; set; }
			public IEnumerable<Link> LinkList { get; set; }
			public string Title { get; set; }
			public DateTime Updated { get; set; }
			public IEnumerable<Entry> EntryList { get; set; }
		}

		static private string BaseUrl = "https://github.com";

		static private string ProfileUrlFormat = BaseUrl +"/{0}";
		static public string GetProfileUrl(string Id)
		{
			return String.Format(ProfileUrlFormat, Id);
		}

		static private string AcitivityUrlFormat = BaseUrl + "/{0}?tab=activity";
		static public string GetAcitivityUrl(string Id)
		{
			return String.Format(AcitivityUrlFormat, Id);
		}

		static private string AtomUrlFormat = BaseUrl + "/{0}.atom";
		static public string GetAtomUrl(string Id)
		{
			return String.Format (AtomUrlFormat, Id);
		}

		static private string IconUrlFormat = BaseUrl + "/{0}.png";
		static public string GetIconUrl(string Id)
		{
			return String.Format (IconUrlFormat, Id);
		}

		static public DateTime GetLastPublicActivity(byte[] AtomRawBytes)
		{
			using (var stream = new MemoryStream(AtomRawBytes))
			{
				return DateTime.Parse
				(
					XDocument
						.Load(stream)
						.Descendants()
						.Where(i => i.Name.LocalName == "updated")
						.First()
						.Value
				);
			}
		}
	}
}

