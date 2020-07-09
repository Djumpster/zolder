// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.SceneLoading
{
	/// <summary>
	/// A command containing instructions on what scenes should be (un)loaded. This is not processed
	/// within a closed environment to allow for custom scene loading behaviour.
	/// </summary>
	public class SceneLoadCommand
	{
		/// <summary>
		/// A list of scenes that should be loaded.
		/// </summary>
		public IEnumerable<string> ScenesToLoad { get; }

		/// <summary>
		/// A list of scenes that should be unloaded.
		/// </summary>
		public IEnumerable<string> ScenesToUnload { get; }

		/// <summary>
		/// The scene that should become the new active scene.
		/// </summary>
		public string ActiveScene { get; }

		/// <summary>
		/// This will be set to <see langword="true"/> once this command has been fully consumed.
		/// </summary>
		public bool IsConsumed { private set; get; }

		private readonly Action<SceneLoadCommand> onConsumedCallback;

		public SceneLoadCommand(IEnumerable<string> scenesToLoad, IEnumerable<string> scenesToUnload, string activeScene, Action<SceneLoadCommand> onConsumedCallback)
		{
			ScenesToLoad = scenesToLoad;
			ScenesToUnload = scenesToUnload;
			ActiveScene = activeScene;

			this.onConsumedCallback = onConsumedCallback;
		}

		/// <summary>
		/// Consume the command, marking it as completed.
		/// </summary>
		public void Consume()
		{
			IsConsumed = true;
			onConsumedCallback.Invoke(this);
		}
	}
}
