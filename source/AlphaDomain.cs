using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using keep.grass.Helpers;
using Xamarin.Forms;

namespace keep.grass
{
	public abstract class AlphaDomain
	{
		protected static Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		private HttpClient HttpClient = new HttpClient();
		public void RefreshHttpClient()
		{
			HttpClient?.Dispose();
			HttpClient = new HttpClient();
		}

		public virtual async Task<byte[]> GetByteArrayFromUrlAsync(string Url)
		{
			return await HttpClient.GetByteArrayAsync(Url);
		}
		public async Task<string> GetStringFromUrlAsync(string Url, System.Text.Encoding Encoding)
		{
			var Data = await GetByteArrayFromUrlAsync(Url);
			return Encoding.GetString(Data, 0, Data.Length);
		}
		public async Task<string> GetStringFromUrlAsync(string Url)
		{
			return await GetStringFromUrlAsync(Url, System.Text.Encoding.UTF8);
		}

		private Dictionary<string, DateTime> LastPublicActivityCache = new Dictionary<string, DateTime>();
		public void SetLastPublicActivity(string User, DateTime value)
		{
			Debug.WriteLine($"AlphaDomain::SetLastPublicActivity({User},{ToString(value)}); ");
			if (GetLastPublicActivity(User) != value)
			{
				Settings.SetLastPublicActivity(User, value);
				lock (LastPublicActivityCache)
				{
					LastPublicActivityCache[User] = value;
				}
				Settings.SetIsValidUserName(User, true);
				OnUpdateLastPublicActivity(User, value);
			}
		}
		public DateTime GetLastPublicActivity(string User)
		{
			var result = default(DateTime);
			if (!string.IsNullOrWhiteSpace(User))
			{
				lock (LastPublicActivityCache)
				{
					if (!LastPublicActivityCache.TryGetValue(User, out result))
					{
						LastPublicActivityCache[User] =
						result =
							Settings.GetLastPublicActivity(User);
					}
				}
			}
			return result;
		}

		private Dictionary<string, GitHub.Feed> FeedCache = new Dictionary<string, GitHub.Feed>();
		public void SetFeed(string User, GitHub.Feed value)
		{
			Debug.WriteLine($"AlphaDomain::SetFeed({User},...); ");
			lock (FeedCache)
			{
				FeedCache[User] = value;
			}
		}
		public async Task<GitHub.Feed> GetFeed(string User)
		{
			Debug.WriteLine($"AlphaDomain::GetFeed({User}); ");
			var result = default(GitHub.Feed);
			lock (FeedCache)
			{
				if (FeedCache.TryGetValue(User, out result))
				{
					Debug.WriteLine($"AlphaDomain::GetFeed({User}): Hit Cache; ");
					return result;
				}
			}
			SetFeed
			(
				User,
				result = GitHub.ParseFeed
				(
					await GetByteArrayFromUrlAsync
					(
						GitHub.GetAtomUrl(User)
					)
				)
			);
			return result;
		}

		private DateTime PreviousUpdateLastPublicActivityStamp = default(DateTime);
		public DateTime NextUpdateLastPublicActivityStamp
		{
			get
			{
				return PreviousUpdateLastPublicActivityStamp
					+TimeSpan.FromSeconds(300);
			}
		}

		public AlphaDomain()
		{
			//LastPublicActivityCache = Settings.LastPublicActivity;
		}

		public async Task AutoUpdateLastPublicActivityAsync()
		{
			//Debug.WriteLine("AlphaDomain::AutoUpdateInfoAsync");
			if (NextUpdateLastPublicActivityStamp <= DateTime.Now)
			{
				await UpdateAllLastPublicActivityAsync();
			}
		}
		public async Task BackgroundUpdateLastPublicActivityAsync()
		{
			//Debug.WriteLine("AlphaDomain::AutoUpdateInfoAsync");
			var User = Settings.UserName;
			if (NextUpdateLastPublicActivityStamp <= DateTime.Now && !string.IsNullOrWhiteSpace(User))
			{
				await UpdateLastPublicActivityAsync(User);
			}
		}
		public async Task ManualUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::ManualUpdateInfoAsync");
			await UpdateAllLastPublicActivityAsync();
		}
		private async Task UpdateLastPublicActivityCoreAsync(string User)
		{
			var Feed = GitHub.ParseFeed
			(
				await GetByteArrayFromUrlAsync
				(
					GitHub.GetAtomUrl(User)
				)
			);
			SetFeed
			(
				User,
				Feed
			);
			var LastPublicActivity = Feed.EntryList
				.Where(i => i.IsContribution)
				.Select(i => i.Updated)
				.FirstOrDefault();
			if (!LastPublicActivity.IsDefault())
			{
				SetLastPublicActivity
				(
					User,
					Feed.EntryList
						.Where(i => i.IsContribution)
						.Select(i => i.Updated)
						.FirstOrDefault()
				);
			}
		}
		private async Task UpdateIconAsync(string User)
		{
			var IconUrl = GitHub.GetIconUrl(User);
			if (!(AlphaImageProxy.GetFromCache(IconUrl)?.Any() ?? false))
			{
				var Binary = await AlphaImageProxy.Get(IconUrl);
				if (Binary?.Any() ?? false)
				{
					OnUpdateIcon(User, Binary);
				}
			}
		}

