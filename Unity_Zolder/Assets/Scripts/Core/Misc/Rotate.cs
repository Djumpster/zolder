// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Rotates the attached transform using the given settings.
	/// </summary>
	public class Rotate : MonoBehaviour
	{
		public enum UpdateMethod
		{
			Update,
			LateUpdate
		}

		[SerializeField] private bool useUnscaledTime = false;
		[SerializeField] private Vector3 rotation;
		[SerializeField] private bool beginRandom = true;
		[SerializeField] private bool allowAddition = true;
		[SerializeField] private bool allowPausing = true;
		[SerializeField] private UpdateMethod updateMethod = UpdateMethod.LateUpdate;

		private Transform trans;
		private Vector3 addRotation;

		public bool Paused { get; set; }

		private void Awake()
		{
			trans = transform;
			if (beginRandom && rotation != Vector3.zero)
			{
				trans.Rotate(rotation.normalized * Random.Range(0, 360));
			}
		}

		public void Add(Vector3 euler)
		{
			if (allowAddition)
			{
				addRotation += euler;
			}
		}
		private void PerformUpdate()
		{
			float time = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			time = (Paused && allowPausing) ? 0f : time;
			trans.Rotate(rotation * time + addRotation);
			addRotation = Vector3.zero;
		}

		private void Update()
		{
			if (updateMethod == UpdateMethod.Update)
			{
				PerformUpdate();
			}
		}

		private void LateUpdate()
		{
			if (updateMethod == UpdateMethod.LateUpdate)
			{
				PerformUpdate();
			}
		}
	}
}