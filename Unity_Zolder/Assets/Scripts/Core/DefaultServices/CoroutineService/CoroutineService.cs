// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Pooling;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	/// <summary>
	/// Allows you to use Coroutine functionality in other classes than MonoBehaviours and runs them in a managed, 
	/// viewable environment. Uses object pooling.
	/// A context object can be passed to group coroutines together. They can then all be stopped at once by using
	/// StopContext(), for example.
	/// </summary>
	public class CoroutineService : IDisposable, ICoroutineService
	{
		private Dictionary<object, CoroutineServiceHelper> contextLookup = new Dictionary<object, CoroutineServiceHelper>();
		private PrefabObjectPool<CoroutineServiceHelper> dontDestroyPool = null;
		private PrefabObjectPool<CoroutineServiceHelper> pool = null;
		private bool hasBeenDisposed = false;
		private int counter = 0;

		public CoroutineService(ICallbackService unityCallbackService)
		{
			if (!Application.isPlaying)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Attempted to create new instance while application is not playing.");
				return;
			}
			dontDestroyPool = new PrefabObjectPool<CoroutineServiceHelper>("CoroutineHelper", unityCallbackService,
				50, "CoroutineHelper (cross-scene)");
			dontDestroyPool.FillPool(50);
			pool = new PrefabObjectPool<CoroutineServiceHelper>("CoroutineHelper", unityCallbackService, 50,
				"CoroutineHelper");
			pool.FillPool(50);
		}

		public Coroutine StartCoroutine(IEnumerator coroutine, object context, string contextName = "")
		{
			if (hasBeenDisposed)
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Attempting to StartCoroutine but this service has been disposed!");
				return null;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return null;
			}
#endif

			counter++;
			EnsureContext(context, contextName);
			return contextLookup[context].StartCoroutine(coroutine);
		}

		public void StartContext(object context)
		{
			if (hasBeenDisposed)
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Attempting to StartContext but this service has been disposed!");
				return;
			}

			EnsureContext(context);
		}

		public bool HasContext(object context)
		{
			return contextLookup.ContainsKey(context) && contextLookup[context];
		}

		public void StopCoroutine(object context, IEnumerator coroutine)
		{
			if (HasContext(context))
			{
				contextLookup[context].StopCoroutine(coroutine);
			}
			else
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Can't stop context <" + context.GetType() +
					"> because it could not be found!");
			}
		}

		public void StopCoroutine(object context, Coroutine coroutine)
		{
			if (HasContext(context))
			{
				contextLookup[context].StopCoroutine(coroutine);
			}
			else
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Can't stop context <" + context.GetType() +
					"> because it could not be found!");
			}
		}

		public void StopContext(object context)
		{
			if (HasContext(context))
			{
				contextLookup[context].StopAllCoroutines();
			}
			else
			{
				//LogUtil.Warning(LogTags.SYSTEM, this, "Can't stop context <" + context.GetType() + "> because it could not be found!");
			}
		}

		public void Dispose()
		{
			if (hasBeenDisposed)
			{
				return;
			}

			hasBeenDisposed = true;

			contextLookup.Clear();

			dontDestroyPool.Dispose();
			dontDestroyPool = null;

			pool.Dispose();
			pool = null;
		}

		private void EnsureContext(object context, string contextName = "")
		{
			if (contextLookup.ContainsKey(context) && !contextLookup[context])
			{
				contextLookup.Remove(context);
			}
			if (!contextLookup.ContainsKey(context))
			{
				bool crossScene = context is ICrossSceneCoroutineContext;

				CoroutineServiceHelper helper = null;
				if (crossScene)
				{
					helper = dontDestroyPool.GetPoolableObject();
				}
				else
				{
					helper = pool.GetPoolableObject();
				}

				helper.AllCoroutinesCompletedEvent += AllCoroutinesCompletedEvent; ;
				helper.DestroyedEvent += DestroyedEvent;

				helper.gameObject.name = counter + ". " +
					(crossScene ? "[CROSS SCENE] " : "") +
					"Coroutine Runner <" +
					context.ToString() +
					(!string.IsNullOrEmpty(contextName) ? " " + contextName + " " : "") +
					">";

				contextLookup.Add(context, helper);
			}
		}

		private void DestroyedEvent(CoroutineServiceHelper helper)
		{
			helper.AllCoroutinesCompletedEvent -= AllCoroutinesCompletedEvent; ;
			helper.DestroyedEvent -= DestroyedEvent;

			foreach (KeyValuePair<object, CoroutineServiceHelper> kvp in contextLookup)
			{
				if (kvp.Value == helper)
				{
					contextLookup.Remove(kvp.Key);
					return;
				}
			}
		}

		private void AllCoroutinesCompletedEvent(CoroutineServiceHelper helper)
		{
			helper.AllCoroutinesCompletedEvent -= AllCoroutinesCompletedEvent;
			helper.DestroyedEvent -= DestroyedEvent;

			foreach (KeyValuePair<object, CoroutineServiceHelper> kvp in contextLookup)
			{
				if (kvp.Value == helper)
				{
					contextLookup.Remove(kvp.Key);
					break;
				}
			}

			helper.ReturnToPool();
		}
	}
}