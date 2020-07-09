// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Serialization
{
	/// <summary>
	/// A simple attribute to override <code>Type.Name</code> information.
	/// It is up to the implementee of the system showing the type name to
	/// implement support for this though.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TypeNameAttribute : Attribute
	{
		public string Name { get; }

		public TypeNameAttribute(string name)
		{
			Name = name;
		}
	}
}
