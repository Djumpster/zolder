// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// The Spot pointer uses a virtual sphere to detect
	/// any objects within the specified radius from the 
	/// specified origin.
	/// 
	/// Interacts with everything on the specified layers in <see cref="PointerInteractionService.LayerMask"/>.
	///  
	/// <para>
	/// Generally the resulting hit will be the object nearest
	/// to the origin.
	/// 
	/// For UI however it will contain the hit data of the
	/// UI element with the highest depth.
	/// </para>
	/// </summary>
	public interface IPointerSpot : IPointer
	{
		Vector3 Origin { get; }

		float Radius { get; }
	}
}
