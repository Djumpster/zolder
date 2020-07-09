// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Service for keeping track of all available microphones and handles new connected and disconnected devices.
	/// </summary>
	public class MicrophoneService : IDisposable, IBootstrappable
	{
		private const float INTERVAL_CHECK_DEVICES_SECONDS = 2;

		public delegate void MicrophoneConnectionChangeHandler(MicrophoneDevice microphone);

		public MicrophoneConnectionChangeHandler OnNewMicrophoneConnectedEvent = delegate { };
		public MicrophoneConnectionChangeHandler OnCurrentMicrophoneChangedEvent = delegate { };
		public MicrophoneConnectionChangeHandler OnMicrophoneDisconnectedEvent = delegate { };

		public List<MicrophoneDevice> MicrophoneDevices { get; private set; }

		public MicrophoneRecordingSettings Settings { get; private set; }

		public MicrophoneDevice CurrentMicrophone { get; private set; }

		public bool MicrophoneDetected => CurrentMicrophone != null && !CurrentMicrophone.IsSilent;

		private readonly ICallbackService callbackService;

		private float lastDevicesPollTime;

		public MicrophoneService(MicrophoneRecordingSettings recordingSettings, ICallbackService callbackService)
		{
			this.callbackService = callbackService;

			callbackService.UpdateEvent += Update;

			Settings = recordingSettings;
			RetrieveConnectedDevices();

			CurrentMicrophone = MicrophoneDevices.Count > 0 ? MicrophoneDevices[0] : null;
			CurrentMicrophone?.Start();
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup >= lastDevicesPollTime + INTERVAL_CHECK_DEVICES_SECONDS)
			{
				lastDevicesPollTime = Time.realtimeSinceStartup;
				string[] devices = Microphone.devices;

				if (devices.Length != MicrophoneDevices.Count)
				{
					UpdateDevices(devices);
				}
			}
		}

		private void RetrieveConnectedDevices()
		{
			lastDevicesPollTime = Time.realtimeSinceStartup;

			MicrophoneDevices = new List<MicrophoneDevice>();
			foreach (string device in Microphone.devices)
			{
				MicrophoneDevices.Add(new MicrophoneDevice(
					device,
					Settings.SampleRate,
					Settings.BufferSizeSecs,
					Settings.SampleSegments,
					Settings.SilenceThreshold,
					Settings.SilenceDuration,
					callbackService));
			}
		}

		private void UpdateDevices(string[] devices)
		{
			if (devices.Length > MicrophoneDevices.Count)
			{
				OnNewDeviceConnected(devices);
			}
			else if (devices.Length < MicrophoneDevices.Count)
			{
				OnDeviceDisconnected(devices);
			}
		}

		private void OnNewDeviceConnected(string[] devices)
		{
			MicrophoneDevice newDevice = new MicrophoneDevice(
						devices.First(deviceName => MicrophoneDevices.All(device => device.DeviceName != deviceName)),
						Settings.SampleRate,
						Settings.BufferSizeSecs,
						Settings.SampleSegments,
						Settings.SilenceThreshold,
						Settings.SilenceDuration,
						callbackService);

			bool currentChanged = false;

			if (!MicrophoneDetected)
			{
				CurrentMicrophone?.Stop();
				CurrentMicrophone = newDevice;
				CurrentMicrophone?.Start();
				currentChanged = true;
			}

			MicrophoneDevices.ForEach(x => x.OnNewDeviceConnected());
			MicrophoneDevices.Add(newDevice);
			OnNewMicrophoneConnectedEvent.Invoke(newDevice);

			if (currentChanged)
			{
				OnCurrentMicrophoneChangedEvent.Invoke(newDevice);
			}
		}

		private void OnDeviceDisconnected(string[] devices)
		{
			MicrophoneDevice disconnectedDevice = MicrophoneDevices.First(device => !devices.Contains(device.DeviceName));
			bool currentChanged = false;

			disconnectedDevice.Dispose();
			MicrophoneDevices.Remove(disconnectedDevice);

			currentChanged = CurrentMicrophone == disconnectedDevice;

			CurrentMicrophone = MicrophoneDevices.IsEmpty() ? null : currentChanged ? MicrophoneDevices[0] : CurrentMicrophone;

			MicrophoneDevices.ForEach(device => device.OnNewDeviceConnected());
			OnMicrophoneDisconnectedEvent.Invoke(disconnectedDevice);

			if (currentChanged)
			{
				OnCurrentMicrophoneChangedEvent.Invoke(null);
			}
		}

		public void Dispose()
		{
			callbackService.UpdateEvent -= Update;
		}
	}
}
