// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Session
{
	// Implementors should perform starting and stopping of a single session. 
	// The functions are called by the GamePlaySessionService whenever loading 
	// is complete and the session should start and whenever the session is complete.
	public interface ILevelActivator
	{
		event LevelStartedHandler OnLevelStartedEvent;
		event LevelHandler OnLevelEndedEvent;

		ILevelData ActiveLevel { get; }

		void ActivateLevel(ILevelData data, Action<ILevelData> callback = null);
		void StopActiveLevel(Action<ILevelData> callback = null);
	}
}
