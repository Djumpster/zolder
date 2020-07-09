// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	public class UnityEventSystemService : IBootstrappable, ICrossSceneCoroutineContext
	{
		private readonly GameObject eventSystemObject;

		private const int DEFAULT_PIXEL_DRAG_THRESHOLD = 30;

		public UnityEventSystemService(GameObject eventSystemObject)
		{
			this.eventSystemObject = eventSystemObject;
			//Set dragthreshold based on the screens DPI.
			float dpi = Mathf.Approximately(Screen.dpi, 0f) ? 160f : Screen.dpi;
			int calculatedPixelDragThreshold = (int)(DEFAULT_PIXEL_DRAG_THRESHOLD * dpi / 160f);

			EventSystem.current.pixelDragThreshold = Mathf.Max(DEFAULT_PIXEL_DRAG_THRESHOLD, calculatedPixelDragThreshold);
		}

		public T AddInputModule<T>() where T : BaseInputModule
		{
			return eventSystemObject.AddComponent<T>();
		}

		public T RequireInputModule<T>() where T : BaseInputModule
		{
			return eventSystemObject.RequireComponent<T>();
		}

		public void RemoveInputModule<T>() where T : BaseInputModule
		{
			Component component = eventSystemObject.GetComponent<T>();
			Object.Destroy(component);
		}

		public T GetInputModule<T>() where T : BaseInputModule
		{
			return eventSystemObject.GetComponent<T>();
		}
	}
}
