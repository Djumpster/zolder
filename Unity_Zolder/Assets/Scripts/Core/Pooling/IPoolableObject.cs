// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Pooling
{
	/// <summary>
	/// Interface for an object to be pooled by ObjectPool<T>.
	/// </summary>
	public interface IPoolableObject
	{
		/// <summary>
		/// Is the object in the pool or is it being used?
		/// Use this as an "isDestroyed" check.
		/// </summary>
		/// <value><see langword="true" /> if this instance is in the object pool; otherwise, <see langword="false" />.</value>
		bool IsInPool { get; }

		/// <summary>
		/// Callback used by the pooling system to return the object to the pool.
		/// </summary>
		event Action<IPoolableObject> ReturnToPoolHandler;

		/// <summary>
		/// Reset to a start state from which the object can become active again as if it were the first time.
		/// </summary>
		void Reset();

		/// <summary>
		/// Become active, when taken out of the inactive pool and becoming active.
		/// </summary>
		void BecomeActive();

		/// <summary>
		/// Become inactive, in a state that can be kept in an inactive pool of objects.
		/// </summary>
		void BecomeInactive();

		/// <summary>
		/// Return this object to the pool, making it inactive.
		/// </summary>
		void ReturnToPool();

		/// <summary>
		/// Completely and utterly destroy this object, it will no longer be pooled.
		/// Not simply called Destroy in case of MonoBehaviour.
		/// </summary>
		void DestroyForever();
	}
}