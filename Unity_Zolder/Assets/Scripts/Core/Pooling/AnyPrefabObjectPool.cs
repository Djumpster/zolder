// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// A pool for a prefab. If the prefab includes an IPoolableObject implementation then that will be used, otherwise
	/// the BasicPoolableMonoBehaviour will be added when spawning.
	/// </summary>
	public class AnyPrefabObjectPool : PrefabObjectPool<IPoolableObject>
	{
		public AnyPrefabObjectPool
		(
			string handleLocation,
			ICallbackService unityCallbackService,
			int cullSize,
			string hierarchyPoolContentsName,
			bool dontDestroyOnLoad = true)
			: base(handleLocation, unityCallbackService, cullSize, hierarchyPoolContentsName, dontDestroyOnLoad)
		{
		}

		public AnyPrefabObjectPool(GameObject gameObjectHandle,
			UnityCallbackService unityCallbackService,
			int cullSize,
			bool dontDestroyOnLoad = true)
			: base(gameObjectHandle, unityCallbackService, cullSize, dontDestroyOnLoad)
		{
		}

		protected override IPoolableObject InstantiateNewPoolableObject()
		{
			bool prefabWasEnabled = prefab.activeSelf;
			// spawn disabled to prevent flickering
			prefab.SetActive(false);
			GameObject go = Object.Instantiate(prefab) as GameObject;
			prefab.SetActive(prefabWasEnabled);

			IPoolableObject poolableComponent = go.GetComponent<IPoolableObject>();
			if (poolableComponent == null)
			{
				poolableComponent = go.AddComponent<BasePoolableMonoBehaviour>();
			}
			return poolableComponent;
		}
	}
}