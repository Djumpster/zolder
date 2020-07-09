// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// The Ray pointer draws a line from the specified origin
	/// into the specified direction.
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
	public interface IPointerRay : IPointer
	{
		Vector3 Origin { get; }
		Vector3 Direction { get; }
	}
}
