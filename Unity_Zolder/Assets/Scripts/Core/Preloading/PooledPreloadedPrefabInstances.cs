// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Pooling;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Preloading
{
	/// <summary>
	/// Handles preloading prefabs by using a pool and lets you retrieve instances and manage the pool size by using contexts.
	/// The pool will always have the size of the largest context. If contexts are removed or added the pool resizes.
	/// If no more contexts are active then the pool is set to cull size 0 and will empty itself as instances are
	/// returned to the pool.
	/// </summary>	
	public class PooledPreloadedPrefabInstances
	{
		public string PrefabGuid { get; private set; }
		public GameObject Prefab { get; private set; }
		public int ContextCount => contextsWithAmounts.Count;
		public IEnumerable<object> Contexts => contextsWithAmounts.Keys;

		private Dictionary<object, int> contextsWithAmounts = new Dictionary<object, int>();

		private AnyPrefabObjectPool prefabPool;

		public PooledPreloadedPrefabInstances(string prefabGuid, UnityCallbackService callbackService)
		{
			PrefabGuid = prefabGuid;
			Prefab = prefabGuid.LoadGuidIfAvailable<GameObject>();
			prefabPool = new AnyPrefabObjectPool(Prefab, callbackService, 0, true);
		}

		public PooledPreloadedPrefabInstances(GameObject prefab, string prefabGuid, UnityCallbackService callbackService)
		{
			PrefabGuid = prefabGuid;
			Prefab = prefab;
			prefabPool = new AnyPrefabObjectPool(Prefab, callbackService, 0, true);
		}

		public void AddContext(object context, int minAmount)
		{
			if (contextsWithAmounts.ContainsKey(context))
			{
				if (contextsWithAmounts[context] <= minAmount)
				{
					contextsWithAmounts[context] = minAmount;
				}
			}
			else
			{
				contextsWithAmounts.Add(context, minAmount);
			}

			if (prefabPool.CullSize < minAmount)
			{
				prefabPool.CullSize = minAmount;
				prefabPool.FillPool(minAmount);
			}
		}

		public bool ContainsContext(object context)
		{
			return contextsWithAmounts.ContainsKey(context);
		}

		public void RemoveContext(object context)
		{
			if (!contextsWithAmounts.ContainsKey(context))
			{
				return;
			}
			contextsWithAmounts.Remove(context);

			int minAmount = 0;
			foreach (KeyValuePair<object, int> kvp in contextsWithAmounts)
			{
				if (kvp.Value > minAmount)
				{
					minAmount = kvp.Value;
				}
			}

			prefabPool.CullSize = minAmount;
		}

		public void RemoveContexts()
		{
			// this will basically make this pool inactive without destroying currently active pooled objects.
			// you can make it active again by adding a context.
			contextsWithAmounts.Clear();
			prefabPool.CullSize = 0;
		}

		public void Dispose(bool destroyInstances = false)
		{
			prefabPool.CullSize = 0;
			prefabPool.Dispose();
			if (destroyInstances)
			{
				prefabPool.DestroyActiveObjects();
			}
			prefabPool = null;
		}

		public GameObject GetInstance()
		{
			MonoBehaviour monoBehaviour = prefabPool.GetPoolableObject() as MonoBehaviour;
			return monoBehaviour.gameObject;
		}
	}
}
