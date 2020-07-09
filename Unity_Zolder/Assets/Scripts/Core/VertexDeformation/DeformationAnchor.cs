// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.VertexDeformation
{
	/// <summary>
	/// Representation of a deformation anchor.
	/// Keeps track of the vertex index on the target mesh,
	/// the inital position calculated when the deformation anchor is added,
	/// and the assigned radius.
	/// </summary>
	[Serializable]
	public struct DeformationAnchor
	{
		[SerializeField] public int VertexIndex;
		[SerializeField] public Vector3 InitialVertexPosition;
		[SerializeField] public float Radius;
	}
}
