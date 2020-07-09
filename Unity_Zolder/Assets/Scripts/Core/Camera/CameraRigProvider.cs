// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Audio;
using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.XR;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// The default camera rig provider. Provides configuration
	/// using the <see cref="CameraRigLibrary"/>.
	/// <para>
	/// This implementation allows for custom camera rig types
	/// using the <see cref="ICameraRigTypeProvider"/> interface.
	/// Simply implement the interface as a service and this provider
	/// will pick it up and use it as long as the returned rig type is
	/// not <see cref="CameraRigType.None"/>.
	/// </para>
	/// <para>
	/// When not using the <see cref="ICameraRigTypeProvider"/> interface,
	/// this implementation will default to a set of built-in supported rig types.
	/// These rig types are:
	/// <list type="bullet">
	/// <item><see cref="CameraRigType.OculusQuest"/></item>
	/// <item><see cref="CameraRigType.OculusRift"/></item>
	/// <item><see cref="CameraRigType.HtcVive"/></item>
	/// <item><see cref="CameraRigType.Mobile"/></item>
	/// <item><see cref="CameraRigType.Desktop"/></item>
	/// </list>
	/// </para>
	/// </summary>
	/// <seealso cref="CameraRigLibrary"/>
	/// <seealso cref="CameraService"/>
	public class CameraRigProvider : ICameraProvider
	{
		private readonly CameraRigLibrary rigLibrary;
		private readonly ICameraRigTypeProvider rigTypeProvider;
		private readonly IDependencyInjector injector;
		private readonly AudioListenerService audioListener;

		private ICameraController spawnedController;

		public CameraRigProvider(CameraRigLibrary rigLibrary, ICameraRigTypeProvider rigTypeProvider, IDependencyInjector injector, AudioListenerService audioListener)
		{
			this.rigLibrary = rigLibrary;
			this.rigTypeProvider = rigTypeProvider;
			this.injector = injector;
			this.audioListener = audioListener;
		}

		/// <inheritdoc/>
		public ICameraController Provide()
		{
			if (spawnedController == null)
			{
				CameraRigType rigType = GetRigType();
				CameraRigConfiguration configuration = rigLibrary.GetConfiguration(rigType);

				spawnedController = CreateCameraRig(rigType, configuration);
			}

			return spawnedController;
		}

		private CameraRigType GetRigType()
		{
			CameraRigType rigType = CameraRigType.None;

			if (rigTypeProvider != null)
			{
				rigType = rigTypeProvider.GetRigType();
			}

			if (rigType == CameraRigType.None)
			{
				rigType = GetRigTypeBuiltIn();
			}

			return rigType;
		}
		
		private ICameraController CreateCameraRig(CameraRigType rigType, CameraRigConfiguration configuration)
		{
			GameObject obj = InjectionUtils.InstantiateAndInject(configuration.RigPrefab, injector, (p) => Object.Instantiate(p, Vector3.zero, Quaternion.identity));
			obj.name = $"Camera Rig ({rigType})";
			Object.DontDestroyOnLoad(obj);

			ICameraController controller = obj.GetComponentInChildren<ICameraController>();

			if (controller == null)
			{
				Debug.LogError($"No camera controller found on camera rig of type: {rigType}", obj);
				return null;
			}

			Debug.Log($"Spawned camera rig of type {rigType}", obj);

			audioListener.ClaimListener(controller.Ears);
			return controller;
		}

		private static CameraRigType GetRigTypeBuiltIn()
		{
			if (!string.IsNullOrEmpty(XRSettings.loadedDeviceName))
			{
				Debug.Log($"Loaded XR Device Name: {XRSettings.loadedDeviceName}");

				switch (XRSettings.loadedDeviceName)
				{
					case "Oculus":
					case "oculus display":
						XRSettings.enabled = true;
						return Application.isMobilePlatform ? CameraRigType.OculusQuest : CameraRigType.OculusRift;
					case "OpenVR":
						XRSettings.enabled = true;
						return CameraRigType.HtcVive;
					default:
						Debug.LogWarning($"Unsupported XR device: {XRSettings.loadedDeviceName}");
						XRSettings.enabled = false;
						break;
				}
			}

			if (Application.isMobilePlatform)
			{
				return CameraRigType.Mobile;
			}

			return CameraRigType.Desktop;
		}
	}
}
