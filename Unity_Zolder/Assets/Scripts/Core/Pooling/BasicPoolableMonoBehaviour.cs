// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// The minimum required implementation for an IPoolableObject GameObject to be usable in an object pool.
	/// </summary>
	public class BasePoolableMonoBehaviour : MonoBehaviour, IPoolableObject
	{
		public bool IsInPool { get; protected set; } = true;

		public event Action<IPoolableObject> ReturnToPoolHandler;

		public virtual void Reset()
		{
		}

		public virtual void BecomeActive()
		{
			IsInPool = false;
			gameObject.SetActive(true);
		}

		public virtual void BecomeInactive()
		{
			gameObject.SetActive(false);
		}

		public virtual void ReturnToPool()
		{
			IsInPool = true;

			ReturnToPoolHandler(this);
		}

		public void DestroyForever()
		{
			Destroy(gameObject);
		}
	}
}
