using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using keep.grass.Helpers;

namespace keep.grass
{
	public abstract class AlphaDomain
	{
		protected Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		protected HttpClient HttpClient = new HttpClient();

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

		public AlphaDomain()
		{
			LastPublicActivityCache = Settings.LastPublicActivity;
		}

		public async Task AutoUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::AutoUpdateInfoAsync");
			await UpdateLastPublicActivityAsync();
		}
		public async Task ManualUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::ManualUpdateInfoAsync");
			await UpdateLastPublicActivityAsync();
		}
		private async Task UpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync");
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
			AlphaFactory.GetApp()?.Main?.OnStartQuery();
		}
		public void OnUpdateLastPublicActivity()
		{
			UpdateAlerts();
			AlphaFactory.GetApp()?.Main?.OnUpdateLastPublicActivity();
		}
		public void OnErrorInQuery()
		{
			AlphaFactory.GetApp()?.Main?.OnErrorInQuery();
		}
		public void OnEndQuery()
		{
			AlphaFactory.GetApp()?.Main?.OnEndQuery();
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
					var AlertStamp = Limit.Add(-Span);
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
	}
}

