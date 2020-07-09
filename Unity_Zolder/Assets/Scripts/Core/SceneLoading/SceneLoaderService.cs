// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Talespin.Core.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Talespin.Core.Foundation.SceneLoading
{
	/// <summary>
	/// A generic scene loader which will unload a scene if there are no more references towards it.
	/// </summary>
	/// <remarks>
	/// Whenever a caller requests a scene it will load it if it hasn't already been loaded,
	/// once there are no more requests for a specific scene it will be unloaded.
	/// </remarks>
	public class SceneLoaderService : ISceneLoader
	{
		/// <inheritdoc/>
		public event Action<SceneLoadCommand> ScenesProcessingEvent = delegate { };

		/// <inheritdoc/>
		public event Action<SceneLoadCommand> ScenesProcessedEvent = delegate { };

		/// <inheritdoc/>
		public bool IsProcessing { private set; get; }

		private readonly Dictionary<object, List<string>> desiredSceneContext;
		private readonly List<string> loadedScenes;

		private string cachedActiveScene;
		private string newActiveScene;

		public SceneLoaderService()
		{
			desiredSceneContext = new Dictionary<object, List<string>>();
			loadedScenes = new List<string>();

			cachedActiveScene = SceneManager.GetActiveScene().name;
			newActiveScene = cachedActiveScene;
		}

		/// <inheritdoc/>
		public void RegisterScenes(IEnumerable<string> sceneNames, object context)
		{
			foreach (string scene in sceneNames)
			{
				RegisterScene(scene, context);
			}
		}

		/// <inheritdoc/>
		public void RegisterScene(string sceneName, object context)
		{
			if (!desiredSceneContext.ContainsKey(context))
			{
				desiredSceneContext.Add(context, new List<string>());
			}

			desiredSceneContext[context].Add(sceneName);
		}

		/// <inheritdoc/>
		public void UnregisterScenes(IEnumerable<string> sceneNames, object context)
		{
			foreach (string scene in sceneNames)
			{
				UnregisterScene(scene, context);
			}
		}

		/// <inheritdoc/>
		public void UnregisterScene(string sceneName, object context)
		{
			if (!desiredSceneContext.ContainsKey(context))
			{
				return;
			}

			desiredSceneContext[context].Remove(sceneName);
		}

		/// <inheritdoc/>
		public void UnregisterContext(object context)
		{
			if (!desiredSceneContext.ContainsKey(context))
			{
				return;
			}

			desiredSceneContext[context].Clear();
		}

		/// <inheritdoc/>
		public SceneLoadCommand Process()
		{
			if (!HasUnprocessedChanges())
			{
				ScenesProcessingEvent.Invoke(null);
				return null;
			}

			if (IsProcessing)
			{
				ScenesProcessingEvent.Invoke(null);
				return null;
			}

			IsProcessing = true;

			List<object> removedContexts = new List<object>();

			foreach (KeyValuePair<object, List<string>> kvp in desiredSceneContext)
			{
				if (kvp.Value.Count == 0)
				{
					removedContexts.Add(kvp.Key);
				}
			}

			for (int i = 0; i < removedContexts.Count; i++)
			{
				object context = removedContexts[i];
				desiredSceneContext.Remove(context);
			}

			List<string> concatenatedScenes = ConcatenateLists(desiredSceneContext);
			List<string> scenesToLoad = CalculateDelta(concatenatedScenes, loadedScenes);
			List<string> scenesToUnload = CalculateDelta(loadedScenes, concatenatedScenes);

			if (scenesToUnload.Contains(newActiveScene))
			{
				newActiveScene = null;
			}

			// Pre-update the loaded scenes to ensure subsequent Process calls
			// before the command has been consumed will not trigger another change
			loadedScenes.Clear();
			loadedScenes.AddRange(concatenatedScenes);
			cachedActiveScene = newActiveScene;

			void OnSceneLoadCommandConsumed(SceneLoadCommand cmd)
			{
				IsProcessing = false;
				ScenesProcessedEvent.Invoke(cmd);
			}

			string debugMessage = CreateSceneLoadCommandLog(scenesToUnload, scenesToLoad, cachedActiveScene);
			Debug.Log(debugMessage);

			SceneLoadCommand command = new SceneLoadCommand(scenesToLoad, scenesToUnload, cachedActiveScene, OnSceneLoadCommandConsumed);

			ScenesProcessingEvent.Invoke(command);
			return command;
		}

		/// <inheritdoc/>
		public void SetActiveScene(string sceneName)
		{
			List<string> concatenatedScenes = ConcatenateLists(desiredSceneContext);

			if (concatenatedScenes.Contains(sceneName))
			{
				newActiveScene = sceneName;
			}
		}

		/// <inheritdoc/>
		public bool HasUnprocessedChanges()
		{
			List<string> concatenatedScenes = ConcatenateLists(desiredSceneContext);

			bool scenesChanged = concatenatedScenes.Count != loadedScenes.Count;

			if (!scenesChanged)
			{
				for (int i = 0; i < concatenatedScenes.Count; i++)
				{
					string scene = concatenatedScenes[i];

					if (!loadedScenes.Contains(scene))
					{
						scenesChanged = true;
						break;
					}
				}
			}

			// Don't use SceneManager.GetActiveScene().name here to prevent
			// infinite loading spam if it failed to set the active scene for whatever
			// reason, most likely because the target scene hasn't been added to the build settings.
			return scenesChanged || newActiveScene != cachedActiveScene;
		}

		private static List<string> CalculateDelta(IEnumerable<string> original, IEnumerable<string> target)
		{
			List<string> result = new List<string>();

			foreach (string item in original)
			{
				if (!target.Contains(item))
				{
					result.Add(item);
				}
			}

			return result;
		}

		private static List<string> ConcatenateLists(Dictionary<object, List<string>> data)
		{
			List<string> result = new List<string>();

			foreach (KeyValuePair<object, List<string>> kvp in data)
			{
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (result.Contains(kvp.Value[i]))
					{
						continue;
					}

					result.Add(kvp.Value[i]);
				}
			}

			return result;
		}

		private static string CreateSceneLoadCommandLog(List<string> scenesToUnload, List<string> scenesToLoad, string activeScene)
		{
			StringBuilder result = new StringBuilder();
			result.AppendLine("[Scene Loading] Requested to process scene changes:");

			result.AppendLine("Scenes to unload:");
			for (int i = 0; i < scenesToUnload.Count; i++)
			{
				result.Append("\t- ");
				result.AppendLine(scenesToUnload[i]);
			}

			result.AppendLine("Scenes to load:");
			for (int i = 0; i < scenesToLoad.Count; i++)
			{
				result.Append("\t- ");
				result.AppendLine(scenesToLoad[i]);
			}

			result.Append("Active Scene: ");
			result.AppendLine(activeScene ?? "null");

			return result.ToString();
		}
	}
}
