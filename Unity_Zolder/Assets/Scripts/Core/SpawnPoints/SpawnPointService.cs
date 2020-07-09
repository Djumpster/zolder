// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.SpawnPoints
{
	/// <summary>
	/// Keeps track of spawn points by a string id. These can be registered when a scene is loaded and unregistered
	/// when it is unloaded.
	/// </summary>
	public class SpawnPointService
	{
		public delegate void SpawnPointEvent(string id);

		public event SpawnPointEvent SpawnPointRegisteredEvent = delegate { };
		public event SpawnPointEvent SpawnPointUnregisteredEvent = delegate { };

		private readonly Dictionary<string, Transform> spawnPoints;

		public SpawnPointService()
		{
			spawnPoints = new Dictionary<string, Transform>();
		}

		public SpawnPointHelper ManageTransform(Transform transform, string spawnPointID, bool managePosition = true, bool manageRotation = true)
		{
			UnmanageTransform(transform);

			Transform spawnPoint = GetSpawnPoint(spawnPointID);
			SpawnPointHelper helper = transform.gameObject.AddComponent<SpawnPointHelper>();
			helper.Initialize(transform, spawnPoint, managePosition, manageRotation);

			return helper;
		}

		public void UnmanageTransform(Transform transform)
		{
			SpawnPointHelper helper = transform.GetComponent<SpawnPointHelper>();
			UnityEngine.Object.Destroy(helper);
		}

		public void UnmanageTransform(SpawnPointHelper helper)
		{
			UnityEngine.Object.Destroy(helper);
		}

		public void RegisterSpawnPoint(string spawnPointID, Transform transform)
		{
			LogUtil.Info(LogTags.UI, this, "Registering spawn point with id " + spawnPointID);
			spawnPoints[spawnPointID] = transform;
			SpawnPointRegisteredEvent.Invoke(spawnPointID);
		}

		public void UnregisterSpawnPoint(string spawnPointID, Transform transform)
		{
			if (spawnPoints.ContainsKey(spawnPointID) && spawnPoints[spawnPointID] == transform)
			{
				LogUtil.Info(LogTags.UI, this, "Unregistering spawn point with id " + spawnPointID);
				spawnPoints.Remove(spawnPointID);
				SpawnPointUnregisteredEvent.Invoke(spawnPointID);
			}
		}

		public bool HasSpawnPoint(string spawnPointID) => spawnPoints.ContainsKey(spawnPointID);

		public Transform GetSpawnPoint(string spawnPointID)
		{
			if (!spawnPoints.ContainsKey(spawnPointID))
			{
				throw new ArgumentException($"No spawn point with ID: {spawnPointID} found");
			}

			return spawnPoints[spawnPointID];
		}
	}
}
