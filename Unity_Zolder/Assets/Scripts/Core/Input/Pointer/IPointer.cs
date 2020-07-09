// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Base pointer interface. This interface does not contain
	/// any implementation specific properties.
	/// </summary>
	public interface IPointer
	{
		Transform Transform { get; }
		PointerHit Hit { get; }
		PointerInteractionEventData EventData { get; }
		bool IsActive { get; }

		LayerMask InteractionLayerMask { get; }

		string Tag { get; }

		int GUID { get; }

		void SetHit(PointerHit hit);

		void SetEventData(PointerInteractionEventData eventData);

		void SetPointerActive(bool isActive);
	}
}