		private async Task UpdateAllLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync");
			PreviousUpdateLastPublicActivityStamp = DateTime.Now;

			await UpdateLastPublicActivityAsync
			(
				new[] { Settings.UserName, }
					.Union(Settings.GetFriendList())
					.Where(i => !string.IsNullOrWhiteSpace(i))
					.ToArray()
			);
		}
		private async Task UpdateLastPublicActivityAsync(params string[] Users)
		{
			Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync");
			PreviousUpdateLastPublicActivityStamp = DateTime.Now;
			try
			{
				OnStartQuery();
				foreach (var User in Users)
				{
					await UpdateLastPublicActivityCoreAsync(User);
					await UpdateIconAsync(User);
				}
			}
			catch (Exception err)
			{
				Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync::catch::err" + err.ToString());
				RefreshHttpClient();
				OnErrorInQuery();
			}
			finally
			{
				OnEndQuery();
			}
		}
		public void OnStartQuery()
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					AlphaFactory.GetApp()?.Main?.OnStartQuery();
				}
			);
		}
		public void OnUpdateLastPublicActivity(string User, DateTime LastPublicActivity)
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					if (User == Settings.UserName)
					{
						UpdateAlerts(LastPublicActivity);
					}
					AlphaFactory.GetApp()?.Main?.OnUpdateLastPublicActivity(User, LastPublicActivity);
				}
			);
		}
		public void OnUpdateIcon(string User, byte[] Binary)
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					AlphaFactory.GetApp()?.Main?.OnUpdateIcon(User, Binary);
				}
			);
		}
		public void OnErrorInQuery()
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					AlphaFactory.GetApp()?.Main?.OnErrorInQuery();
				}
			);
		}
		public void OnEndQuery()
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					AlphaFactory.GetApp()?.Main?.OnEndQuery();
				}
			);
		}

		public virtual void UpdateAlerts(DateTime LastPublicActivity)
		{
			CancelAllAlerts();
			if (default(DateTime) < LastPublicActivity)
			{
				Debug.WriteLine("AlphaDomain::UpdateAlerts");
				var Limit = LastPublicActivity.AddHours(24);
				var LastPublicActivityInfo = L["Last Activity Stamp"] + ": " + ToString(LastPublicActivity);
				var Now = DateTime.Now;
				int i = 0;
				foreach (var Span in Settings.AlertTimeSpanTable)
				{
					++i;
					var AlertStamp = Limit - Span;
					if (Settings.GetAlert(Span) && Now < AlertStamp)
					{
						ShowAlert
						(
							Settings.AlertTimeSpanToDisplayName(L, Span),
							LastPublicActivityInfo,
							i,
							AlertStamp
						);
					}
					else
					{
						CancelAlert(i);
					}
				}
				i = 100;
				foreach (var Span in Settings.AlertDailyTimeTable)
				{
					++i;
					if (Settings.GetDailyAlert(Span))
					{
						var AlertStamp = (Now.TimeOfDay < Span ? Now : Now.AddDays(1))
							- Now.TimeOfDay
							+ Span;
						var LeftTime = Limit - AlertStamp;
						ShowAlert
						(
							Settings.AlertLeftTimeToDisplayName(L, LeftTime),
							LastPublicActivityInfo,
							i,
							AlertStamp
						);
					}
					else
					{
						CancelAlert(i);
					}
				}
			}
		}
		public abstract void ShowAlert(string title, string body, int id, DateTime notifyTime);
		public abstract void CancelAlert(int id);

		public virtual void CancelAllAlerts()
		{
			int i = 0;
			foreach (var Span in Settings.AlertTimeSpanTable)
			{
				CancelAlert(++i);
			}
		}

		public virtual Uri GetApplicationStoreUri()
		{
			return null;
		}

		public static Color GetElapsedTimeColor()
		{
			return Color.FromRgb(0xAA, 0xAA, 0xAA);
		}
		public static IEnumerable<TimePie> MakeSlices(TimeSpan LeftTime, Color LeftTimeColor)
		{
			if (0 <= LeftTime.Ticks)
			{
				//	※調整しておなかいと、表示上、経過時間と残り時間の合計が24時間より1秒足りない状態になってしまうので。
				var JustifiedLeftTime = new TimeSpan(LeftTime.Days, +LeftTime.Hours, LeftTime.Minutes, LeftTime.Seconds);
				var JustifiedElapsedTime = TimeSpan.FromDays(1) - JustifiedLeftTime;

				return new[]
				{
					new TimePie
					{
						Text = L["Left Time"],
						Value = JustifiedLeftTime,
						Color = LeftTimeColor,
					},
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = JustifiedElapsedTime,
						Color = GetElapsedTimeColor(),
					},
				};
			}
			else
			{
				return new[]
				{
					new TimePie
					{
						Text = L["Left Time"],
						Value = TimeSpan.FromTicks(0),
						Color = Color.FromRgb(0xD6, 0xE6, 0x85),
					},
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = TimeSpan.FromDays(1),
						Color = Color.FromRgb(0xEE, 0x11, 0x11),
					},
				};
			}
		}

		public static float TimeToAngle(DateTime Time)
		{
			return (float)((Time.TimeOfDay.Ticks * 360.0) / TimeSpan.FromDays(1).Ticks);
		}

		public static Color MakeLeftTimeColor(TimeSpan LeftTime)
		{
			double LeftTimeRate = Math.Max(0.0, Math.Min(1.0, LeftTime.TotalHours / 24.0));
			byte red = (byte)(255.0 * (1.0 - LeftTimeRate));
			byte green = (byte)(255.0 * Math.Min(0.5, LeftTimeRate));
			byte blue = 0;
			return Color.FromRgb(red, green, blue);
		}

		public static IEnumerable<CircleGraphSatelliteText> MakeSatelliteTexts(DateTime Now, DateTime LastPublicActivity, int mod = 0)
		{
			var Today = Now.Date;
			var LimitTime = LastPublicActivity.AddHours(24);
			var LeftTime = LimitTime - Now;
			return Enumerable.Range(0, 24).Select
			(
				i => new
				{
					Hour = i,
					Time = Today + TimeSpan.FromHours(i),
				}
			)
			.Select
			(
				i => new
				{
					Hour = i.Hour,
					Time = i.Time.Ticks < LastPublicActivity.Ticks ?
							i.Time + TimeSpan.FromDays(1) :
							(
								TimeSpan.FromDays(1).Ticks < (i.Time - LastPublicActivity).Ticks ?
								i.Time - TimeSpan.FromDays(1) :
								i.Time
							),
				}
			)
			.Select
			(
				i => new CircleGraphSatelliteText
				{
					Text = (mod < 2 || 0 == i.Hour % mod) ? i.Hour.ToString() : "・",
					Color = LeftTime.Ticks <= 0 ?
						MakeLeftTimeColor(LeftTime) :
						i.Time.Ticks < Now.Ticks ?
							 Color.Gray :
							MakeLeftTimeColor(LimitTime - i.Time),
					Angle = 360.0f * ((float)(i.Hour) / 24.0f),
				}
			);
		}

		public string ToString(DateTime a)
		{
			return a.IsDefault() ?
				"" :
				a.ToString("yyyy-MM-dd HH:mm:ss");
		}
		public string ToString(TimeSpan a)
		{
			return Math.Floor(a.TotalHours).ToString() + a.ToString("\\:mm\\:ss");
		}

		private string[] RecentUsers = null;
		public string[] GetRecentUsers()
		{
			if (null == RecentUsers)
			{
				RecentUsers = Settings.GetRecentUsers();
			}
			return RecentUsers;
		}
		public void AddRecentUser(string User)
		{
			RecentUsers = new[] { User }
				.Concat(GetRecentUsers())
				.Distinct()
				.Take(20)
				.ToArray();
			Settings.SetRecentUsers(RecentUsers);
		}
	}
}

