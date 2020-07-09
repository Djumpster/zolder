// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Basic implementation of a spot pointer.
	/// A spot pointer interacts with objects within
	/// the specified radius around it's origin.
	/// </summary>
	public class PointerSpot : PointerBase, IPointerSpot
	{
		public Vector3 Origin => Transform.position;

		public float Radius => radius;

		[SerializeField] private float radius = 0.1f;

		private Color debugColor;

		protected override void Awake()
		{
			base.Awake();

			debugColor = ColorExtensions.RandomHueColor;
		}

		protected void OnDrawGizmos()
		{
			if (Application.isPlaying && pointerService.IsDebugModeEnabled)
			{
				Gizmos.color = debugColor;
				Gizmos.DrawWireSphere(Origin, Radius);

				if (Hit.IsValid)
				{
					Gizmos.DrawLine(Origin, Hit.Point);
				}
			}
		}
	}
}
