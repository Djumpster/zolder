// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_IOS
using UnityEngine.iOS;
using Talespin.Core.Foundation.Logging;
#endif

using Talespin.Core.Foundation.Graphics;

namespace Talespin.Core.Foundation.Settings
{
	public class ReflectionsSettingsService : SettingsService<ReflectionsSettingsService.ReflectionMode>
	{
		public enum ReflectionMode : int
		{
			OFF = 0,
			ON = 1
		}

		private readonly PlanarReflectionService planarReflectionService;

		public ReflectionsSettingsService(PlanarReflectionService planarReflectionService) : base(false)
		{
			this.planarReflectionService = planarReflectionService;
			ApplyMode((ReflectionMode)mode);
		}

		protected override string Identifier
		{
			get
			{
				return "set_reflections";
			}
		}

		private ReflectionMode defaultMode;

		protected override ReflectionMode DefaultValue
		{
			get
			{
				defaultMode = AutoDetectQualitySettings();
				return defaultMode;
			}
		}

		protected override void ApplyMode(ReflectionMode mode)
		{
			planarReflectionService.EnableReflections = mode == ReflectionMode.ON ? true : false;
		}

		private ReflectionMode AutoDetectQualitySettings()
		{
			ReflectionMode result = ReflectionMode.ON;

#if UNITY_IOS
			LogUtil.Log(LogTags.SYSTEM, this, "Device generation: " + Device.generation);

			switch (Device.generation)
			{
				// at some point we can require arm64 / opengles-3 or metal to exclude devices.
				// Device usage stats: http://hwstats.unity3d.com/mobile/device-ios.html
				// Performance stats: https://browser.primatelabs.com/ios-benchmarks
				// Max memory stats: http://stackoverflow.com/questions/5887248/ios-app-maximum-memory-budget 

				// iPhone
				case DeviceGeneration.iPhone3G: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPhone3GS: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPhone4: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPhone4S: // NO arm64, opengles-3, metal
				
				// iPad
				case DeviceGeneration.iPad1Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPad2Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPad3Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPadMini1Gen: // NO arm64, opengles-3, metal
						
				// iPod Touch
				case DeviceGeneration.iPodTouch1Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPodTouch2Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPodTouch3Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPodTouch4Gen: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPodTouch5Gen:
					result = ReflectionMode.OFF;
					break;

				// iPhone
				case DeviceGeneration.iPhone5: // NO arm64, opengles-3, metal
				case DeviceGeneration.iPhone5C: // NO arm64, opengles-3, metal
			
				// iPad
				case DeviceGeneration.iPadAir1:
				case DeviceGeneration.iPadMini2Gen:
				case DeviceGeneration.iPadAir2:
				case DeviceGeneration.iPadMini3Gen:
				case DeviceGeneration.iPadMini4Gen:
				case DeviceGeneration.iPadPro1Gen:
				case DeviceGeneration.iPadPro10Inch1Gen:

				case DeviceGeneration.iPad4Gen: // NO arm64, opengles-3, metal
					result = ReflectionMode.OFF; 
					break;

				// iPhone
				case DeviceGeneration.iPhone5S:
				case DeviceGeneration.iPhone6:
				case DeviceGeneration.iPhone6Plus:
				case DeviceGeneration.iPhone6S:
				case DeviceGeneration.iPhone6SPlus:
				case DeviceGeneration.iPhoneSE1Gen:

					result = ReflectionMode.ON;
					break;

					// iPhone
				case DeviceGeneration.iPhone7:
				case DeviceGeneration.iPhone7Plus:

				// Unknown (future proof)
				case DeviceGeneration.iPhoneUnknown:
				case DeviceGeneration.iPodTouchUnknown:
				case DeviceGeneration.iPadUnknown:
				case DeviceGeneration.Unknown:
				default:
					result = ReflectionMode.ON;
					break;
			}

#elif !ENABLE_VR && UNITY_ANDROID
            result = ReflectionMode.ON;
#elif ENABLE_VR && UNITY_ANDROID
			result = ReflectionMode.ON;
#endif

			return result;
		}
	}
}
