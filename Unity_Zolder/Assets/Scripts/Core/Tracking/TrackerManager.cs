// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// Manages stat trackers of type ITrackGameEvent
	/// </summary>
	public class TrackerManager
	{
		/// <summary>
		/// The trackers. Key == a string identifier.
		/// </summary>
		private Dictionary<string, ITrackGameEvent> trackers = new Dictionary<string, ITrackGameEvent>();

		public TrackerManager()
		{
		}

		public void RegisterTracker(string id, ITrackGameEvent tracker)
		{
			if (trackers.ContainsKey(id))
			{
				if (trackers[id] == tracker)
				{
					return;
				}
				else
				{
					LogUtil.Error(LogTags.ANALYTICS, this, "Cannot add tracker with id " + id + " because another tracker with that id already exists!");
				}
			}

			foreach (KeyValuePair<string, ITrackGameEvent> kvp in trackers)
			{
				if (kvp.Value == tracker)
				{
					LogUtil.Error(LogTags.ANALYTICS, this, "Cannot add tracker with id " + id + " because it was already registered with different id " + kvp.Key + " !");
				}
			}

			trackers.Add(id, tracker);
		}

		public ITrackGameEvent GetTracker(string id)
		{
			if (trackers.ContainsKey(id))
			{
				return trackers[id];
			}

			return null;
		}

		public T GetTracker<T>(string id) where T : ITrackGameEvent
		{
			if (trackers.ContainsKey(id))
			{
				ITrackGameEvent tracker = trackers[id];
				if (tracker is T)
				{
					return (T)trackers[id];
				}
			}

			return default(T);
		}

		public List<T> GetTrackers<T>() where T : ITrackGameEvent
		{
			List<T> trackerList = new List<T>();
			foreach (ITrackGameEvent tracker in trackers.Values)
			{
				if (tracker is T)
				{
					trackerList.Add((T)tracker);
				}
			}

			return trackerList;
		}

		public void UnregisterTracker(ITrackGameEvent tracker)
		{
			List<string> keys = new List<string>();

			foreach (KeyValuePair<string, ITrackGameEvent> kvp in trackers)
			{
				if (kvp.Value == tracker)
				{
					keys.Add(kvp.Key);
				}
			}

			foreach (string key in keys)
			{
				trackers.Remove(key);
			}
		}

		public void DestroyTracker(ITrackGameEvent tracker)
		{
			List<string> keys = new List<string>();

			foreach (KeyValuePair<string, ITrackGameEvent> kvp in trackers)
			{
				if (kvp.Value == tracker)
				{
					kvp.Value.Destroy();
					keys.Add(kvp.Key);
				}
			}

			foreach (string key in keys)
			{
				trackers.Remove(key);
			}
		}
	}
}
