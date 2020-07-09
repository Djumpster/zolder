// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Serialization.Drawer
{
	/// <summary>
	/// <para>
	/// This attribute is used to specify a custom drawer used by <see cref="EditorSerializedClass{T}"/>.
	/// In addition to this attribute your drawer also needs to implement <see cref="ISerializedClassDrawer"/>.
	/// </para>
	/// 
	/// <para>
	/// This attribute only specifies what type and optionally attribute type the drawer is for.
	/// </para>
	/// </summary>
	/// <seealso cref="ISerializedClassDrawer"/>
	[AttributeUsage(AttributeTargets.Class)]
	public class SerializedClassDrawerAttribute : PropertyAttribute
	{
		public Type TargetType;
		public Type TargetAttributeType;
	}
}
