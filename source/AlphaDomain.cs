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
		public HttpClient HttpClient = new HttpClient();
		public void RefreshHttpClient()
		{
			HttpClient?.Dispose();
			HttpClient = new HttpClient();
		}

		private DateTime LastPublicActivityCache;
		public DateTime LastPublicActivity
		{
			set
			{
				Settings.LastPublicActivity = LastPublicActivityCache = value;
			}
			get
			{
				return LastPublicActivityCache;
			}
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
			LastPublicActivityCache = Settings.LastPublicActivity;
		}

		public async Task AutoUpdateLastPublicActivityAsync()
		{
			//Debug.WriteLine("AlphaDomain::AutoUpdateInfoAsync");
			if (NextUpdateLastPublicActivityStamp <= DateTime.Now)
			{
				await UpdateLastPublicActivityAsync();
			}
		}
		public async Task ManualUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::ManualUpdateInfoAsync");
			await UpdateLastPublicActivityAsync();
		}
		private async Task UpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync");
			PreviousUpdateLastPublicActivityStamp = DateTime.Now;
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				try
				{
					OnStartQuery();

					var OldLastPublicActivity = LastPublicActivity;
					LastPublicActivity = await GitHub.GetLastPublicActivityAsync(HttpClient, User);
					Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync::LastPublicActivity = " + LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss"));

					if (OldLastPublicActivity != LastPublicActivity)
					{
						OnUpdateLastPublicActivity();
					}
					Settings.IsValidUserName = true;
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
		public void OnUpdateLastPublicActivity()
		{
			Device.BeginInvokeOnMainThread
			(
				() =>
				{
					UpdateAlerts();
					AlphaFactory.GetApp()?.Main?.OnUpdateLastPublicActivity();
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

		public virtual void UpdateAlerts()
		{
			CancelAllAlerts();
			if
			(
				String.IsNullOrWhiteSpace(Settings.UserName) ||
				default(DateTime) == LastPublicActivity
			)
			{
				Debug.WriteLine("AlphaDomain::CancelAllAlerts");
			}
			else
			{
				Debug.WriteLine("AlphaDomain::UpdateAlerts");
				var Limit = LastPublicActivity.AddHours(24);
				var LastPublicActivityInfo = L["Last Acitivity Stamp"] + ": " + LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");
				var Now = DateTime.Now;
				int i = 0;
				foreach (var Span in Settings.AlertTimeSpanTable)
				{
					++i;
					var AlertStamp = Limit -Span;
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
						var AlertStamp = (Now.TimeOfDay < Span ? Now: Now.AddDays(1))
							-Now.TimeOfDay
							+Span;
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
						Color = Color.FromRgb(0xAA, 0xAA, 0xAA),
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
	}
}

