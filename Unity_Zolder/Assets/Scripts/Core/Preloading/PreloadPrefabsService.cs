// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Preloading
{
	/// <summary>
	/// Manages preloading of prefab instances. Can be used by things like the SpawnScopedPrefabsAction in the flow in
	/// combination with the PreloadPrefabsScopedAction.
	/// Internally uses the object pooling system. Add an IPoolableObject implementation to your prefab to customize
	/// pooling behaviour. If you preload a prefab that does not implement IPoolableObject then
	/// BasicPoolableMonoBehaviour is added.
	/// Prefabs are preloaded by context and amount. The pool size is determined by the context with the largest
	/// passed amount and will resize as contexts are added and removed. Once all contexts are removed for a prefab the
	/// pool will revert to size 0 and inactive instances are destroyed. Note that active instances still persist and
	/// will only be destroyed on being returned to the pool when its size is 0. If you add a context for the pool
	/// again then it may become used again.
	/// </summary>
	public class PreloadPrefabsService : IDisposable
	{
		private List<PooledPreloadedPrefabInstances> preloadedPrefabs = new List<PooledPreloadedPrefabInstances>();
		private UnityCallbackService unityCallbackService;

		public PreloadPrefabsService(UnityCallbackService unityCallbackService)
		{
			this.unityCallbackService = unityCallbackService;
		}

		/// <summary>
		/// Preloads the prefab with the given prefabGuid by creating an object pool with minAmount of instances in it.
		/// If a pool for the prefab already exists then the size is increased if it was less than minAmount.
		/// </summary>
		/// <param name="prefabGuid">The guid of the prefab.</param>
		/// <param name="minAmount">The minimum amount of instances in the preloaded object pool.</param>
		/// <param name="context">The context used to keep the pool and minAmount alive.</param>
		public void PreloadPrefab(string prefabGuid, int minAmount, object context)
		{
			PooledPreloadedPrefabInstances preloadData = GetOrCreatePreloadedPrefabData(prefabGuid);
			preloadData.AddContext(context, minAmount);
			preloadedPrefabs.Add(preloadData);
		}

		/// <summary>
		/// Preloads the prefab with the given prefabGuid by creating an object pool with minAmount of instances in it.
		/// If a pool for the prefab already exists then the size is increased if it was less than minAmount.
		/// </summary>
		/// <param name="prefab">The loaded resource (not an instance) of the prefab.</param>
		/// <param name="prefabGuid">The guid of the prefab.</param>
		/// <param name="minAmount">The minimum amount of instances in the preloaded object pool.</param>
		/// <param name="context">The context used to keep the pool and minAmount alive.</param>
		public void PreloadPrefab(GameObject prefab, string prefabGuid, int minAmount, object context)
		{
			PooledPreloadedPrefabInstances preloadData = GetOrCreatePreloadedPrefabData(prefab, prefabGuid);
			preloadData.AddContext(context, minAmount);
			preloadedPrefabs.Add(preloadData);
		}

		/// <summary>
		/// Attempts to unload the prefab pool for the given prefabGuid. It will not be unloaded if it still has other
		/// contexts keeping it alive or if you pass ignoreContexts as true. Note that only inactive instances will be
		/// removed.
		/// </summary>
		/// <param name="prefabGuid">The guid of the prefab.</param>
		/// <param name="context">The context used to keep the pool and minAmount alive. Passed when calling <see cref="PreloadPrefab"/>.</param>
		/// <param name="ignoreContexts">Should the other contexts keeping the pool alive be ignored and the pool be emptied regardless?</param>
		/// <returns></returns>
		public bool TryUnloadPrefab(string prefabGuid, object context, bool ignoreContexts = false)
		{
			PooledPreloadedPrefabInstances data = GetPreloadedPrefabData(prefabGuid);
			if (data == null || data.ContextCount == 0)
			{
				return true;
			}

			if (ignoreContexts)
			{
				data.RemoveContexts();
			}
			else
			{
				data.RemoveContext(context);
			}

			// Keep in mind removing a context does not destroy active instances, which can still return to the pool.
			// Therefore do not remove the data itself from preloadedPrefabs so it can be reused again when a context
			// is re-added.

			return data.ContextCount == 0;
		}

		/// <summary>
		/// Remove the context from all preloaded prefab pools that have it. They may decrease in size if this context
		/// had the largest minAmount, all the way down to 0 entries. Note that only inactive instances will be removed.
		/// </summary>
		/// <param name="context"></param>
		public void UnloadForContext(object context)
		{
			for (int i = preloadedPrefabs.Count - 1; i >= 0; i--)
			{
				PooledPreloadedPrefabInstances preloadedPrefab = preloadedPrefabs[i];
				if (preloadedPrefab.ContainsContext(context))
				{
					preloadedPrefab.RemoveContext(context);
				}
			}

			// Keep in mind removing a context does not destroy active instances, which can still return to the pool.
			// Therefore do not remove the data itself from preloadedPrefabs so it can be reused again when a context is re-added.
		}

		/// <summary>
		/// Unload all preloaded prefab instances, both active and inactive.
		/// </summary>
		public void UnloadAll()
		{
			foreach (PooledPreloadedPrefabInstances preloadedPrefab in preloadedPrefabs)
			{
				preloadedPrefab.Dispose(true);
			}

			preloadedPrefabs.Clear();
		}

		public bool HasPreloadedInstance(string prefabGuid)
		{
			return GetPreloadedPrefabData(prefabGuid) != null;
		}

		/// <summary>
		/// Retrieve a prefab instance with the given prefabGuid from the preloaded pool.
		/// </summary>
		/// <param name="prefabGuid">The guid of the prefab.</param>
		/// <returns>A pooled instance of the prefab with guid prefabGuid.</returns>
		public GameObject GetPreloadedInstance(string prefabGuid)
		{
			GameObject instance = null;
			PooledPreloadedPrefabInstances preloadData = GetPreloadedPrefabData(prefabGuid);

			if (preloadData != null)
			{
				instance = preloadData.GetInstance();
				instance.transform.SetParent(null);
			}

			return instance;
		}

		public void Dispose()
		{
			UnloadAll();
		}

		private PooledPreloadedPrefabInstances GetPreloadedPrefabData(string prefabGuid)
		{
			foreach (PooledPreloadedPrefabInstances preloadedPrefab in preloadedPrefabs)
			{
				if (preloadedPrefab.PrefabGuid == prefabGuid)
				{
					return preloadedPrefab;
				}
			}

			return null;
		}

		private PooledPreloadedPrefabInstances GetOrCreatePreloadedPrefabData(string prefabGuid)
		{
			PooledPreloadedPrefabInstances data = GetPreloadedPrefabData(prefabGuid);
			if (data != null)
			{
				return data;
			}

			data = new PooledPreloadedPrefabInstances(prefabGuid, unityCallbackService);
			preloadedPrefabs.Add(data);
			return data;
		}

		public PooledPreloadedPrefabInstances GetOrCreatePreloadedPrefabData(GameObject prefab, string prefabGuid)
		{
			PooledPreloadedPrefabInstances data = GetPreloadedPrefabData(prefabGuid);
			if (data != null)
			{
				return data;
			}

			data = new PooledPreloadedPrefabInstances(prefab, prefabGuid, unityCallbackService);
			preloadedPrefabs.Add(data);
			return data;
		}
	}
}
