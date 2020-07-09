// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// A simple data container for animation event parameters
	/// </summary>
	[System.Serializable]
	public class AnimationEventParameterContainer
	{
		public float FloatParameter => floatParameter;
		[SerializeField] private float floatParameter = 0f;

		public int IntParameter => intParameter;
		[SerializeField] private int intParameter = 0;

		public string StringParameter => stringParameter;
		[SerializeField] private string stringParameter = "";

		public Object ObjectParameter => objectParameter;
		[SerializeField] private Object objectParameter = null;

		public AnimationEventParameterContainer(float floatParameter = 0, int intParameter = 0, string stringParameter = "", Object objectParameter = null)
		{
			this.floatParameter = floatParameter;
			this.intParameter = intParameter;
			this.stringParameter = stringParameter;
			this.objectParameter = objectParameter;
		}
	}
}
