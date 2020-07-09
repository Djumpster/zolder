// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Basic implementation of a Ray pointer.
	/// </summary>
	public class PointerRay : PointerBase, IPointerRay
	{
		public Vector3 Origin => Transform.position;

		public Vector3 Direction => Transform.forward;

		private Color debugColor;
		private LineRenderer debugRay;

		protected override void Awake()
		{
			base.Awake();

			debugColor = ColorExtensions.RandomHueColor;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (!IsRegistered())
			{
				return;
			}

			if (pointerService.IsDebugModeEnabled && debugRay == null)
			{
				debugRay = Transform.gameObject.RequireComponent<LineRenderer>();
				debugRay.useWorldSpace = false;
				debugRay.SetPositions(new Vector3[2] { Vector3.zero, new Vector3(0, 0, 100f) });
				debugRay.material = new Material(Shader.Find("Unlit/Color"))
				{
					color = debugColor
				};
				debugRay.startWidth = 0.025f;
				debugRay.endWidth = 0.025f;
			}
			else if (!pointerService.IsDebugModeEnabled && debugRay != null)
			{
				Destroy(debugRay);
				debugRay = null;
			}
		}

		public override void SetHit(PointerHit hit)
		{
			base.SetHit(hit);

			if (debugRay != null)
			{
				debugRay.SetPositions(new Vector3[2] { Vector3.zero, new Vector3(0, 0, hit.IsValid ? hit.Distance : 100) });
			}
		}
	}
}
