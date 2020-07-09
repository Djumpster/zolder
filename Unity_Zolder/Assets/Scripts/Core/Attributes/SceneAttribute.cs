// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Attributes
{
	/// <summary>
	/// Add this to a string or list of string to make the values a dropdown of scenes that have been added to the
	/// build settings.
	/// </summary>
	public class SceneAttribute : PropertyAttribute
	{
		public SceneAttribute()
		{
		}
	}
}