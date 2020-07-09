// Copyright 2020 Talespin, LLC. All Rights Reserved.

#if !UNITY_ANALYTICS_PLUGIN

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Analytics
{
	/// <summary>
	/// Simple dummy analytics provider that doesn't do anything, but makes sure projects don't break when they have no unity analytics implemented.
	/// Without this the <see cref="IAnalyticsProvider"/> will cause nullrefs (for example in the <see cref="BrainCloud.Login.LoginService"/>).
	/// </summary>
	public class DummyAnalyticsProvider : IAnalyticsProvider
	{
		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool Custom(string eventName, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool GameOver(IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool GameStart(IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool LevelComplete(string name, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool LevelFail(string name, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool LevelQuit(string name, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool LevelStart(string name, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		/// <returns>Always returns true.</returns>
		public bool ScreenVisit(string name, IDictionary<string, object> parameters = null)
		{
			return true;
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		public void SetSessionId(string sessionId)
		{
		}

		/// <summary>
		/// Dummy function without any logic.
		/// </summary>
		public void SetUserId(string userId)
		{
		}
	}
}
#endif // !UNITY_ANALYTICS_PLUGIN
