// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// Identity provider for Android devices.
	/// </summary>
	public class AndroidDeviceIdentityProvider : IDeviceIdentityProvider
	{
		/// <inheritdoc/>
		public string Type { get; }

		/// <inheritdoc/>
		public string Serial { get; }

		public AndroidDeviceIdentityProvider()
		{
			AndroidJavaClass c = new AndroidJavaClass("android.os.Build");
			string serial = c.GetStatic<string>("SERIAL");

			if(string.IsNullOrEmpty(serial) || serial == "unknown")
			{
				Debug.LogWarning("Device does not have a serial!");
			}

			Serial = serial;
			Type = "android";

#if OCULUS_VR
			Type += "-oculus-quest";
#endif
		}
	}
}
