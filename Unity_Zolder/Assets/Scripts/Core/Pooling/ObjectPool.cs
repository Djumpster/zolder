// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// Base class for a pool of objects.
	/// </summary>
	public abstract class ObjectPool<T> where T : IPoolableObject
	{
		#region properties
		public int PoolSize { get { return activeObjects.Count + inactiveObjects.Count; } }
		public int CullSize
		{
			get { return cullSize; }
			set
			{
				cullSize = value;
				while (PoolSize > cullSize && inactiveObjects.Count > 0)
				{
					IPoolableObject instance = inactiveObjects[0];
					inactiveObjects.RemoveAt(0);
					instance.ReturnToPoolHandler -= OnReturnToPoolCallback;
					instance.DestroyForever();
				}
			}
		}
		#endregion

		#region members
		protected List<IPoolableObject> activeObjects = new List<IPoolableObject>();
		protected List<IPoolableObject> inactiveObjects = new List<IPoolableObject>();

		protected int cullSize = 0;
		#endregion

		#region constructor
		protected ObjectPool(int cullSize)
		{
			this.cullSize = cullSize;
		}
		#endregion

		#region public methods
		public virtual void Dispose()
		{
			foreach (IPoolableObject obj in activeObjects)
			{
				obj.ReturnToPoolHandler -= OnReturnToPoolCallback;
				obj.DestroyForever();
			}

			foreach (IPoolableObject obj in inactiveObjects)
			{
				obj.ReturnToPoolHandler -= OnReturnToPoolCallback;
				obj.DestroyForever();
			}

			activeObjects.Clear();
			inactiveObjects.Clear();
		}

		public void FillPool(int size)
		{
			while (inactiveObjects.Count + activeObjects.Count < size)
			{
				inactiveObjects.Add(GetNewInactivePoolableObject());
			}
		}

		public void DeactivateAllActiveObjects()
		{
			foreach (IPoolableObject item in new List<IPoolableObject>(activeObjects))
			{
				item.ReturnToPool();
			}
		}

		public void EmptyInactivePool()
		{
			if (inactiveObjects.Count > 0)
			{
				for (int i = inactiveObjects.Count - 1; i >= 0; i--)
				{
					inactiveObjects[i].ReturnToPoolHandler -= OnReturnToPoolCallback;
					inactiveObjects[i].DestroyForever();
				}
				inactiveObjects.Clear();
			}
		}

		public virtual T GetPoolableObject()
		{
			T instance = default(T);
			if (inactiveObjects.Count > 0)
			{
				instance = (T)inactiveObjects[0];
				inactiveObjects.RemoveAt(0);
			}
			else
			{
				instance = GetNewInactivePoolableObject();
			}

			instance.Reset();
			instance.BecomeActive();
			activeObjects.Add(instance);
			return instance;
		}
		#endregion

		#region private methods
		protected virtual T GetNewInactivePoolableObject()
		{
			T instance = InstantiateNewPoolableObject();
			instance.BecomeInactive();
			instance.ReturnToPoolHandler += OnReturnToPoolCallback;
			return instance;
		}

		protected abstract T InstantiateNewPoolableObject();

		protected virtual void OnReturnToPoolCallback(IPoolableObject obj)
		{
			if (inactiveObjects.Contains(obj))
			{
				return;
			}

			activeObjects.Remove(obj);

			if (PoolSize > cullSize)
			{
				obj.ReturnToPoolHandler -= OnReturnToPoolCallback;
				obj.DestroyForever();
				return;
			}

			obj.BecomeInactive();
			inactiveObjects.Add(obj);
		}
		#endregion
	}
}