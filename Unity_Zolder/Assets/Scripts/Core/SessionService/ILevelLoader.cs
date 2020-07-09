// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Talespin.Core.Foundation.Session
{
	// Implementors should perform loading and unloading of the level content. 
	// Functions are called by the GamePlaySessionService whenever loading or unloading should happen.
	public interface ILevelLoader
	{
		event LevelHandler OnLevelLoadedEvent;

		ILevelData LoadedLevel { get; }

		float Progress { get; }

		void Load(ILevelData data, Action<ILevelData> callback = null);
		void CleanUp(Action callback = null);
	}
}
