// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Pooling;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	public class CoroutineServiceHelper : MonoBehaviour, IPoolableObject
	{
		#region events
		public event Action<IPoolableObject> ReturnToPoolHandler;
		public event Action<CoroutineServiceHelper> AllCoroutinesCompletedEvent;
		public event Action<CoroutineServiceHelper> DestroyedEvent;

		#endregion

		#region properties
		public bool IsInPool { get; private set; } = true;

		#endregion

		#region members
		private int runningRoutines = 0;
		private Dictionary<Coroutine, IEnumerator> coroutines = new Dictionary<Coroutine, IEnumerator>();

		#endregion

		#region public methods
		public void Reset()
		{
			StopAllCoroutines();
		}

		public void BecomeActive()
		{
			IsInPool = false;
			gameObject.SetActive(true);
		}

		public void BecomeInactive()
		{
			gameObject.SetActive(false);
		}

		public void ReturnToPool()
		{
			IsInPool = true;

			DestroyedEvent?.Invoke(this);

			ReturnToPoolHandler(this);
		}

		public void DestroyForever()
		{
			Destroy(gameObject);
		}

		#endregion

		#region Unity callbacks & overrides
		new public Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return base.StartCoroutine(PerformCoroutine(coroutine));
		}

		new public void StopCoroutine(Coroutine coroutine)
		{
			base.StopCoroutine(coroutine);

			if (coroutines.ContainsKey(coroutine))
			{
				coroutines.Remove(coroutine);
				runningRoutines--;
				CheckAllRoutinesComplete();
			}
		}

		new public void StopCoroutine(IEnumerator coroutine)
		{
			base.StopCoroutine(coroutine);

			if (coroutines.ContainsValue(coroutine))
			{
				List<Coroutine> activeCoroutines = new List<Coroutine>();
				foreach (KeyValuePair<Coroutine, IEnumerator> kvp in coroutines)
				{
					if (kvp.Value == coroutine)
					{
						activeCoroutines.Add(kvp.Key);
					}
				}

				foreach (Coroutine activeCoroutine in activeCoroutines)
				{
					coroutines.Remove(activeCoroutine);
					runningRoutines--;
				}

				CheckAllRoutinesComplete();
			}
		}

		new public void StopAllCoroutines()
		{
			coroutines.Clear();

			if (this != null || gameObject)
			{
				base.StopAllCoroutines();
			}

			runningRoutines = 0;
			CheckAllRoutinesComplete();
		}

		protected void OnDestroy()
		{
			DestroyedEvent?.Invoke(this);
		}

		#endregion

		#region private methods
		private void CheckAllRoutinesComplete()
		{
			if (AllCoroutinesCompletedEvent != null && runningRoutines == 0)
			{
				AllCoroutinesCompletedEvent(this);
			}
		}

		protected IEnumerator PerformCoroutine(IEnumerator coroutine)
		{
			runningRoutines++;
			Coroutine routine = base.StartCoroutine(coroutine);
			// if instantly completes
			if (routine == null)
			{
				runningRoutines--;
				CheckAllRoutinesComplete();
				yield break;
			}

			coroutines.Add(routine, coroutine);
			yield return routine;
			runningRoutines--;
			coroutines.Remove(routine);
			CheckAllRoutinesComplete();
		}

		#endregion
	}
}