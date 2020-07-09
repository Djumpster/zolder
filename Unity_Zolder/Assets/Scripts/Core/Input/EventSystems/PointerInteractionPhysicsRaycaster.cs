// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// A raycaster implementation that allows for pointer interaction with 3D objects.
	/// You'd attach this to your main camera if you want:
	/// a) A 3D object receiving UnityEvents (IPointerDown, IPointerClick, etc.)
	/// b) A pointer to collide with 3D objects.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class PointerInteractionPhysicsRaycaster : BaseRaycaster
	{
		private Camera _eventCamera;
		public override Camera eventCamera
		{
			get
			{
				if (_eventCamera == null)
				{
					_eventCamera = GetComponent<Camera>();
				}

				return _eventCamera;
			}
		}

		public virtual int Depth => (int)eventCamera.depth;

		public int MaxIntersections
		{
			set { maxIntersections = value; }
			get { return maxIntersections; }
		}

		[SerializeField] private int maxIntersections;

		private RaycastHit[] rayHits;
		private Collider[] sphereHits;

		private PointerInteractionService pointerService;

		private int lastMaxIntersections;

		protected override void Awake()
		{
			base.Awake();

			pointerService = GlobalDependencyLocator.Instance.Get<PointerInteractionService>();
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			PointerInteractionEventData pointerSourceEventData = eventData as PointerInteractionEventData;
			if (pointerSourceEventData == null)
			{
				return;
			}

			switch (pointerSourceEventData.Type)
			{
				case PointerInteractionEventData.PointerType.Ray:
					RaycastPointerRay(pointerSourceEventData, resultAppendList);
					break;
				case PointerInteractionEventData.PointerType.Spot:
					RaycastPointerSpot(pointerSourceEventData, resultAppendList);
					break;
				default:
					throw new InvalidOperationException("Unsupported pointer type: " + pointerSourceEventData.Type + ", add an implementation to to this class.");
			}
		}

		public void Spherecast(PointerEventData eventData, List<RaycastResult> resultAppendList, float radius)
		{
			PointerInteractionEventData pointerSourceEventData = eventData as PointerInteractionEventData;
			if (pointerSourceEventData == null)
			{
				return;
			}

			switch (pointerSourceEventData.Type)
			{
				case PointerInteractionEventData.PointerType.Ray:
					SpherecastPointerRay(pointerSourceEventData, resultAppendList, radius);
					break;
				default:
					throw new InvalidOperationException("Unsupported pointer type: " + pointerSourceEventData.Type + ", add an implementation to to this class.");
			}
		}
		
		#region Pointer Ray Implementation
		private void RaycastPointerRay(PointerInteractionEventData eventData, List<RaycastResult> resultAppendList)
		{
			Ray ray = new Ray(eventData.Origin, eventData.Direction);

			float distance = eventCamera.farClipPlane - eventCamera.nearClipPlane;

			int hitCount = 0;

			if (maxIntersections == 0)
			{
				rayHits = Physics.RaycastAll(ray, distance, eventData.InteractionLayerMask);
				hitCount = rayHits.Length;
			}
			else
			{
				if (maxIntersections != lastMaxIntersections)
				{
					rayHits = new RaycastHit[maxIntersections];
					lastMaxIntersections = maxIntersections;
				}

				hitCount = Physics.RaycastNonAlloc(ray, rayHits, distance, eventData.InteractionLayerMask);
			}

			if (hitCount > 1)
			{
				Array.Sort(rayHits, (r1, r2) => r1.distance.CompareTo(r2.distance));
			}

			for (int i = 0; i < hitCount; ++i)
			{
				RaycastResult result = new RaycastResult
				{
					gameObject = rayHits[i].collider.gameObject,
					module = this,
					distance = rayHits[i].distance,
					index = resultAppendList.Count,
					worldPosition = rayHits[i].point,
					worldNormal = rayHits[i].normal,
					screenPosition = eventCamera.WorldToScreenPoint(rayHits[i].point),
					sortingLayer = 0,
					sortingOrder = 0
				};

				resultAppendList.Add(result);
			}
		}

		private void SpherecastPointerRay(PointerInteractionEventData eventData, List<RaycastResult> resultAppendList, float radius)
		{
			Ray ray = new Ray(eventData.Origin, eventData.Direction);

			float distance = eventCamera.farClipPlane - eventCamera.nearClipPlane;

			int hitCount = 0;

			if (maxIntersections == 0)
			{
				rayHits = Physics.SphereCastAll(ray, radius, distance, eventData.InteractionLayerMask);
				hitCount = rayHits.Length;
			}
			else
			{
				if (maxIntersections != lastMaxIntersections)
				{
					rayHits = new RaycastHit[maxIntersections];
					lastMaxIntersections = maxIntersections;
				}

				hitCount = Physics.SphereCastNonAlloc(ray, radius, rayHits, distance, eventData.InteractionLayerMask);
			}

			if (hitCount > 1)
			{
				Array.Sort(rayHits, (r1, r2) => r1.distance.CompareTo(r2.distance));
			}

			for (int i = 0; i < hitCount; ++i)
			{
				RaycastResult result = new RaycastResult
				{
					gameObject = rayHits[i].collider.gameObject,
					module = this,
					distance = rayHits[i].distance,
					index = resultAppendList.Count,
					worldPosition = rayHits[i].point,
					worldNormal = rayHits[i].normal,
					screenPosition = eventCamera.WorldToScreenPoint(rayHits[i].point),
					sortingLayer = 0,
					sortingOrder = 0
				};
				resultAppendList.Add(result);
			}
		}
		#endregion

		#region Pointer Spot Implementation
		private void RaycastPointerSpot(PointerInteractionEventData eventData, List<RaycastResult> resultAppendList)
		{
			int hitCount = 0;

			if (maxIntersections == 0)
			{
				sphereHits = Physics.OverlapSphere(eventData.Origin, eventData.Radius, eventData.InteractionLayerMask);
				hitCount = sphereHits.Length;
			}
			else
			{
				if (maxIntersections != lastMaxIntersections)
				{
					sphereHits = new Collider[maxIntersections];
					lastMaxIntersections = maxIntersections;
				}

				hitCount = Physics.OverlapSphereNonAlloc(eventData.Origin, eventData.Radius, sphereHits, eventData.InteractionLayerMask);
			}

			if (hitCount > 1)
			{
				Array.Sort(sphereHits, (r1, r2) =>
				{
					float d1 = Vector3.Distance(eventData.Origin, r1.transform.position);
					float d2 = Vector3.Distance(eventData.Origin, r2.transform.position);
					return d1.CompareTo(d2);
				});
			}

			for (int i = 0; i < hitCount; ++i)
			{
				RaycastResult result = new RaycastResult
				{
					gameObject = sphereHits[i].gameObject,
					module = this,
					distance = Vector3.Distance(eventData.Origin, sphereHits[i].transform.position),
					index = resultAppendList.Count,
					worldPosition = sphereHits[0].transform.position,
					worldNormal = sphereHits[0].transform.forward,
					screenPosition = eventData.position,
					sortingLayer = 0,
					sortingOrder = 0
				};

				resultAppendList.Add(result);
			}
		}
		#endregion
	}
}
