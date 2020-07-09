// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.Services;
using Talespin.Core.Foundation.Storage;
using UnityEngine;

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// Keeps track of the time the app was open and running by the player.
	/// You should make sure to initialize this service at the start of your app.
	/// </summary>
	public class TimePlayedTrackerService
	{
		public const int HEART_BEAT_INTERVAL = 10;

		public float TotalTimePlayed { get { return totalTimePlayed; } }
		private float totalTimePlayed
		{
			get
			{
				return dataPacket.GetFloat("totalTimePlayed", 0f);
			}
			set
			{
				dataPacket.Set("totalTimePlayed", value);
				localDataManager.Save(dataPacket);
			}
		}

		public float SessionTimePlayed { get; private set; }

		private LocalDataManager localDataManager;
		private DataPacket dataPacket;

		public TimePlayedTrackerService(LocalDataManager localDataManager, DataPacket dataPacket,
			CoroutineService coroutineService)
		{
			this.localDataManager = localDataManager;
			this.dataPacket = dataPacket;

			coroutineService.StartCoroutine(TrackHeartbeat(), this);
		}

		private IEnumerator TrackHeartbeat()
		{
			while (true)
			{
				totalTimePlayed += HEART_BEAT_INTERVAL;
				SessionTimePlayed += HEART_BEAT_INTERVAL;
				yield return new WaitForSeconds(HEART_BEAT_INTERVAL);
			}
		}
	}
}
