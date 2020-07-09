// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.XR;

namespace Talespin.Core.Foundation.Misc
{
	public enum Platform
	{
		iOS,
		Android,
		AppleTV,
		Windows,
		Mac,
		Editor,
		Mobile,
		Standalone,
		AndroidVR,
		OculusRift,
		Vive,
		StandaloneNonVR
	}

	public static class PlatformIdentifier
	{
		public static bool Is(Platform platform)
		{
#if UNITY_TVOS
			if (platform == Platform.AppleTV)
			{
				return true;
			}
#elif UNITY_ANDROID

			if (platform == Platform.AndroidVR)
			{
				return XRSettings.enabled;
			}

			if (platform == Platform.Android)
			{
				return true;
			}

			if (platform == Platform.Mobile)
			{
				return true;
			}
#elif UNITY_IOS
			if (platform == Platform.iOS)
			{
				return true;
			}
			if (platform == Platform.Mobile)
			{
				return true;
			}
#elif UNITY_STANDALONE_OSX
			if(platform == Platform.StandaloneNonVR)
			{
				return !XRSettings.enabled;
			}
			if (platform == Platform.Mac)
			{
				return true;
			}
			if (platform == Platform.Standalone)
			{
				return true;
			}
#elif UNITY_STANDALONE_WIN
#if OCULUS_VR
			if (platform == Platform.OculusRift)
			{
				return XRSettings.enabled;
			}
#elif STEAM_VR
			if(platform == Platform.Vive)
			{
				return XRSettings.enabled;
			}
#endif
			if (platform == Platform.StandaloneNonVR)
			{
				return !XRSettings.enabled;
			}

			if (platform == Platform.Windows)
			{
				return true;
			}

			if (platform == Platform.Standalone)
			{
				return true;
			}
#elif UNITY_EDITOR
			if (platform == Platform.Editor)
			{
				return true;
			}
#endif
			return false;
		}
	}
}
