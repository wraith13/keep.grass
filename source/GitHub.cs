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

				public static Link Parse(XElement Node)
				{
					var result = new Link();
					result.Type = Node.Attribute("type").Value;
					result.Rel = Node.Attribute("rel").Value;
					result.Href = Node.Attribute("href").Value;
					return result;
				}
			}
			public class Entry
			{
				public string Id { get; set; }
				public DateTime Published { get; set; }
				public DateTime Updated { get; set; }
				public IEnumerable<Link> LinkList { get; set; }
				public string Title { get; set; }

				public static Entry Parse(XElement Node)
				{
					var result = new Entry();
					result.Id = Node.LocalElement("id").Value;
					result.Published = DateTime.Parse(Node.LocalElement("published").Value);
					result.Updated = DateTime.Parse(Node.LocalElement("updated").Value);
					result.LinkList = Node.LocalElements("link").Select(i => Link.Parse(i)).ToList();
					result.Title = Node.LocalElement("title").Value;
					return result;
				}
			}

			public string Id { get; set; }
			public IEnumerable<Link> LinkList { get; set; }
			public string Title { get; set; }
			public DateTime Updated { get; set; }
			public IEnumerable<Entry> EntryList { get; set; }

			public static Feed Parse(XElement Root)
			{
				var result = new Feed();
				result.Id = Root.LocalElement("id").Value;
				result.Title = Root.LocalElement("title").Value;
				result.Updated = DateTime.Parse(Root.LocalElement("updated").Value);
				result.LinkList = Root.LocalElements("link").Select(i => Link.Parse(i)).ToList();
				result.EntryList = Root.LocalElements("entry").Select(i => Entry.Parse(i)).ToList();
				return result;
			}
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

		static public Feed ParseFeed(byte[] AtomRawBytes)
		{
			using (var stream = new MemoryStream(AtomRawBytes))
			{
				return Feed.Parse(XDocument.Load(stream).Root);
			}
		}
	}
}

