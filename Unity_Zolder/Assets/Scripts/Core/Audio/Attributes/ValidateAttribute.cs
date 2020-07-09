// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public abstract class ValidateAttribute : PropertyAttribute
	{
#if UNITY_EDITOR
		public abstract bool Validate(UnityEditor.SerializedProperty property);
#endif
	}
}