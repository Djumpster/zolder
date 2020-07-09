// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Talespin.Core.Foundation.Settings
{
	public enum GraphicsMode : int
	{
		LOW = 0,
		MEDIUM = 1,
		HIGH = 2,
		CUSTOM = 4
	}

	public class GraphicsSettingsService : SettingsService<GraphicsMode>
	{
		protected override GraphicsMode DefaultValue => GetOptimizedQualitySettingsForDevice();
		protected override string Identifier => "set_graphic";

		private Dictionary<GraphicsMode, Vector2> modeResolutions = new Dictionary<GraphicsMode, Vector2>
		{
			{ GraphicsMode.LOW, new Vector2(800, 480)},
			{ GraphicsMode.MEDIUM, new Vector2(1280, 720)},
		};

		private readonly IEventDispatcher eventSystem;
		private readonly NativeResolutionService nativeResolutionService;

		private Action<GraphicsMode> overrideChangeHandler;

		public GraphicsSettingsService(IEventDispatcher eventSystem, NativeResolutionService nativeResolutionService) : base(false)
		{
			this.eventSystem = eventSystem;
			this.nativeResolutionService = nativeResolutionService;

			ApplyMode((GraphicsMode)mode);
		}

		public void SetOverrideChangeHandler(Action<GraphicsMode> handler)
		{
			overrideChangeHandler = handler;
			overrideChangeHandler?.Invoke(CurrentSetting);
		}

		public void ClearOverrideChangeHandler()
		{
			SetOverrideChangeHandler(null);
		}

		protected override void ApplyMode(GraphicsMode mode)
		{
			if (overrideChangeHandler != null)
			{
				overrideChangeHandler.Invoke(mode);
			}
			else
			{
				DefaultGraphicsModeChangedHandler(mode);
			}

			// TEMP FIX: In VR (on Oculus / Gear VR) changing the Screen.Resolution will cause the game to go black
			// after regaining focus.
			// TODO: Replace with new Unity render scale feature?
#if !UNITY_STANDALONE && !ENABLE_VR
			OptimizeResolution(mode);
#endif

			eventSystem.Invoke(new GraphicsSettingsChangedEvent(mode, Shader.globalMaximumLOD));

			LogUtil.Log(LogTags.GRAPHICS, this, "Applied graphics mode " + mode);
		}

		private void OptimizeResolution(GraphicsMode mode)
		{
			Vector2 targetResolution = GetResolutionForGraphicsMode(mode);

			float widthRatio = nativeResolutionService.Width / targetResolution.x;
			float heightRatio = nativeResolutionService.Height / targetResolution.y;

			float lowest = Mathf.Min(widthRatio, heightRatio);
			float frac = Mathf.Clamp(lowest, 1f, 2f);

			int newWidth = Mathf.FloorToInt((nativeResolutionService.Width) / frac);
			int newHeight = Mathf.FloorToInt((nativeResolutionService.Height) / frac);

			LogUtil.Log(LogTags.GRAPHICS, this, "Setting screen resolution to " + newWidth + " x " + newHeight);

			Screen.SetResolution(newWidth, newHeight, true);
		}

		private void DefaultGraphicsModeChangedHandler(GraphicsMode mode)
		{
			switch (mode)
			{
				case GraphicsMode.LOW:
					QualitySettings.SetQualityLevel(0);
					Shader.globalMaximumLOD = 300;
					break;
				case GraphicsMode.MEDIUM:
					QualitySettings.SetQualityLevel(1);
					Shader.globalMaximumLOD = 400;
					break;
				case GraphicsMode.HIGH:
					QualitySettings.SetQualityLevel(2);
					Shader.globalMaximumLOD = 500;
					break;
				case GraphicsMode.CUSTOM:
					QualitySettings.SetQualityLevel(4);
					Shader.globalMaximumLOD = 1000;
					break;
			}
		}

		private Vector2 GetResolutionForGraphicsMode(GraphicsMode mode)
		{
			Vector2 v;

			if (modeResolutions.ContainsKey(mode))
			{
				v = modeResolutions[mode];
			}
			else
			{
				v = new Vector2(nativeResolutionService.Width, nativeResolutionService.Height);
			}

			float width = Screen.orientation == ScreenOrientation.Landscape ? v.x : v.y;
			float height = Screen.orientation == ScreenOrientation.Landscape ? v.y : v.x;
			return new Vector2(width, height);
		}

		public static bool IsGalaxyS6(string deviceModel)
		{
			string parsedDeviceModel = deviceModel;

			// Testing for the specific device numbers used by samsung
			// example: SystemInfo.deviceModel for Galaxy S8 is: "samsung SM-G950F"
			if (parsedDeviceModel.StartsWith("samsung "))
			{
				parsedDeviceModel = parsedDeviceModel.Remove(0, "samsung ".Length);
			}
			// the last bit of the device model string for Samsung are variations of the same models for different providers,
			// which mostly aren't relevant for quality settings, so strip the irrelevant part.
			parsedDeviceModel = parsedDeviceModel.Substring(0, parsedDeviceModel.Length >= 7 ? 7 : parsedDeviceModel.Length);

			switch (parsedDeviceModel)
			{
				case "SM-G920": // Galaxy S6
				case "SM-G925": // Galaxy S6 Edge
				case "SM-G928": // Galaxy S6 Edge Plus
				case "SM-N920": // Galaxy Note 5. Including this as being as S6 since the CPU / GPU is the same.
					return true;
				default:
					return false;
			}
		}

		public static bool IsGalaxyS7SnapDragon(string deviceModel)
		{
			// Testing for the specific device numbers used by samsung to find S7 and then to find specific vendors to
			// identify the chipset, Exynos or SnapDragon
			// example: SystemInfo.deviceModel for Galaxy S8 is: "samsung SM-G950F"
			string parsedDeviceModel = deviceModel;
			if (parsedDeviceModel.StartsWith("samsung "))
			{
				parsedDeviceModel = parsedDeviceModel.Remove(0, "samsung ".Length);
			}

			// the last bit of the device model string for Samsung are variations of the same models for different providers.
			//string vendorId = parsedDeviceModel.Remove(0, parsedDeviceModel.Length >= 7 ? 7 : parsedDeviceModel.Length);
			parsedDeviceModel = parsedDeviceModel.Substring(0, parsedDeviceModel.Length >= 7 ? 7 : parsedDeviceModel.Length);

			bool isS7 = false;
			switch (parsedDeviceModel)
			{
				case "SC-02H": // Japanese Galaxy S7 Edge SnapDragon 820
				case "SCV33": // Japanese Galaxy S7 Edge SnapDragon 820
					return true;

				case "SM-G930": // Galaxy S7
				case "SM-G935": // Galaxy S7 Edge
					isS7 = true;
					break;
				default:
					isS7 = false;
					break;
			}

			return isS7 && !SystemInfo.graphicsDeviceName.StartsWith("Mali-T880");
		}

		public static GraphicsMode GetOptimizedQualitySettingsForDevice()
		{
#if UNITY_EDITOR
			GraphicsMode result = GraphicsMode.HIGH;
#elif UNITY_IOS
			GraphicsMode result = GraphicsMode.HIGH;
			LogUtil.Log(LogTags.SYSTEM, typeof(GraphicsSettingsService), "Device generation: " + Device.generation);

			switch (Device.generation)
			{
			// at some point we can require arm64 / opengles-3 or metal to exclude devices.
			// Device usage stats: http://hwstats.unity3d.com/mobile/device-ios.html
			// Performance stats: https://browser.primatelabs.com/ios-benchmarks
			// Max memory stats: http://stackoverflow.com/questions/5887248/ios-app-maximum-memory-budget 

			// iPhone
			case DeviceGeneration.iPhone: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone3G: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone3GS: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone4: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone4S: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone5: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPhone5C: // NO arm64, opengles-3, metal

			// iPad
			case DeviceGeneration.iPad1Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPad2Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPad3Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPad4Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPadMini1Gen: // NO arm64, opengles-3, metal
			

			// iPod Touch
			case DeviceGeneration.iPodTouch1Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPodTouch2Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPodTouch3Gen: // NO arm64, opengles-3, metal
			case DeviceGeneration.iPodTouch4Gen: // NO arm64, opengles-3, metal
			
				result = GraphicsMode.LOW;
				break;

			// iPhone
			case DeviceGeneration.iPodTouch5Gen:
			case DeviceGeneration.iPhone5S:
			case DeviceGeneration.iPhone6:
			case DeviceGeneration.iPhone6Plus:
			case DeviceGeneration.iPhone6S:
			case DeviceGeneration.iPhone6SPlus:
			case DeviceGeneration.iPhoneSE1Gen:
			case DeviceGeneration.iPhone7:
			case DeviceGeneration.iPhone7Plus:

			// iPad
			case DeviceGeneration.iPadAir1:
			case DeviceGeneration.iPadMini2Gen:
			case DeviceGeneration.iPadAir2:
			case DeviceGeneration.iPadMini3Gen:
			case DeviceGeneration.iPadMini4Gen:
			case DeviceGeneration.iPadPro1Gen:
			case DeviceGeneration.iPadPro10Inch1Gen:

				result = GraphicsMode.MEDIUM;
				break;

			// iPhone
			case DeviceGeneration.iPhone8:
			case DeviceGeneration.iPhone8Plus:
			case DeviceGeneration.iPhoneX:

			// iPad
			case DeviceGeneration.iPad5Gen:
			case DeviceGeneration.iPadPro2Gen:
			case DeviceGeneration.iPadPro10Inch2Gen:

			// iPod Touch
			case DeviceGeneration.iPodTouch6Gen:

			// Unknown (future proof)
			case DeviceGeneration.iPhoneUnknown:
			case DeviceGeneration.iPodTouchUnknown:
			case DeviceGeneration.iPadUnknown:
			case DeviceGeneration.Unknown:

				result = GraphicsMode.HIGH;
				break;

			default:

				result = GraphicsMode.HIGH;
				break;
			}

#elif UNITY_ANDROID
			GraphicsMode result = GraphicsMode.MEDIUM;
#if OCULUS_VR
			result = GetQualityLevelForGearVR(SystemInfo.deviceModel);
#endif
#else
			GraphicsMode result = GraphicsMode.HIGH;
#endif

			return result;
		}

		public static GraphicsMode GetQualityLevelForGearVR(string deviceModel)
		{
			GraphicsMode result = GraphicsMode.HIGH;

			string parsedDeviceModel = deviceModel;

			// Testing for the specific device numbers used by samsung
			// example: SystemInfo.deviceModel for Galaxy S8 is: "samsung SM-G950F"
			if (parsedDeviceModel.StartsWith("samsung "))
			{
				parsedDeviceModel = parsedDeviceModel.Remove(0, "samsung ".Length);
			}
			// the last bit of the device model string for Samsung are variations of the same models for different providers,
			// which mostly aren't relevant for quality settings, so strip the irrelevant part.
			parsedDeviceModel = parsedDeviceModel.Substring(0, parsedDeviceModel.Length >= 7 ? 7 : parsedDeviceModel.Length);

			switch (parsedDeviceModel)
			{
				case "SM-G920": // Galaxy S6
				case "SM-G925": // Galaxy S6 Edge
				case "SM-G928": // Galaxy S6 Edge Plus
				case "SM-N920": // Galaxy Note 5
					result = GraphicsMode.LOW;
					break;
				case "SC-02H": // Japanese Galaxy S7 Edge Snapdragon 820
				case "SCV33": // Japanese Galaxy S7 Edge Snapdragon 820
				case "SM-G930": // Galaxy S7
				case "SM-G935": // Galaxy S7 Edge
					result = IsGalaxyS7SnapDragon(deviceModel) ? GraphicsMode.LOW : GraphicsMode.MEDIUM;
					break;
				case "SM-A530": // Galaxy A8 (2018)
				case "SM-A730": // Galaxy A8+ (2018)
					result = GraphicsMode.MEDIUM;
					break;
				case "SM-G950": // Galaxy S8
				case "SM-G955": // Galaxy S8 Plus
				case "SM-N950": // Galaxy Note 8
				case "SM-G960": // Galaxy S9
				case "SM-G965": // Galaxy S9 Plus
					result = GraphicsMode.HIGH;
					break;
			}

			LogUtil.Log(LogTags.GRAPHICS, "GraphicsSettingsService", "Set graphics settings for GearVR device to " +
				result + " based on deviceModel " + deviceModel + " with GPU " + SystemInfo.graphicsDeviceName + " by " +
				SystemInfo.graphicsDeviceVendor + ", parsed deviceModel: " + parsedDeviceModel);

			return result;
		}
	}
}