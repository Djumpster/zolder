// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

#if UNITY_ANALYTICS_PLUGIN
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

namespace Talespin.Core.Foundation.Analytics
{
	/// <summary>
	/// A wrapper around the Unity analytics portal, requires the Unity
	/// Analytics package to be installed from the package manager,
	/// as well as a scripting define for <c>UNITY_ANALYTICS_PLUGIN</c>.
	/// </summary>
	public class UnityAnalyticsProvider : IAnalyticsProvider
	{
		private readonly string defaultUserId;

		private string sessionId;

		public UnityAnalyticsProvider()
		{
			defaultUserId = AnalyticsSessionInfo.userId;
		}

		/// <inheritdoc/>
		public bool GameStart(IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.GameStart(parameters);
			return ExamineAnalyticsResult(result, "game_start", parameters);
		}

		/// <inheritdoc/>
		public bool GameOver(IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.GameOver(0, null, parameters);
			return ExamineAnalyticsResult(result, "game_over", parameters);
		}

		/// <inheritdoc/>
		public bool LevelStart(string name, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.LevelStart(name, parameters);
			return ExamineAnalyticsResult(result, "level_start", parameters, name);
		}

		/// <inheritdoc/>
		public bool LevelComplete(string name, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.LevelComplete(name, parameters);
			return ExamineAnalyticsResult(result, "level_complete", parameters, name);
		}

		/// <inheritdoc/>
		public bool LevelQuit(string name, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.LevelQuit(name, parameters);
			return ExamineAnalyticsResult(result, "level_quit", parameters, name);
		}

		/// <inheritdoc/>
		public bool LevelFail(string name, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.LevelFail(name, parameters);
			return ExamineAnalyticsResult(result, "level_fail", parameters, name);
		}

		/// <inheritdoc/>
		public bool ScreenVisit(string name, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.ScreenVisit(name, parameters);
			return ExamineAnalyticsResult(result, "screen_visit", parameters, name);
		}

		/// <inheritdoc/>
		public bool Custom(string eventName, IDictionary<string, object> parameters = null)
		{
			parameters = InjectMandatoryData(parameters);
			AnalyticsResult result = AnalyticsEvent.Custom(eventName, parameters);
			return ExamineAnalyticsResult(result, eventName, parameters);
		}

		/// <inheritdoc/>
		public void SetUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				userId = defaultUserId;
			}

			UnityEngine.Analytics.Analytics.SetUserId(userId);
		}

		/// <inheritdoc/>
		public void SetSessionId(string sessionId)
		{
			this.sessionId = sessionId;
		}

		private IDictionary<string, object> InjectMandatoryData(IDictionary<string, object> parameters)
		{
			if (parameters == null)
			{
				parameters = new Dictionary<string, object>();
			}

			if (!string.IsNullOrEmpty(sessionId))
			{
				parameters["session_id"] = sessionId;
			}

			return parameters;
		}

		private static bool ExamineAnalyticsResult(AnalyticsResult result, string eventName, IDictionary<string, object> parameters, params string[] additionalParameters)
		{
			string eventString = GetEventDataString(eventName, parameters, additionalParameters);

			if (result == AnalyticsResult.Ok)
			{
				Debug.Log($"Sent Unity analytics event: {eventString}");
				return true;
			}

			Debug.LogError($"Failed to send Unity analytics event, got \"{result}\" for event: {eventString}");
			return false;
		}

		private static string GetEventDataString(string eventName, IDictionary<string, object> parameters, string[] additionalParameters)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(eventName);
			builder.Append(" { ");

			bool first = true;

			if (additionalParameters != null)
			{
				foreach (string parameter in additionalParameters)
				{
					if (!first)
					{
						builder.Append(", ");
					}

					first = false;

					builder.Append(parameter);
				}
			}

			if (parameters != null)
			{
				foreach (KeyValuePair<string, object> parameter in parameters)
				{
					if (!first)
					{
						builder.Append(", ");
					}

					first = false;

					builder.Append(parameter.Key);
					builder.Append(" = ");
					builder.Append(parameter.Value);
				}
			}

			builder.AppendLine(" } ");

			return builder.ToString();
		}
	}
}
#endif // UNITY_ANALYTICS_PLUGIN
