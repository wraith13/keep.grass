using System;
using System.Net.Http;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

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

				public string OctIcon => Svg
					.Split(new[]{ ' ', '=', '"', })
					.Where(i => i.StartsWith("octicon-"))
					.FirstOrDefault();

				public string SingleLineValue => Regex.Replace(Value, "[\r\n]+", " ");

				public IEnumerable<string> GetTagText(string Tag) =>
					Regex.Matches(SingleLineValue, $"<{Tag}[^>]*>(?<item>.*?)</{Tag}>").Cast<Match>()
					.Select(i => i.Groups["item"].Value)
					.Select(i => Regex.Replace(i, "<.*?>", ""))
					.Select(i => System.Net.WebUtility.HtmlDecode(i))
					.Select(i => Regex.Replace(i, "[ \r\n\t]+", " "))
					.Select(i => i.Trim());
				
				public IEnumerable<string> Details
				{
					get
					{
						var result = GetTagText("li");
						if (!result.Any())
						{
							result = GetTagText("blockquote");
						}
						return result;
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

				public string EventTypeName => Regex.Match(Id, "([A-Za-z0-9]+Event)").Value;

				/*
				public bool IsContribution
				{
					get
					{
						var Event = EventTypeName;
						return !string.IsNullOrWhiteSpace(Event) &&
							Event != "WatchEvent" &&
							Event != "IssueCommentEvent" &&
							Event != "GollumEvent";
					}
				}
				*/
					
				public bool IsContribution
				{
					get
					{
						var OctIcon = Content.OctIcon;
						var Tag = "octicon-";
						if (OctIcon.StartsWith(Tag))
						{
							Debug.WriteLine($"EVENT: {EventTypeName}");
							string CoreName = OctIcon.Substring(Tag.Length);
							switch (CoreName)
							{
							//	現状、まだ確認がとれてないモノをコメントアウトしている
								case "book":
									//	GollumEvent
									return false;
								case "comment-discussion":
									//	PullRequestReviewCommentEvent
									//	IssueCommentEvent
									return false; // true の場合もある？
								case "git-branch":
									//	CreateEvent ( created branch ) の場合は false
									//	ForkEvent ( forked branch ) の場合は true
									//	DeleteEvent ( delete branch ) の場合は false
									//	...これらはたまたまそうなるだけで実際には master ブランチかどうかが肝だと思われる
									return "ForkEvent" == EventTypeName;
								case "git-commit":
									//	PushEvent
									//	master ブランチへの commit の場合にのみ true
									return 0 <= Title.IndexOf(" pushed to master ");
								//case "git-compare":
								//	return true;
								//case "git-merge":
								//	return true;
								case "git-pull-request":
									//	PullRequestEvent
									return true; // false の場合もある？
								case "issue-closed":
									//	IssuesEvent
									return false;
								case "issue-opened":
									//	IssuesEvent
									return true;
								//case "issue-reopened":
								//	return true;
								//case "mark-github":
								//	return true;
								case "repo":
									return true;
								case "star":
									//	WatchEvent
									return false;
								case "tag":
									//	CreateEvent
									return false;
							}
						}
						Debug.WriteLine($"IsContribution({OctIcon}): UNKNOWN EVENT!!!");
						return false;
					}
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
		public class SearchResult<T>
		{
			[JsonProperty("total_count")]
			public long TotalCount { get; set; }
			[JsonProperty("incomplete_results")]
			public bool IncompleteResults { get; set; }
			[JsonProperty("items")]
			public IEnumerable<T> Items { get; set; }

			public static SearchResult<T> Parse(string Json)
			{
				return JsonConvert.DeserializeObject<SearchResult<T>>(Json);
			}
		}
		public class BaseUser
		{
			[JsonProperty("login")]
			public string Login { get; set; }
			[JsonProperty("Id")]
			public long Id { get; set; }
			[JsonProperty("avatar_url")]
			public string AvatarUrl { get; set; }
			[JsonProperty("gravatar_id")]
			public string GravatarId { get; set; }
			[JsonProperty("url")]
			public string Url { get; set; }
			[JsonProperty("html_url")]
			public string HtmlUrl { get; set; }
			[JsonProperty("followers_url")]
			public string FollowersUrl { get; set; }
			[JsonProperty("following_url")]
			public string FollowingUrl { get; set; }
			[JsonProperty("gists_url")]
			public string GistsUrl { get; set; }
			[JsonProperty("starred_url")]
			public string StarredUrl { get; set; }
			[JsonProperty("subscriptions_url")]
			public string SubscriptionsUrl { get; set; }
			[JsonProperty("organizations_url")]
			public string OrganizationsUrl { get; set; }
			[JsonProperty("repos_url")]
			public string ReposUrl { get; set; }
			[JsonProperty("events_url")]
			public string EventsUrl { get; set; }
			[JsonProperty("received_events_url")]
			public string ReceivedEventsUrl { get; set; }
			[JsonProperty("type")]
			public string Type { get; set; }
			[JsonProperty("site_admin")]
			public bool SiteAdmin { get; set; }
		}
		public class SearchUser : BaseUser
		{
			[JsonProperty("score")]
			public double Score { get; set; }
		}
		public class User : BaseUser
		{
			[JsonProperty("name")]
			public string Name { get; set; }
			[JsonProperty("company")]
			public string Company { get; set; }
			[JsonProperty("blog")]
			public string Blog { get; set; }
			[JsonProperty("location")]
			public string Location { get; set; }
			[JsonProperty("email")]
			public string Email { get; set; }
			[JsonProperty("hireable")]
			public bool? Hireable { get; set; }
			[JsonProperty("bio")]
			public string Bio { get; set; }
			[JsonProperty("public_repos")]
			public int PublicRepos { get; set; }
			[JsonProperty("public_gists")]
			public int PublicGists { get; set; }
			[JsonProperty("followers")]
			public int Followers { get; set; }
			[JsonProperty("following")]
			public int Following { get; set; }
			[JsonProperty("created_at")]
			public DateTime CreatedAt { get; set; }
			[JsonProperty("updated_at")]
			public DateTime UpdatedAt { get; set; }

			public static User Parse(string Json)
			{
				return JsonConvert.DeserializeObject<User>(Json);
			}
		}

		static private string BaseUrl = "https://github.com";
		static private string BaseApiUrl = "https://api.github.com";

		static private string ProfileUrlFormat = BaseUrl +"/{0}";
		static public string GetProfileUrl(string Id)
		{
			return String.Format(ProfileUrlFormat, Id);
		}

        static private string UserUrlFormat = BaseApiUrl + "/users/{0}";
        static public string GetUserUrl(string Id)
        {
            return String.Format(UserUrlFormat, Id);
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

		static private string SearchUsersUrlFormat = BaseApiUrl + "/search/users?q={0}";
		static public string GetSearchUsersUrl(string Query)
		{
			return String.Format
			(
				SearchUsersUrlFormat,
				System.Net.WebUtility.UrlEncode(Query)
			);
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

