using System;
using System.Diagnostics;
using System.Threading.Tasks;
using keep.grass.Helpers;

namespace keep.grass
{
	public abstract class AlphaDomain
	{
		protected Languages.AlphaLanguage L = AlphaFactory.MakeLanguage();

		public DateTime? LastPublicActivity;
		DateTime LastCheckStamp = default(DateTime);
		TimeSpan NextCheckTimeSpan = default(TimeSpan);

		public AlphaDomain()
		{
			L = AlphaFactory.MakeLanguage();
		}

		public async Task AutoUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::AutoUpdateInfoAsync");
			if (TimeSpan.FromSeconds(60) < DateTime.Now - LastCheckStamp)
			{
				await UpdateLastPublicActivityAsync();
			}
		}
		public async Task ManualUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::ManualUpdateInfoAsync");
			await UpdateLastPublicActivityAsync();
			NextCheckTimeSpan = TimeSpan.FromSeconds(60);
		}
		private async Task UpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync");
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				try
				{
					LastCheckStamp = DateTime.Now;

					OnStartQuery();

					var OldLastPublicActivity = LastPublicActivity;
					LastPublicActivity = await GitHub.GetLastPublicActivityAsync(User);
					Debug.WriteLine("AlphaDomain::UpdateLastPublicActivityAsync::LastPublicActivity = " + LastPublicActivity.Value.ToString("yyyy-MM-dd HH:mm:ss"));

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
				null == LastPublicActivity
			)
			{
				Debug.WriteLine("AlphaDomain::CancelAllAlerts");
			}
			else
			{
				Debug.WriteLine("AlphaDomain::UpdateAlerts");
				var Limit = LastPublicActivity.Value.AddHours(24);
				var LastPublicActivityInfo = L["Last Acitivity Stamp"] + ": " + LastPublicActivity.Value.ToString("yyyy-MM-dd HH:mm:ss");
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
	}
}

