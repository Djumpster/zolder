// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Audio
{
	public class ValidateNotNull : ValidateAttribute
	{
#if UNITY_EDITOR
		public override bool Validate(UnityEditor.SerializedProperty property)
		{
			if (property.propertyType == UnityEditor.SerializedPropertyType.String)
			{
				return !string.IsNullOrEmpty(property.stringValue);
			}
			else
			{
				return property.objectReferenceValue != null;
			}
		}
#endif
	}
}