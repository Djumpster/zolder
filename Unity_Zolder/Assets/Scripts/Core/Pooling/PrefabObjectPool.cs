// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// A pool of objects based on loading a prefab.
	/// T must derive from <see cref="MonoBehaviour"/>! It is not added as a generic constraint directly so you can 
	/// still create instances of this without knowing the exact implementing MonoBehaviour derived type at run-time.
	/// </summary>
	/// <typeparam name="T">The type of the MonoBehaviour derived script on the gameObject that implements IPoolableObject.</typeparam>
	public class PrefabObjectPool<T> : MonoBehaviourObjectPool<T> where T : IPoolableObject
	{
		protected GameObject prefab;
		protected string prefabPath;

		public PrefabObjectPool
		(
			string prefabPath,
			ICallbackService unityCallbackService,
			int cullSize,
			string hierarchyPoolContentsName,
			bool dontDestroyOnLoad = true
		) : base(unityCallbackService, cullSize, hierarchyPoolContentsName, dontDestroyOnLoad)
		{
			prefab = Resources.Load<GameObject>(prefabPath);
		}

		public PrefabObjectPool(GameObject prefab, UnityCallbackService unityCallbackService, int cullSize,
			bool dontDestroyOnLoad = true) : base(unityCallbackService, cullSize, prefab.name, dontDestroyOnLoad)
		{
			this.prefab = prefab;
		}

		protected override T InstantiateNewPoolableObject()
		{
			bool prefabWasEnabled = prefab.activeSelf;
			// spawn disabled to prevent flickering
			prefab.SetActive(false);
			GameObject go = Object.Instantiate(prefab) as GameObject;
			prefab.SetActive(prefabWasEnabled);
			T poolableComponent = go.GetComponent<T>();
			return poolableComponent;
		}
	}
}