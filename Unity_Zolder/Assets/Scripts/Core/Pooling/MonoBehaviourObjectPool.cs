// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// A base class for poolable mono behaviour objects.
	/// T must derive from <see cref="MonoBehaviour"/>! It is not added as a generic constraint directly so you can 
	/// still create instances of this without knowing the exact implementing MonoBehaviour derived type at run-time.
	/// </summary>
	/// <typeparam name="T">The type of the MonoBehaviour derived script on the gameObject that implements IPoolableObject.</typeparam>
	public abstract class MonoBehaviourObjectPool<T> : ObjectPool<T> where T : IPoolableObject
	{
		#region members
		private GameObject _root;
		private GameObject root
		{
			get
			{
				if (_root == null)
				{
					_root = new GameObject
					{
						name = "Pool of " + hierarchyPoolContentsName
					};

					if (dontDestroyOnLoad)
					{
						Object.DontDestroyOnLoad(_root);
					}
				}
				return _root;
			}
		}

		private Transform _inactiveParent;
		private Transform inactiveParent
		{
			get
			{
				if (_inactiveParent == null)
				{
					GameObject inactiveGO = new GameObject
					{
						name = "Inactive"
					};
					inactiveGO.transform.SetParent(root.transform);
					_inactiveParent = inactiveGO.transform;
				}
				return _inactiveParent;
			}
		}

		private Transform _activeParent;
		private Transform activeParent
		{
			get
			{
				if (_activeParent == null)
				{
					GameObject activeGO = new GameObject
					{
						name = "Active"
					};
					activeGO.transform.SetParent(root.transform);
					_activeParent = activeGO.transform;
				}
				return _activeParent;
			}
		}

		private string hierarchyPoolContentsName;
		private bool dontDestroyOnLoad = false;
		private ICallbackService unityCallbackService;

		#endregion

		#region constructor
		protected MonoBehaviourObjectPool(ICallbackService unityCallbackService, int cullSize, string hierarchyPoolContentsName,
			bool dontDestroyOnLoad = true) : base(cullSize)
		{
			this.hierarchyPoolContentsName = hierarchyPoolContentsName;
			this.dontDestroyOnLoad = dontDestroyOnLoad;
			this.unityCallbackService = unityCallbackService;

			if (!dontDestroyOnLoad)
			{
				this.unityCallbackService.UpdateEvent += OnUpdateEvent;
			}
		}

		protected void OnUpdateEvent()
		{
			//GO's can be destroyed by all kinds of means, clean them up.
			for (int i = activeObjects.Count - 1; i >= 0; i--)
			{
				IPoolableObject poolable = activeObjects[i];
				MonoBehaviour monoBehaviour = activeObjects[i] as MonoBehaviour;
				if (monoBehaviour.gameObject == null)
				{
					poolable.ReturnToPoolHandler -= OnReturnToPoolCallback;
					poolable.DestroyForever();
					activeObjects.Remove(poolable);
				}
			}

			for (int i = inactiveObjects.Count - 1; i >= 0; i--)
			{
				IPoolableObject poolable = inactiveObjects[i];
				MonoBehaviour monoBehaviour = inactiveObjects[i] as MonoBehaviour;
				if (monoBehaviour.gameObject == null)
				{
					poolable.ReturnToPoolHandler -= OnReturnToPoolCallback;
					poolable.DestroyForever();
					inactiveObjects.Remove(poolable);
				}
			}
		}

		#endregion

		#region public methods
		public override void Dispose()
		{
			unityCallbackService.UpdateEvent -= OnUpdateEvent;
			unityCallbackService = null;
			CullSize = 0;
			Object.Destroy(root);
			base.Dispose();
		}

		public override T GetPoolableObject()
		{
			T instance = base.GetPoolableObject();
			MonoBehaviour monoBehaviour = instance as MonoBehaviour;
			monoBehaviour.transform.SetParent(activeParent, true);
			return instance;
		}

		public T GetPoolableObject(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			T instance = default(T);
			if (inactiveObjects.Count > 0)
			{
				instance = (T)inactiveObjects[0];
				inactiveObjects.RemoveAt(0);
			}
			else
			{
				LogUtil.Log(LogTags.GAME, GetType().Name + "-" + typeof(T).Name, "Pool size not big enough, " +
						   "grown by 1. Now: " + PoolSize + ", will cull back to size: " + cullSize);

				instance = GetNewInactivePoolableObject();
			}

			MonoBehaviour monoBehaviour = instance as MonoBehaviour;

			monoBehaviour.transform.parent = parent;
			monoBehaviour.transform.position = position;
			monoBehaviour.transform.rotation = rotation;
			monoBehaviour.transform.localScale = scale;

			instance.Reset();
			instance.BecomeActive();
			activeObjects.Add(instance);
			return instance;
		}

		public void DestroyActiveObjects()
		{
			for (int i = activeObjects.Count - 1; i >= 0; i--)
			{
				IPoolableObject poolable = activeObjects[i];
				MonoBehaviour monoBehaviour = activeObjects[i] as MonoBehaviour;
				if (monoBehaviour.gameObject)
				{
					poolable.ReturnToPoolHandler -= OnReturnToPoolCallback;
					poolable.DestroyForever();
				}
			}
			activeObjects.Clear();
		}
		#endregion

		#region private methods
		protected override T GetNewInactivePoolableObject()
		{
			T instance = base.GetNewInactivePoolableObject();
			MonoBehaviour monoBehaviour = instance as MonoBehaviour;
			monoBehaviour.transform.SetParent(inactiveParent, true);
			return instance;
		}

		protected override void OnReturnToPoolCallback(IPoolableObject obj)
		{
			base.OnReturnToPoolCallback(obj);
			MonoBehaviour monoBehaviour = obj as MonoBehaviour;
			if (monoBehaviour.gameObject && monoBehaviour.gameObject != null)
			{
				monoBehaviour.transform.SetParent(inactiveParent, true);
			}
		}

		#endregion
	}
}
