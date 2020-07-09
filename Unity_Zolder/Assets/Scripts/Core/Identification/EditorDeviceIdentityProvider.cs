// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// Device identity provider for use within the Unity Editor.
	/// </summary>
	public class EditorDeviceIdentityProvider : IDeviceIdentityProvider
	{
		/// <inheritdoc/>
		public string Type { get; }

		/// <inheritdoc/>
		public string Serial { get; }

		public EditorDeviceIdentityProvider()
		{
#if UNITY_EDITOR_WIN
			Type = "windows";
#elif UNITY_EDITOR_OSX
			Type = "macos";
#elif UNITY_EDITOR_LINUX
			Type = "linux";
#else
			Type = "unknown";
#endif

			Type += "-unity-editor";
			Serial = "unity-editor";
		}
	}
}
