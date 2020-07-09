// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Interface for something that has a position and a facing.
	/// </summary>
	public interface IPositioned
	{
		string Name { get; }
		Vector3 Position { get; }
		Vector3 Forward { get; }
		Vector3 Right { get; }
		Vector3 Up { get; }
	}
}