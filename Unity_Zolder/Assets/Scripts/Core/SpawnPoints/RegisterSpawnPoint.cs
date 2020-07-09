// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.SpawnPoints
{
	/// <summary>
	/// Place this on an object to register / unregister a spawn point Transform on Start / OnDestroy
	/// </summary>
	public class RegisterSpawnPoint : MonoBehaviour
	{
		[SerializeField, ConstantTag(typeof(string), typeof(SpawnPointIdentifiersBase))] private string spawnPointID;
		[SerializeField] private Transform spawnTransform;

		private SpawnPointService spawnPointService;

		private Transform SpawnTransform => spawnTransform = spawnTransform ?? transform;
		private SpawnPointService SpawnPointService => spawnPointService = spawnPointService ?? GlobalDependencyLocator.Instance.Get<SpawnPointService>();

		public void InjectDependencies(SpawnPointService spawnPointService) => this.spawnPointService = spawnPointService;
		
		protected void Start()
		{
			SpawnPointService.RegisterSpawnPoint(spawnPointID, SpawnTransform);
		}

		protected void OnDestroy()
		{
			SpawnPointService.UnregisterSpawnPoint(spawnPointID, SpawnTransform);
		}
	}
}
