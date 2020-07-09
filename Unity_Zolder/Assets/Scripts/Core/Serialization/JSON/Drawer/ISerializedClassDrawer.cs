// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization.Drawer
{
	/// <summary>
	/// <para>
	/// A custom SerializedClass drawer. When paired with the <see cref="SerializedClassDrawerAttribute"/>
	/// this can be used to draw a custom inspector for a specific type or attribute.
	/// </para>
	/// 
	/// <para>
	/// Usage is designed to be as close to native Unity property drawers as possible.
	/// For examples on how to use this, check out <see cref="FallbackDrawer"/>.
	/// </para>
	/// </summary>
	/// <seealso cref="FallbackDrawer"/>
	public interface ISerializedClassDrawer
	{
		object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute);
	}
}
