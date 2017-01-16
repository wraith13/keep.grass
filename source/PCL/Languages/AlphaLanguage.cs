using System;
using System.Collections.Generic;

using keep.grass.Helpers;

namespace keep.grass.Languages
{
	public class AlphaLanguage
	{
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
					{ "Rivals", "ライバル" },
					{ "Notifications", "通知" },
					{ "Alert by Left Time", "残り時間による通知" },
					{ "Daily Alert", "定刻通知" },
					{ "Last Activity Stamp", "最終アクティビティ日時" },
					{ "Activity", "アクティビティ" },
					{ "Left Time", "残り時間" },
					{ "Last Stamp", "最終時刻: " },
					{ "Elapsed Time", "経過時間" },
					{ "User ID", "ユーザーID" },
					{ "{0} hours left", "残り {0} 時間" },
					{ "1 hour left", "残り 1 時間" },
					{ "{0} minutes left", "残り {0} 分" },
					{ "1 minute left", "残り 1 分" },
					{ "Just 24 hours later", "24時間経過" },
					{ "{0:D2}:{1:D2} left", "残り {0:D2}:{1:D2}" },
					{ "{0:D2}:{1:D2} over", "{0:D2}:{1:D2} オーバー" },
					{ "Every day at {0:D2}:{1:D2}", "毎日 {0:D2}:{1:D2}" },
					{ "unspecified", "未指定" },
					{ "Error", "エラー" },
					{ "Language", "言語" },
					{ "Information", "情報" },
					{ "Version", "バージョン" },
					{ "Auther", "作者" },
					{ "Github Repository", "Github リポジトリ" },
					{ "Built with", "利用技術" },
                    { "Select a user", "ユーザーの選択" },
                    { "User's name etc.", "ユーザーの名前等" },
				}
			}
		};
		Dictionary<string, string> current;

		Dictionary<string, string> names = new Dictionary<string, string>
		{
			{ "", "System Default" },
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

		public AlphaLanguage()
		{
			Update();
		}
		public string this[string key]
		{
			get
			{
				var value = key;
				if (null != current && current.ContainsKey(key))
				{
					value = current[key];
				}
				return value;
			}
		}
		public void Update()
		{
			Set(Get());
		}
		public String Get()
		{
			var value = Settings.Language;
			if (String.IsNullOrWhiteSpace(value))
			{
				value = getLanguage();
			}
			return value;
		}
		public void Set(String lang)
		{
			current = null;
			master.TryGetValue(lang, out current);
		}

		public virtual String getLanguage()
		{
			return "ja";
		}
	}
}

