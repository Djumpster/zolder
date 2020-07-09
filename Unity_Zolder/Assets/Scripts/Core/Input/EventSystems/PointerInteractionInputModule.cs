// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// A custom InputModule implementation that's designed for <see cref="IPointer"/>s.
	/// 
	/// An instance of this InputModule should automatically be created by <see cref="UnityEventSystemServiceFactory"/>.
	/// If your app doesn't make use of the automatically-created EventSystem, make sure to manually add it.
	/// 
	/// This InputModule will by default only activate when there is atleast one registered <see cref="IPointer"/>.
	/// </summary>
	public class PointerInteractionInputModule : StandaloneInputModule
	{
		private Dictionary<int, PointerInteractionEventData> pointerData;

		[SerializeField] private bool enableDebugMode;

		[Header("Ray Pointer")]
		[Tooltip("Perform an sphere cast to determine correct depth for the pointer")]
		[SerializeField] private bool enableSphereCastForRayPointerSources;
		[SerializeField] private float raySpherecastRadius = 0.001f;

		private PointerInteractionService pointerService;

		private bool lastEnableDebugMode;

		protected override void Awake()
		{
			base.Awake();

			pointerService = GlobalDependencyLocator.Instance.Get<PointerInteractionService>();

			pointerData = new Dictionary<int, PointerInteractionEventData>();
		}

		protected virtual void Update()
		{
			if (enableDebugMode != lastEnableDebugMode)
			{
				lastEnableDebugMode = enableDebugMode;
				pointerService.IsDebugModeEnabled = enableDebugMode;
			}

			if (pointerService.IsDebugModeEnabled != enableDebugMode)
			{
				enableDebugMode = pointerService.IsDebugModeEnabled;
				lastEnableDebugMode = enableDebugMode;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(base.ToString());

			foreach (KeyValuePair<int, PointerInteractionEventData> pointer in pointerData)
			{
				if (pointer.Value == null)
				{
					continue;
				}

				sb.AppendLine("<B>Pointer:</b> " + pointer.Key);
				sb.AppendLine(pointer.Value.ToString());
			}

			return sb.ToString();
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();

			foreach (PointerInteractionEventData eventData in pointerData.Values)
			{
				eventData.pointerEnter = null;
			}
		}

		public override void Process()
		{
			base.Process();

			foreach (IPointer pointer in pointerService.Pointers)
			{
				if(!pointer.IsActive)
				{
					continue;
				}

				PointerInteractionEventData eventData = GetPointerSourceEventData(pointer, out bool pressed, out bool released, out Vector2 scrollDelta);

				ProcessPress(eventData, pressed, released);
				ProcessMove(eventData);
				ProcessScroll(eventData, scrollDelta);

				if (!pressed)
				{
					ProcessDrag(eventData);
				}
			}
		}

		public override bool IsPointerOverGameObject(int pointerGUID)
		{
			if (base.IsPointerOverGameObject(pointerGUID))
			{
				return true;
			}

			foreach (KeyValuePair<int, PointerInteractionEventData> kvp in pointerData)
			{
				if (kvp.Key != pointerGUID)
				{
					continue;
				}

				return kvp.Value.pointerEnter != null;
			}

			return false;
		}

		public GameObject GetGameObjectUnderPointer(int pointerGUID)
		{
			if (!IsPointerOverGameObject(pointerGUID))
			{
				return null;
			}

			PointerEventData data = GetLastPointerEventData(pointerGUID);

			if (data != null)
			{
				return data.pointerCurrentRaycast.gameObject;
			}

			return null;
		}

		public override bool IsModuleSupported()
		{
			return forceModuleActive || (base.IsModuleSupported() || pointerService.NumRegisteredPointers > 0);
		}

		private void ProcessMove(PointerInteractionEventData eventData)
		{
			GameObject targetGameObject = eventData.pointerCurrentRaycast.gameObject;
			HandlePointerExitAndEnter(eventData, targetGameObject);
		}

		private void ProcessScroll(PointerInteractionEventData eventData, Vector2 scrollDelta)
		{
			GameObject currentOverGo = eventData.pointerCurrentRaycast.gameObject;

			eventData.scrollDelta = scrollDelta;

			if(eventData.scrollDelta != Vector2.zero)
			{
				ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.scrollHandler);
			}
		}

		private void ProcessPress(PointerInteractionEventData eventData, bool pressed, bool released)
		{
			GameObject currentOverGo = eventData.pointerCurrentRaycast.gameObject;

			// PointerDown notification
			if (pressed)
			{
				eventData.eligibleForClick = true;
				eventData.delta = Vector2.zero;
				eventData.dragging = false;
				eventData.useDragThreshold = true;
				eventData.pressPosition = eventData.position;
				eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

				if (eventData.pointerEnter != currentOverGo)
				{
					// send a pointer enter to the touched element if it isn't the one to select...
					HandlePointerExitAndEnter(eventData, currentOverGo);
					eventData.pointerEnter = currentOverGo;
				}

				// search for the control that will receive the press
				// if we can't find a press handler set the press
				// handler to be what would receive a click.
				GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

				// didnt find a press handler... search for a click handler
				if (newPressed == null)
				{
					newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
				}

				float time = Time.unscaledTime;

				if (newPressed == eventData.lastPress)
				{
					float diffTime = time - eventData.clickTime;

					if (diffTime < 0.3f)
					{
						++eventData.clickCount;
					}
					else
					{
						eventData.clickCount = 1;
					}

					eventData.clickTime = time;
				}
				else
				{
					eventData.clickCount = 1;
				}

				eventData.pointerPress = newPressed;
				eventData.rawPointerPress = currentOverGo;

				eventData.clickTime = time;

				// Save the drag handler as well
				eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

				if (eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
				}
			}

			// PointerUp notification
			if (released)
			{
				ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

				// see if we mouse up on the same element that we clicked on...
				GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

				// PointerClick and Drop events
				if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
				{
					ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
				}
				else if (eventData.pointerDrag != null && eventData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
				}

				eventData.eligibleForClick = false;
				eventData.pointerPress = null;
				eventData.rawPointerPress = null;

				if (eventData.pointerDrag != null && eventData.dragging)
				{
					ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
				}

				eventData.dragging = false;
				eventData.pointerDrag = null;

				if (eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
				}

				eventData.pointerDrag = null;
			}
		}

		private void ProcessDrag(PointerInteractionEventData eventData)
		{
			if (eventData.pointerDrag == null)
			{
				return;
			}

			if (!eventData.dragging && ShouldStartDrag(eventData.pressPosition, eventData.position, eventSystem.pixelDragThreshold, eventData.useDragThreshold))
			{
				ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
				eventData.dragging = true;
			}

			// Drag notification
			if (eventData.dragging)
			{
				// Before doing drag we should cancel any pointer down state
				// And clear selection!
				if (eventData.pointerPress != eventData.pointerDrag)
				{
					ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

					eventData.eligibleForClick = false;
					eventData.pointerPress = null;
					eventData.rawPointerPress = null;
				}

				ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
			}
		}

		private bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
		{
			if (!useDragThreshold)
			{
				return true;
			}

			return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
		}

		private void PopulatePointerRayFromEventData(IPointerRay pointer, PointerInteractionEventData eventData)
		{
			eventData.Origin = pointer.Origin;
			eventData.Direction = pointer.Direction;

			eventSystem.RaycastAll(eventData, m_RaycastResultCache);

			RaycastResult raycast = FindFirstRaycast(m_RaycastResultCache);
			eventData.pointerCurrentRaycast = raycast;

			if (raycast.module)
			{
				eventData.position = raycast.screenPosition;

				if (raycast.module is PointerInteractionGraphicRaycaster)
				{
					pointer.SetHit(new PointerHit
					{
						GameObject = raycast.gameObject,
						Point = raycast.worldPosition,
						Normal = raycast.gameObject.transform.forward,
						Distance = raycast.distance
					});

					pointer.SetEventData(eventData);
				}
				else if (raycast.module is PointerInteractionPhysicsRaycaster)
				{
					if (enableSphereCastForRayPointerSources)
					{
						List<RaycastResult> results = new List<RaycastResult>();

						PointerInteractionPhysicsRaycaster physicsRaycaster = (PointerInteractionPhysicsRaycaster)raycast.module;
						physicsRaycaster.Spherecast(eventData, results, raySpherecastRadius);

						if (results.Count > 0 && results[0].distance < raycast.distance)
						{
							raycast = results[0];
							eventData.pointerCurrentRaycast = raycast;
						}
					}

					pointer.SetHit(new PointerHit
					{
						GameObject = raycast.gameObject,
						Point = raycast.worldPosition,
						Normal = raycast.worldNormal,
						Distance = raycast.distance
					});

					eventData.position = raycast.screenPosition;
					pointer.SetEventData(eventData);
				}
			}
			else
			{
				pointer.SetHit(new PointerHit());
				pointer.SetEventData(eventData);
			}
		}

		private void PopulatePointerSpotFromEventData(IPointerSpot pointer, PointerInteractionEventData eventData)
		{
			eventData.Origin = pointer.Origin;
			eventData.Radius = pointer.Radius;

			eventSystem.RaycastAll(eventData, m_RaycastResultCache);

			RaycastResult raycast = FindFirstRaycast(m_RaycastResultCache);
			eventData.pointerCurrentRaycast = raycast;

			if (raycast.module)
			{
				if (raycast.module is PointerInteractionGraphicRaycaster || raycast.module is PointerInteractionPhysicsRaycaster)
				{
					Vector3 normal = raycast.worldNormal;

					if (normal == Vector3.zero && raycast.gameObject)
					{
						normal = raycast.gameObject.transform.forward;
					}

					eventData.position = raycast.screenPosition;

					pointer.SetHit(new PointerHit
					{
						GameObject = raycast.gameObject,
						Point = raycast.worldPosition,
						Normal = normal,
						Distance = raycast.distance
					});

					pointer.SetEventData(eventData);
				}
			}
			else
			{
				pointer.SetHit(new PointerHit());
				pointer.SetEventData(eventData);
			}
		}

		protected virtual void GetControllerInteractionState(IPointer pointer, out bool pressed, out bool released, out Vector2 scrollDelta)
		{
			pressed = false;
			released = false;
			scrollDelta = Vector2.zero;
		}

		private PointerInteractionEventData GetPointerSourceEventData(IPointer pointer, out bool pressed, out bool released, out Vector2 scrollDelta)
		{
			PointerInteractionEventData eventData;
			GetOrCreatePointerSourceEventData(pointer, out eventData, true);

			eventData.Reset();
			eventData.button = PointerEventData.InputButton.Left;

			GetControllerInteractionState(pointer, out pressed, out released, out scrollDelta);

			switch (eventData.Type)
			{
				case PointerInteractionEventData.PointerType.Ray:
					PopulatePointerRayFromEventData(pointer as IPointerRay, eventData);
					break;
				case PointerInteractionEventData.PointerType.Spot:
					PopulatePointerSpotFromEventData(pointer as IPointerSpot, eventData);
					break;
				default:
					throw new InvalidOperationException("Unsupported pointer type: " + eventData.Type + ", add support for this pointer type in this class.");
			}

			if (pressed)
			{
				eventSystem.SetSelectedGameObject(pointer.Hit.GameObject);
			}

			m_RaycastResultCache.Clear();
			return eventData;
		}

		private bool GetOrCreatePointerSourceEventData(IPointer pointer, out PointerInteractionEventData eventData, bool create)
		{
			if (!pointerData.TryGetValue(pointer.GUID, out eventData) && create)
			{
				eventData = new PointerInteractionEventData(eventSystem)
				{
					pointerId = pointer.GUID,
					Type = GetPointerType(pointer),
					InteractionLayerMask = pointer.InteractionLayerMask,
					Tag = pointer.Tag
				};

				pointerData.Add(pointer.GUID, eventData);
				return true;
			}

			return false;
		}

		private PointerInteractionEventData.PointerType GetPointerType(IPointer pointer)
		{
			if (pointer is IPointerRay)
			{
				return PointerInteractionEventData.PointerType.Ray;
			}

			if (pointer is IPointerSpot)
			{
				return PointerInteractionEventData.PointerType.Spot;
			}

			return PointerInteractionEventData.PointerType.Invalid;
		}
	}
}
