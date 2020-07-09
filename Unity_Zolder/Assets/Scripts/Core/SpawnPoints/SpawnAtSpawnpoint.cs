// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.SpawnPoints
{
	/// <summary>
	/// Sets the initial orientation of this object to match the defined spawnpoint
	/// </summary>
	public class SpawnAtSpawnpoint : MonoBehaviour
	{
		[SerializeField, ConstantTag(typeof(string), typeof(SpawnPointIdentifiersBase))] private string spawnPoint;

		private SpawnPointService spawnPointService;

		protected void InjectDependencies(SpawnPointService spawnPointService)
		{
			this.spawnPointService = spawnPointService;
		}

		protected void Start()
		{
			spawnPointService.SpawnPointRegisteredEvent += OnSpawnPointRegistered;
			spawnPointService.SpawnPointUnregisteredEvent += OnSpawnPointUnregistered;

			if (spawnPointService.HasSpawnPoint(spawnPoint))
			{
				spawnPointService.ManageTransform(transform, spawnPoint);
			}
			else
			{
				Debug.LogWarning($"The spawnpoint {spawnPoint} does not exist at startup. Waiting for it to be registered", this);
			}
		}

		protected void OnDestroy()
		{
			spawnPointService.SpawnPointRegisteredEvent -= OnSpawnPointRegistered;
			spawnPointService.SpawnPointUnregisteredEvent -= OnSpawnPointUnregistered;
		}

		private void OnSpawnPointUnregistered(string id)
		{
			if (id.Equals(spawnPoint))
			{
				spawnPointService.UnmanageTransform(transform);
			}
		}

		private void OnSpawnPointRegistered(string id)
		{
			if (id.Equals(spawnPoint))
			{
				spawnPointService.ManageTransform(transform, spawnPoint);
			}
		}
	}
}

