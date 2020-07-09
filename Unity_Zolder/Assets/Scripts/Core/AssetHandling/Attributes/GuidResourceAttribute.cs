// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.AssetHandling
{
	/// <summary>
	/// Guid Resource attribute. Use this to tag strings as weak resource links.
	/// This attribute serializes the object by it's Guid instead of it's file path,
	/// which makes it robust against renaming and moving.
	/// </summary>
	public class GuidResourceAttribute : PropertyAttribute
	{
		public Type BaseType { get; }

		public GuidResourceAttribute(Type baseType)
		{
			BaseType = baseType;
		}
	}
}