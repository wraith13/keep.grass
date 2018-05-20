﻿using System;
using System.Net.Http;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using RuyiJinguBang;

namespace keep.grass.Domain
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
                    return new Link
                    {
                        Type = Node.Attribute("type").Value,
                        Rel = Node.Attribute("rel").Value,
                        Href = Node.Attribute("href").Value
                    };
                }
            }
            public class Content
            {
                public string Type { get; set; }
                public string Value { get; set; }

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
                    return new Content
                    {
                        Type = Node.Attribute("type").Value,
                        Value = Node.Value
                    };
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
                    return new Entry
                    {
                        Id = Node.LocalElement("id").Value,
                        Published = DateTime.Parse(Node.LocalElement("published").Value),
                        Updated = DateTime.Parse(Node.LocalElement("updated").Value),
                        LinkList = Node.LocalElements("link").Select(i => Link.Parse(i)).ToList(),
                        Title = Node.LocalElement("title").Value,
                        Content = Content.Parse(Node.LocalElement("content"))
                    };
                }

                public string EventTypeName => Regex.Match(Id, "([A-Za-z0-9]+Event)").Value;

                public bool IsContribution
                {
                    get
                    {
                        Debug.WriteLine($"EVENT: {EventTypeName}");
                        switch (EventTypeName)
                        {
                        //  現状、まだ確認がとれてないモノをコメントアウトしている
                        case "GollumEvent":
                            return false;
                        case "PullRequestReviewCommentEvent":
                            return true;
                        case "IssueCommentEvent":
                            return false;
                        case "CreateEvent":
                            return 0 <= Title.IndexOf(" created a repository ");
                        case "ForkEvent":
                            return true;
                        case "DeleteEvent":
                            return false;
                        case "PushEvent":
                            return true;
                        case "PullRequestEvent":
                            return true; // false の場合もある？
                        case "IssuesEvent":
                            return
                                0 <= Title.IndexOf(" opened an issue in ") ||
                                0 <= Title.IndexOf(" opened issue ") ;
                        case "MemberEvent":
                            return false;
                        case "WatchEvent":
                            return false;
                        }
                        Debug.WriteLine($"IsContribution({EventTypeName}): UNKNOWN EVENT!!!");
                        return false;
                    }
                }
                public string OctIcon
                {
                    get
                    {
                        switch (EventTypeName)
                        {
                            case "GollumEvent":
                                return "book";
                            case "PullRequestReviewCommentEvent":
                                return "comment-discussion";
                            case "IssueCommentEvent":
                                return "comment-discussion";
                            case "CreateEvent":
                                return "git-branch";
                            case "ForkEvent":
                                return "git-branch";
                            case "DeleteEvent":
                                return "git-branch";
                            case "PushEvent":
                                return "git-commit";
                            case "PullRequestEvent":
                                return "git-pull-request";
                            case "IssuesEvent":
                                return
                                    (
                                        0 <= Title.IndexOf(" opened an issue in ") ||
                                        0 <= Title.IndexOf(" opened issue ")
                                    ) ? "issue-opened" :
                                    (
                                        (
                                            0 <= Title.IndexOf(" reopened an issue in ") ||
                                            0 <= Title.IndexOf(" reopened issue ")
                                        ) ? "issue-reopened" :
                                        "issue-closed"
                                    );
                            case "MemberEvent":
                                return "person";
                            case "WatchEvent":
                                return "star";
                            case "ReleaseEvent":
                                return "book";
                        }
                        Debug.WriteLine($"OctIcon({EventTypeName}): UNKNOWN EVENT!!!");
                        return null;
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
                return new Feed
                {
                    Id = Root.LocalElement("id").Value,
                    Title = Root.LocalElement("title").Value,
                    Updated = DateTime.Parse(Root.LocalElement("updated").Value),
                    LinkList = Root.LocalElements("link").Select(i => Link.Parse(i)).ToList(),
                    EntryList = Root.LocalElements("entry").Select(i => Entry.Parse(i)).ToList()
                };
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

        private static readonly string BaseUrl = "https://github.com";
        private static readonly string BaseApiUrl = "https://api.github.com";

        private static readonly string ProfileUrlFormat = BaseUrl + "/{0}";
        static public string GetProfileUrl(string Id)
        {
            return String.Format(ProfileUrlFormat, Id);
        }

        private static readonly string UserUrlFormat = BaseApiUrl + "/users/{0}";
        static public string GetUserUrl(string Id)
        {
            return String.Format(UserUrlFormat, Id);
        }

        private static readonly string AcitivityUrlFormat = BaseUrl + "/{0}?tab=activity";
        static public string GetAcitivityUrl(string Id)
        {
            return String.Format(AcitivityUrlFormat, Id);
        }

        private static readonly string AtomUrlFormat = BaseUrl + "/{0}.atom";
        static public string GetAtomUrl(string Id)
        {
            return String.Format(AtomUrlFormat, Id);
        }

        private static readonly string IconUrlFormat = BaseUrl + "/{0}.png";
        static public string GetIconUrl(string Id)
        {
            return String.Format(IconUrlFormat, Id);
        }

        private static readonly string SearchUsersUrlFormat = BaseApiUrl + "/search/users?q={0}";
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

