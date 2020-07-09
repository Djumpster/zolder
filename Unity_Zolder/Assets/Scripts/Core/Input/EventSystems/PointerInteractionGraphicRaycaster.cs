// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Cameras;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// A raycaster implementation that allows for UI interaction.
	/// You'd attach this to your main camera if you want:
	/// a) An UI object receiving UnityEvents (IPointerDown, IPointerClick, etc.)
	/// b) A pointer to collide with UI objects.
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class PointerInteractionGraphicRaycaster : BaseRaycaster
	{
		private struct GraphicRaycastHit
		{
			public Graphic Graphic;
			public Vector3 WorldPosition;
			public Vector3 ScreenPosition;
		};

		public enum BlockingObjects
		{
			None = 0,
			TwoD = 1,
			ThreeD = 2,
			All = 3,
		}

		private static List<GraphicRaycastHit> sortedGraphics = new List<GraphicRaycastHit>();

		public override Camera eventCamera => canvas.worldCamera ?? CameraController.MainCamera ?? Camera.main;

		public bool RayIgnoreReversedGraphics
		{
			set { rayIgnoreReversedGraphics = value; }
			get { return rayIgnoreReversedGraphics; }
		}

		public BlockingObjects RayBlockingObjects
		{
			set { rayBlockingObjects = value; }
			get { return rayBlockingObjects; }
		}

		private Canvas Canvas
		{
			get
			{
				if (canvas == null)
				{
					canvas = GetComponent<Canvas>();
				}

				return canvas;
			}
		}

		private ICameraController CameraController
		{
			get
			{
				if (cameraController == null)
				{
					cameraController = GlobalDependencyLocator.Instance.Get<ICameraController>();
				}

				return cameraController;
			}
		}

		[Header("Ray Pointer Source")]
		[SerializeField] private bool rayIgnoreReversedGraphics = true;
		[SerializeField] private BlockingObjects rayBlockingObjects = BlockingObjects.None;
		[SerializeField] private LayerMask rayBlockingMask = -1;

		private List<GraphicRaycastHit> raycastResults = new List<GraphicRaycastHit>();

		private ICameraController cameraController;
		private Canvas canvas;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (Canvas == null || Canvas.renderMode != RenderMode.WorldSpace)
			{
				return;
			}

			IList<Graphic> canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(Canvas);

			if (canvasGraphics == null || canvasGraphics.Count == 0)
			{
				return;
			}

			PointerInteractionEventData pointerSourceEventData = eventData as PointerInteractionEventData;
			if (pointerSourceEventData == null)
			{
				return;
			}

			switch (pointerSourceEventData.Type)
			{
				case PointerInteractionEventData.PointerType.Ray:
					RaycastPointerRay(pointerSourceEventData, canvasGraphics, resultAppendList);
					break;
				case PointerInteractionEventData.PointerType.Spot:
					RaycastPointerSpot(pointerSourceEventData, canvasGraphics, resultAppendList);
					break;
				default:
					throw new InvalidOperationException("Unsupported pointer type: " + pointerSourceEventData.Type + ", add an implementation to this class.");
			}
		}

		#region Pointer Ray Implementation
		private void RaycastPointerRay(PointerInteractionEventData eventData, IList<Graphic> canvasGraphics, List<RaycastResult> resultAppendList)
		{
			Camera currentEventCamera = eventCamera;

			// Calculate the maximum hit distance by checking for blocking objects
			Ray ray = new Ray(eventData.Origin, eventData.Direction);
			float hitDistance = float.MaxValue;

			if (RayBlockingObjects != BlockingObjects.None)
			{
				float distanceToClipPlane = eventCamera.farClipPlane;

				if (RayBlockingObjects == BlockingObjects.ThreeD || RayBlockingObjects == BlockingObjects.All)
				{
					RaycastHit[] hits = Physics.RaycastAll(ray, distanceToClipPlane, eventData.InteractionLayerMask);

					if (hits.Length > 0)
					{
						hitDistance = hits[0].distance;
					}
				}

				if (RayBlockingObjects == BlockingObjects.TwoD || RayBlockingObjects == BlockingObjects.All)
				{
					RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, distanceToClipPlane, eventData.InteractionLayerMask);

					if (hits.Length > 0)
					{
						hitDistance = hits[0].distance;
					}
				}
			}

			raycastResults.Clear();
			RaycastGraphics(Canvas, currentEventCamera, ray, eventData.InteractionLayerMask, canvasGraphics, raycastResults);

			for (int i = 0; i < raycastResults.Count; i++)
			{
				GameObject gameObject = raycastResults[i].Graphic.gameObject;

				if (RayIgnoreReversedGraphics)
				{
					Vector3 forward = ray.direction;
					Vector3 direction = gameObject.transform.rotation * Vector3.forward;

					if (Vector3.Dot(forward, direction) <= 0)
					{
						continue;
					}
				}

				float distance = Vector3.Distance(ray.origin, raycastResults[i].WorldPosition);

				if (distance >= hitDistance)
				{
					continue;
				}

				resultAppendList.Add(new RaycastResult
				{
					gameObject = gameObject,
					module = this,
					distance = distance,
					index = resultAppendList.Count,
					depth = raycastResults[i].Graphic.depth,
					sortingLayer = Canvas.sortingLayerID,
					sortingOrder = Canvas.sortingOrder,
					worldPosition = raycastResults[i].WorldPosition,
					screenPosition = raycastResults[i].ScreenPosition
				});
			}
		}

		private void RaycastGraphics(Canvas canvas, Camera eventCamera, Ray ray, LayerMask interactionLayerMask, IList<Graphic> foundGraphics, List<GraphicRaycastHit> results)
		{
			int totalCount = foundGraphics.Count;

			for (int i = 0; i < foundGraphics.Count; i++)
			{
				Graphic graphic = foundGraphics[i];

				if (graphic.depth == -1 || !graphic.raycastTarget || !interactionLayerMask.Contains(graphic.gameObject.layer) || graphic.canvasRenderer.cull)
				{
					continue;
				}

				Vector3 worldSpacePosition;
				if (RayIntersectsRectTransform(graphic.rectTransform, ray, out worldSpacePosition))
				{
					Vector2 screenPosition = eventCamera.WorldToScreenPoint(worldSpacePosition);

					if (graphic.Raycast(screenPosition, eventCamera))
					{
						sortedGraphics.Add(new GraphicRaycastHit
						{
							Graphic = graphic,
							WorldPosition = worldSpacePosition,
							ScreenPosition = screenPosition
						});
					}
				}
			}

			sortedGraphics.Sort((g1, g2) => g2.Graphic.depth.CompareTo(g1.Graphic.depth));

			for (int i = 0; i < sortedGraphics.Count; i++)
			{
				results.Add(sortedGraphics[i]);
			}

			sortedGraphics.Clear();
		}

		private bool RayIntersectsRectTransform(RectTransform rectTransform, Ray ray, out Vector3 worldSpacePosition)
		{
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);

			Plane plane = new Plane(corners[0], corners[1], corners[2]);

			float enter;
			if (!plane.Raycast(ray, out enter))
			{
				worldSpacePosition = Vector3.zero;
				return false;
			}

			Vector3 intersection = ray.GetPoint(enter);
			Vector3 BottomEdge = corners[3] - corners[0];
			Vector3 LeftEdge = corners[1] - corners[0];

			float BottomDot = Vector3.Dot(intersection - corners[0], BottomEdge);
			float LeftDot = Vector3.Dot(intersection - corners[0], LeftEdge);

			if (BottomDot < BottomEdge.sqrMagnitude && LeftDot < LeftEdge.sqrMagnitude && BottomDot >= 0 && LeftDot >= 0)
			{
				worldSpacePosition = corners[0] + LeftDot * LeftEdge / LeftEdge.sqrMagnitude + BottomDot * BottomEdge / BottomEdge.sqrMagnitude;
				return true;
			}

			worldSpacePosition = Vector3.zero;
			return false;
		}
		#endregion

		#region Pointer Spot Implementation
		private void RaycastPointerSpot(PointerInteractionEventData eventData, IList<Graphic> canvasGraphics, List<RaycastResult> resultAppendList)
		{
			Camera currentEventCamera = eventCamera;

			raycastResults.Clear();
			OverlapSphereGraphics(Canvas, currentEventCamera, eventData.Origin, eventData.Radius, eventData.InteractionLayerMask, canvasGraphics, raycastResults);

			for (int i = 0; i < raycastResults.Count; i++)
			{
				GameObject gameObject = raycastResults[i].Graphic.gameObject;

				resultAppendList.Add(new RaycastResult
				{
					gameObject = gameObject,
					module = this,
					distance = Vector3.Distance(eventData.Origin, gameObject.transform.position),
					index = resultAppendList.Count,
					depth = raycastResults[i].Graphic.depth,
					sortingLayer = Canvas.sortingLayerID,
					sortingOrder = Canvas.sortingOrder,
					worldPosition = raycastResults[i].WorldPosition,
					screenPosition = raycastResults[i].ScreenPosition
				});
			}
		}

		private void OverlapSphereGraphics(Canvas canvas, Camera eventCamera, Vector3 origin, float radius, LayerMask interactionLayerMask, IList<Graphic> foundGraphics, List<GraphicRaycastHit> results)
		{
			int totalCount = foundGraphics.Count;

			for (int i = 0; i < foundGraphics.Count; i++)
			{
				Graphic graphic = foundGraphics[i];

				if (graphic.depth == -1 || !graphic.raycastTarget || !interactionLayerMask.Contains(graphic.gameObject.layer) || graphic.canvasRenderer.cull)
				{
					continue;
				}

				Vector3 worldSpacePosition;
				if (OverlapSphereIntersectsRectTransform(graphic.rectTransform, origin, radius, out worldSpacePosition))
				{
					Vector2 screenPosition = eventCamera.WorldToScreenPoint(worldSpacePosition);

					if (graphic.Raycast(screenPosition, eventCamera))
					{
						sortedGraphics.Add(new GraphicRaycastHit
						{
							Graphic = graphic,
							WorldPosition = worldSpacePosition,
							ScreenPosition = screenPosition
						});
					}
				}
			}

			sortedGraphics.Sort((g1, g2) => g2.Graphic.depth.CompareTo(g1.Graphic.depth));

			for (int i = 0; i < sortedGraphics.Count; i++)
			{
				results.Add(sortedGraphics[i]);
			}

			sortedGraphics.Clear();
		}

		private bool OverlapSphereIntersectsRectTransform(RectTransform rectTransform, Vector3 origin, float radius, out Vector3 worldSpacePosition)
		{
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);

			Plane plane = new Plane(corners[0], corners[1], corners[2]);

			Vector3 closestPoint = plane.ClosestPointOnPlane(origin);
			float distanceToOrigin = (closestPoint - origin).sqrMagnitude;

			if (distanceToOrigin > radius * radius)
			{
				worldSpacePosition = Vector3.zero;
				return false;
			}

			Vector3 BottomEdge = corners[3] - corners[0];
			Vector3 LeftEdge = corners[1] - corners[0];

			float BottomDot = Vector3.Dot(closestPoint - corners[0], BottomEdge);
			float LeftDot = Vector3.Dot(closestPoint - corners[0], LeftEdge);

			if (BottomDot < BottomEdge.sqrMagnitude && LeftDot < LeftEdge.sqrMagnitude && BottomDot >= 0 && LeftDot >= 0)
			{
				worldSpacePosition = corners[0] + LeftDot * LeftEdge / LeftEdge.sqrMagnitude + BottomDot * BottomEdge / BottomEdge.sqrMagnitude;
				return true;
			}

			worldSpacePosition = Vector3.zero;
			return false;
		}
		#endregion
	}
}
