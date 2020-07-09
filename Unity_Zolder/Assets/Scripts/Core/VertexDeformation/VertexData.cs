// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.VertexDeformation
{
	/// <summary>
	/// Representation of a single vertex.
	/// Keeps track of the vertex index on the source mesh,
	/// and the initial position calculated when the component is
	/// added, or the source mesh changed.
	/// </summary>
	[Serializable]
	public struct VertexData
	{
		[SerializeField] public int VertexIndex;
		[SerializeField] public Vector3 InitialVertexPosition;
	}
}
