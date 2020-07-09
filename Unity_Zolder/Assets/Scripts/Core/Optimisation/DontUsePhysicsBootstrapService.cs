// Copyright 2018 Talespin, LLC. All Rights Reserved.
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation
{
	/// <summary>
	/// A bootstrap service that disables physics simulation completely.
	/// </summary>
	public class DontUsePhysicsBootstrapService : IBootstrappable
	{
		public DontUsePhysicsBootstrapService() => Physics.autoSimulation = false;
	}
}