// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Base implementation of the <see cref="IUpdater"/> class.<para/>
	/// 
	/// You can subscribe to a specific update mode to receive updates
	/// from either regular Update, LateUpdate or FixedUpdate.<para/>
	/// 
	/// This class was created because I kept running into the same
	/// problem where I wanted to be able to select the update method from the editor in several
	/// classes without having to write the same code over and over.<para/>
	/// 
	/// Now you can just create a <see cref="UpdateMode"/> variable in your
	/// class which you can edit it in the editor, then you inject a <see cref="IUpdater"/>
	/// in your class and subscribe to the selected <see cref="UpdateMode"/> with your desired callback.
	/// When you're done you call Unsubscribe with either the specific UpdateMode or just your callback.
	/// </summary>
	public class UpdaterService : IUpdater, IDisposable
	{
		protected struct Updatable
		{
			public readonly UpdateCallback UpdateCallback;
			public readonly bool Unscaled;

			public Updatable(UpdateCallback updateCallback, bool unscaled)
			{
				UpdateCallback = updateCallback;
				Unscaled = unscaled;
			}
		}

		private Dictionary<UpdateMode, List<Updatable>> updatables;
		private UnityCallbackService unityCallbackService;

		public UpdaterService(UnityCallbackService unityCallbackService)
		{
			this.unityCallbackService = unityCallbackService;
			updatables = new Dictionary<UpdateMode, List<Updatable>>();
		}

		/// <summary>
		/// Subscribes the given updateCallback to the given UpdateMode.
		/// </summary>
		/// <param name="updateMode"></param>
		/// <param name="updateCallback"></param>
		/// <param name="unscaledDeltaTime"></param>
		public void Subscribe(UpdateMode updateMode, UpdateCallback updateCallback,
			bool unscaledDeltaTime = false)
		{
			if (updatables.ContainsKey(updateMode))
			{
				if (!Contains(updateMode, updateCallback).contains)
				{
					updatables[updateMode].Add(new Updatable(updateCallback, unscaledDeltaTime));
				}
			}
			else
			{
				AddUpdateMode(updateMode);
				updatables[updateMode].Add(new Updatable(updateCallback, unscaledDeltaTime));
			}
		}

		/// <summary>
		/// Unsubscribe the callback from all update methods.
		/// </summary>
		/// <param name="updateCallback"></param>
		public void Unsubscribe(UpdateCallback updateCallback)
		{
			List<UpdateMode> emptyUpdateModes = new List<UpdateMode>();
			foreach (KeyValuePair<UpdateMode, List<Updatable>> kvp in updatables)
			{
				(bool contains, bool unscaled) contains = Contains(kvp.Key, updateCallback);
				if (contains.contains)
				{
					updatables[kvp.Key].Remove(new Updatable(updateCallback, contains.unscaled));
				}

				if (kvp.Value.Count == 0)
				{
					emptyUpdateModes.Add(kvp.Key);
				}
			}

			// Remove empty UpdateModes.
			foreach (UpdateMode updatemode in emptyUpdateModes)
			{
				RemoveUpdateMode(updatemode);
			}
		}

		public void Dispose()
		{
			foreach (var kvp in updatables)
			{
				switch (kvp.Key)
				{
					case UpdateMode.Update:
						unityCallbackService.UpdateEvent -= Update;
						break;
					case UpdateMode.LateUpdate:
						unityCallbackService.LateUpdateEvent -= LateUpdate;
						break;
					case UpdateMode.FixedUpdate:
						unityCallbackService.FixedUpdateEvent -= FixedUpdate;
						break;
				}
			}

			updatables.Clear();
		}

		private void Update()
		{
			if (updatables.ContainsKey(UpdateMode.Update))
			{
				foreach (Updatable updatable in updatables[UpdateMode.Update])
				{
					updatable.UpdateCallback?.Invoke(updatable.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);
				}
			}
		}

		private void LateUpdate()
		{
			if (updatables.ContainsKey(UpdateMode.LateUpdate))
			{
				foreach (Updatable updatable in updatables[UpdateMode.LateUpdate])
				{
					updatable.UpdateCallback?.Invoke(updatable.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);
				}
			}
		}

		private void FixedUpdate()
		{
			if (updatables.ContainsKey(UpdateMode.FixedUpdate))
			{
				foreach (Updatable updatable in updatables[UpdateMode.FixedUpdate])
				{
					updatable.UpdateCallback?.Invoke(updatable.Unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
				}
			}
		}

		private void AddUpdateMode(UpdateMode updateMode)
		{
			if (updatables.ContainsKey(updateMode))
			{
				return;
			}

			switch (updateMode)
			{
				case UpdateMode.Update:
					unityCallbackService.UpdateEvent += Update;
					break;
				case UpdateMode.LateUpdate:
					unityCallbackService.LateUpdateEvent += LateUpdate;
					break;
				case UpdateMode.FixedUpdate:
					unityCallbackService.FixedUpdateEvent += FixedUpdate;
					break;
			}

			updatables.Add(updateMode, new List<Updatable>());
		}

		private void RemoveUpdateMode(UpdateMode updateMode)
		{
			if (!updatables.ContainsKey(updateMode))
			{
				return;
			}

			switch (updateMode)
			{
				case UpdateMode.Update:
					unityCallbackService.UpdateEvent -= Update;
					break;
				case UpdateMode.LateUpdate:
					unityCallbackService.LateUpdateEvent -= LateUpdate;
					break;
				case UpdateMode.FixedUpdate:
					unityCallbackService.FixedUpdateEvent -= FixedUpdate;
					break;
			}

			updatables.Remove(updateMode);
		}

		private (bool contains, bool unscaled) Contains(UpdateMode updateMode, UpdateCallback updateCallback)
		{
			if (updatables.ContainsKey(updateMode))
			{
				if (updatables[updateMode].Contains(new Updatable(updateCallback, true)))
				{
					return (true, true);
				}
				else if (updatables[updateMode].Contains(new Updatable(updateCallback, false)))
				{
					return (true, false);
				}
				else
				{
					return (false, false);
				}
			}
			else
			{
				return (false, false);
			}
		}
	}
}