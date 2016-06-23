using System;
using System.Collections.Generic;

using keep.grass.Helpers;

namespace keep.grass.Languages
{
	public class AlphaLanguage
	{
		AlphaApp Root;
		Dictionary<string, Dictionary<string, string>> master = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"en",
				null
			},
			{
				"ja",
				new Dictionary<string, string>
				{
					{ "Settings", "設定" },
					{ "Update", "更新" },
					{ "Github Account", "Githubアカウント" },
					{ "Notifications", "通知" },
					{ "Last Acitivity Stamp", "最終アクティビティ日時" },
					{ "Left Time", "残り時間" },
					{ "Last Stamp: ", "最終時刻" },
					{ "User ID", "ユーザーID" },
					{ "{0} hours left", "残り {0} 時間" },
					{ "1 hour left", "残り 1 時間" },
					{ "{0} minutes left", "残り {0} 分" },
					{ "1 minute left", "残り 1 分" },
					{ "Just 24 hours later", "24時間経過" },
					{ "unspecified", "未指定" },
					{ "Error", "エラー" },
					{ "Language", "言語" }
				}
			}
		};
		Dictionary<string, string> current;

		Dictionary<string, string> names = new Dictionary<string, string>
		{
			{ "en", "English" },
			{ "ja", "日本語" },
		};

		public Dictionary<string, string> DisplayNames
		{
			get
			{
				return names;
			}
		}

		public AlphaLanguage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			Set(Get());
		}
		public string this[string key]
		{
			get
			{
				var value = key;
				if (null != current)
				{
					current.TryGetValue(key, out value);
				}
				return value;
			}
		}
		public String Get()
		{
			var value = Settings.Language;
			if (String.IsNullOrWhiteSpace(value))
			{
				value = Root.getLanguage();
			}
			return value;
		}
		public void Set(String lang)
		{
			current = master[lang];
		}
	}
}

