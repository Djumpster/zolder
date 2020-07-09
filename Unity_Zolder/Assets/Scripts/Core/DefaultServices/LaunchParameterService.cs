// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEngine.Networking;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	public class LaunchParameterService
	{
		private static readonly char[] urlSplitCharacters = new char[] { '?' };
		private static readonly char[] querySplitCharacters = new char[] { '&' };
		private static readonly char[] parameterSplitCharacters = new char[] { '=' };

		private readonly Dictionary<string, string> parameters;
		public readonly string LaunchUrl;

		public LaunchParameterService()
		{
			LaunchUrl = GetLaunchUrl() ?? string.Empty;
			parameters = ParseUrl(LaunchUrl);

			string log = "[CONTEXT] Launch Url: " + LaunchUrl + "\n";
			foreach (KeyValuePair<string, string> entry in parameters)
			{
				log += "\tParameter: '" + entry.Key + "' = '" + entry.Value + "'\n";
			}

			LogUtil.Log(LogTags.SYSTEM, this, log);
		}

		public bool HasParameter(string key)
		{
			return parameters.ContainsKey(key);
		}

		public string GetParameter(string key, string defaultValue)
		{
			string value;
			return parameters.TryGetValue(key, out value) ? value : defaultValue;
		}

		private string GetLaunchUrl()
		{
#if UNITY_EDITOR
			return string.Empty;
#elif UNITY_IOS
			const string PLAYER_PREFS_KEY = "launchUrl";
			return PlayerPrefs.GetString(PLAYER_PREFS_KEY, string.Empty);
#elif UNITY_ANDROID
			AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject launchIntent = activity.Call<AndroidJavaObject>("getIntent");
			return launchIntent.Call<string>("getDataString");
#elif UNITY_WEBPLAYER
			return Application.absoluteURL;
#else
			return string.Empty;
#endif
		}

		private Dictionary<string, string> ParseUrl(string url)
		{
			Dictionary<string, string> queryParamters = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(url))
			{
				string[] split = url.Split(urlSplitCharacters);
				if (split.Length >= 2)
				{
					string queryString = split[1];
					foreach (string element in queryString.Split(querySplitCharacters))
					{
						string[] parameter = element.Split(parameterSplitCharacters);
						if (parameter.Length == 2)
						{
							string key = UnityWebRequest.UnEscapeURL(parameter[0]).Trim();
							string value = UnityWebRequest.UnEscapeURL(parameter[1]).Trim();
							queryParamters.Add(key, value);
						}
					}
				}
			}
			return queryParamters;
		}
	}
}