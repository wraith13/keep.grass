// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace keep.grass.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static readonly string SettingsDefault = string.Empty;

		#endregion


		public static string GeneralSettings
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(SettingsKey, SettingsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(SettingsKey, value);
			}
		}

		private const string LanguageKey = "Language";
		private static readonly string LanguageDefault = string.Empty;
		public static string Language
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(LanguageKey, LanguageDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(LanguageKey, value);
			}
		}

		private const string UserNameKey = "UserName";
		private static readonly string UserNameDefault = string.Empty;
		public static string UserName
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(UserNameKey, UserNameDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(UserNameKey, value);
			}
		}
		public static string FriendSettingKey(int index)
		{
			return String.Format("friend{0:D2}", index);
		}
		public static string GetFriend(int index)
		{
			return Get(FriendSettingKey(index), "");
		}
		public static void SetFriend(int index, string NewValue)
		{
			Set(FriendSettingKey(index), NewValue);
		}
		public static int GetFriendCount()
		{
			var result = 0;
			while(!string.IsNullOrWhiteSpace(GetFriend(result)))
			{
				++result;
			}
			return result;
		}
		public static string[] GetFriendList()
		{
			var result = new List<string>();
			int i = 0;
			while(true)
			{
				var Friend = GetFriend(i++);
				if (string.IsNullOrWhiteSpace(Friend))
				{
					break;
				}
				else
				{
					result.Add(Friend);
				}
			}
			return result.ToArray();
		}

		private const string IsValidUserNameKey = "IsValidUserName";
		private static readonly bool IsValidUserNameDefault = false;
		public static bool IsValidUserName
		{
			get
			{
				return AppSettings.GetValueOrDefault<bool>(IsValidUserNameKey, IsValidUserNameDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue<bool>(IsValidUserNameKey, value);
			}
		}

		private const string LastPublicActivityKey = "LastPublicActivity";
		private static readonly DateTime LastPublicActivityDefault = default(DateTime);
		public static DateTime LastPublicActivity
		{
			get
			{
				var value = AppSettings.GetValueOrDefault<DateTime>(LastPublicActivityKey, LastPublicActivityDefault);
				return value == LastPublicActivityDefault ?
					value:
					value.ToLocalTime();
			}
			set
			{
				AppSettings.AddOrUpdateValue<DateTime>
	           	(
	           		LastPublicActivityKey,
					value == LastPublicActivityDefault ?
		           		value:
			           	value.ToUniversalTime()
	          	);
			}
		}

		public static T Get<T>(string Key, T DefaultValue)
		{
			return AppSettings.GetValueOrDefault<T>(Key, DefaultValue);
		}
		public static void Set<T>(string Key, T NewValue)
		{
			AppSettings.AddOrUpdateValue<T>(Key, NewValue);
		}

		public static TimeSpan[] AlertTimeSpanTable = new []
		{
			TimeSpan.FromTicks(0),
			TimeSpan.FromMinutes(5),
			TimeSpan.FromMinutes(10),
			TimeSpan.FromMinutes(15),
			TimeSpan.FromMinutes(30),
			TimeSpan.FromMinutes(45),
			TimeSpan.FromHours(1),
			TimeSpan.FromHours(2),
			TimeSpan.FromHours(3),
			TimeSpan.FromHours(4),
			TimeSpan.FromHours(5),
			TimeSpan.FromHours(6),
			TimeSpan.FromHours(7),
			TimeSpan.FromHours(8),
			TimeSpan.FromHours(9),
			TimeSpan.FromHours(10),
			TimeSpan.FromHours(11),
			TimeSpan.FromHours(12),
			TimeSpan.FromHours(13),
			TimeSpan.FromHours(14),
			TimeSpan.FromHours(15),
			TimeSpan.FromHours(16),
			TimeSpan.FromHours(17),
			TimeSpan.FromHours(18),
			TimeSpan.FromHours(19),
			TimeSpan.FromHours(20),
			TimeSpan.FromHours(21),
			TimeSpan.FromHours(22),
			TimeSpan.FromHours(23),
		};
		public static string AlertTimeSpanToDisplayName(Languages.AlphaLanguage L, TimeSpan left)
		{
			if (TimeSpan.FromHours(1) < left)
			{
				return String.Format(L["{0} hours left"], left.TotalHours);
			}
			else
			if (TimeSpan.FromHours(1) == left)
			{
				return L["1 hour left"];
			}
			else
			if (TimeSpan.FromMinutes(1) < left)
			{
				return String.Format(L["{0} minutes left"], left.TotalMinutes);
			}
			else
			if (TimeSpan.FromMinutes(1) == left)
			{
				return L["1 minute left"];
			}
			else
			{
				return L["Just 24 hours later"];
			}
		}
		public static string AlertLeftTimeToDisplayName(Languages.AlphaLanguage L, TimeSpan left)
		{
			if (0 <= left.Ticks)
			{
				return String.Format(L["{0:D2}:{1:D2} left"], left.Hours, left.Minutes);
			}
			else
			{
				var over = -left;
				return String.Format(L["{0:D2}:{1:D2} over"], over.Hours, over.Minutes);
			}
		}
		public static string AlertTimeSpanToSettingKey(TimeSpan left)
		{
			return String.Format("alert{0}{1}", left.Hours, left.Minutes);
		}
		public static bool GetAlert(TimeSpan key)
		{
			return Get(AlertTimeSpanToSettingKey(key), false);
		}
		public static void SetAlert(TimeSpan Key, bool NewValue)
		{
			Set(AlertTimeSpanToSettingKey(Key), NewValue);
		}

		public static TimeSpan AlertDailyTimeUnit = TimeSpan.FromMinutes(30);
		public static TimeSpan[] AlertDailyTimeTable = Enumerable.Range
		(
			0,
			(int)(TimeSpan.FromDays(1).Ticks /AlertDailyTimeUnit.Ticks)
		)
		.Select(i => TimeSpan.FromTicks(i *AlertDailyTimeUnit.Ticks))
		.ToArray();
		public static string AlertDailyTimeToDisplayName(Languages.AlphaLanguage L, TimeSpan Time)
		{
			return String.Format(L["Every day at {0:D2}:{1:D2}"], Time.Hours, Time.Minutes);
		}
		public static string AlertDailyTimeToSettingKey(TimeSpan Time)
		{
			return String.Format("daily{0:D2}{1:D2}", Time.Hours, Time.Minutes);
		}
		public static bool GetDailyAlert(TimeSpan key)
		{
			return Get(AlertDailyTimeToSettingKey(key), false);
		}
		public static void SetDailyAlert(TimeSpan Key, bool NewValue)
		{
			Set(AlertDailyTimeToSettingKey(Key), NewValue);
		}

	}
}