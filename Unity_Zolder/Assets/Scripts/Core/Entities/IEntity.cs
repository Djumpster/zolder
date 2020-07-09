// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// Serves as a handle to the root of a (gameObject) entity that can be described with multiple tags.
	/// Use this to look up any kind of game entity by the defined tags.
	/// </summary>
	public interface IEntity : IDisposable
	{
		#region properties
		IEnumerable<string> Tags { get; }
		bool Disposed { get; }
		#endregion

		#region public methods
		bool HasTag(string tag);
		bool HasAll(List<string> tagset);
		bool HasAll(params string[] tagset);
		bool HasAny(List<string> tagset);
		bool HasAny(params string[] tagset);
		#endregion
	}
}