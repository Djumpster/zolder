// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.UI
{
	/// <summary>
	/// A utility class to manage canvas visibility states. This will toggle
	/// the visibility of the specified canvas and manage the enabled state
	/// of all child raycasters. If a raycaster is enabled at the time of disabling,
	/// it will disable the raycaster and restore it when interactivity is restored.
	/// If a raycaster is already disabled it will not manage that raycaster.
	/// </summary>
	public static class CanvasInteractivityUtility
	{
		private static HashSet<BaseRaycaster> disabledRaycasters;
		private static HashSet<BaseRaycaster> raycastersToRemove;

		static CanvasInteractivityUtility()
		{
			disabledRaycasters = new HashSet<BaseRaycaster>();
			raycastersToRemove = new HashSet<BaseRaycaster>();
		}

		/// <summary>
		/// Set the interactivity of the specified canvas and manage the enabled state
		/// of all child raycasters. If a raycaster is enabled at the time of disabling,
		/// it will disable the raycaster and restore it when interactivity is restored.
		/// If a raycaster is already disabled it will not manage that raycaster.
		/// </summary>
		/// <param name="canvas">The root canvas to manage</param>
		/// <param name="interactive">Whether the canvas and its child raycasters should be enabled</param>
		public static void SetInteractivity(Canvas canvas, bool interactive)
		{
			// If the canvas exists apply toggle logic, otherwise only run the cleanup
			// because it's likely that this is the SetInteractivity(canvas, true) call
			// and we have (now) invalid raycasters in memory
			if (canvas)
			{
				// Early out to optimize, note that this prevents raycasters from being
				// disabled if the canvas is already disabled.
				if (canvas.enabled == interactive)
				{
					return;
				}

				BaseRaycaster[] raycasters = canvas.transform.GetComponentsInChildren<BaseRaycaster>();

				foreach (BaseRaycaster raycaster in raycasters)
				{
					if (interactive)
					{
						if (disabledRaycasters.Contains(raycaster))
						{
							disabledRaycasters.Remove(raycaster);
							raycaster.enabled = true;
						}
					}
					else
					{
						if (raycaster.isActiveAndEnabled)
						{
							raycaster.enabled = false;
							disabledRaycasters.Add(raycaster);
						}
					}
				}

				canvas.enabled = interactive;
			}

			// clean up the disabled raycasters collection to prevent leaks
			if (disabledRaycasters.Count > 0)
			{
				foreach (BaseRaycaster raycaster in disabledRaycasters)
				{
					if (!raycaster)
					{
						raycastersToRemove.Add(raycaster);
					}
				}

				foreach (BaseRaycaster raycaster in raycastersToRemove)
				{
					disabledRaycasters.Remove(raycaster);
				}

				raycastersToRemove.Clear();
			}
		}
	}
}
