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
			public class Content
			{
				public string Type { get; set; }
				public string Value { get; set; }
				public string Svg
				{
					get
					{
						var start = Value.IndexOf("<svg");
						var end = Value.IndexOf("</svg>");
						if (0 <= start && 0 <= end)
						{
							var source = Value.Substring(start, end + "</svg>".Length - start);
							if (source.IndexOf("xmlns=\"http://www.w3.org/2000/svg\"") < 0)
							{
								source = source.Replace("<svg", "<svg xmlns=\"http://www.w3.org/2000/svg\"");
							}
							return source;
						}
						return null;
					}
				}
				public string OctIcon
				{
					get
					{
						return Svg.Split(new[]{ ' ', '=', '"', }).Where(i => i.StartsWith("octicon-")).FirstOrDefault();
					}
				}

				public static Content Parse(XElement Node)
				{
					var result = new Content();
					result.Type = Node.Attribute("type").Value;
					result.Value = Node.Value;
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
				public Content Content { get; set; }

				public static Entry Parse(XElement Node)
				{
					var result = new Entry();
					result.Id = Node.LocalElement("id").Value;
					result.Published = DateTime.Parse(Node.LocalElement("published").Value);
					result.Updated = DateTime.Parse(Node.LocalElement("updated").Value);
					result.LinkList = Node.LocalElements("link").Select(i => Link.Parse(i)).ToList();
					result.Title = Node.LocalElement("title").Value;
					result.Content = Content.Parse(Node.LocalElement("content"));
					return result;
				}

				public bool IsContribution => !string.IsNullOrWhiteSpace(Id) && Id.IndexOf(":WatchEvent/") < 0;
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

		static public Feed ParseFeed(byte[] AtomRawBytes)
		{
			using (var stream = new MemoryStream(AtomRawBytes))
			{
				return Feed.Parse(XDocument.Load(stream).Root);
			}
		}
	}
}

