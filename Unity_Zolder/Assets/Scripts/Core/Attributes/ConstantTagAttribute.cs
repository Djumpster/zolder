// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Attributes
{
	/// <summary>
	/// Lets you draw a dropdown menu containing the values of the given types of type constType 
	/// which are exposed as constants in the given types.
	/// Note: only a few types are currently supported; string and int. Expand the editor script to introduce more.
	/// </summary>
	public class ConstantTagAttribute : PropertyAttribute
	{
		public readonly Type ConstType;
		public readonly Type[] Types;

		public ConstantTagAttribute(Type constType, params Type[] types)
		{
			ConstType = constType;
			Types = types;
		}
	}
}
