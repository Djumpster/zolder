// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.SceneLoading
{
	/// <summary>
	/// The scene loader is required to implement a "queue" of desired scenes, and a process
	/// command to load the queue.
	///
	/// <para>
	/// The idea of having a queue and processing it later is designed so that multiple
	/// scene loading commands within a timeframe can be processed at the same time,
	/// preventing unneeded application stalls.
	/// </para>
	///
	/// <para>
	/// By default the interval between processing the queue should be a single frame,
	/// but for specific use-cases the process interval may be customized.
	/// </para>
	/// </summary>
	public interface ISceneLoader
	{
		/// <summary>
		/// Event guaranteed to fire when <see cref="Process"/> is called. The data
		/// of the action will either be <see langword="null"/> if there are no modifications,
		/// or a <see cref="SceneLoadCommand"/> when there are.
		///
		/// <para>
		/// This callback can be used to guarantee some behaviours are executed after all (un)registration
		/// calls in a single cycle have been made.
		/// </para>
		/// </summary>
		event Action<SceneLoadCommand> ScenesProcessingEvent;

		/// <summary>
		/// Event fired when the current queue of scene modifications has
		/// been fully processed.
		/// </summary>
		event Action<SceneLoadCommand> ScenesProcessedEvent;

		/// <summary>
		/// This property is set to <see langword="true"/> when <see cref="Process"/> is called,
		/// and will remain so until the <see cref="SceneLoadCommand.IsConsumed"/> flag has been set.
		/// </summary>
		bool IsProcessing { get; }

		/// <summary>
		/// Register multiple scenes under the same context.
		/// </summary>
		/// <remarks>
		/// Note that this does not immediately invoke scene loading. The scenes are added to
		/// a queue and processed once <see cref="Process(Action)"/> has been invoked.
		/// </remarks>
		/// <param name="sceneNames">A list containing the names of the scenes you want to load</param>
		/// <param name="context">The parent of the scenes, use this to later on unload the scenes</param>
		void RegisterScenes(IEnumerable<string> sceneNames, object context);

		/// <summary>
		/// Register a single scene under a context.
		/// </summary>
		/// <remarks>
		/// Note that this does not immediately invoke scene loading. The scenes is added to
		/// a queue and processed once <see cref="Process(Action)"/> has been invoked.
		/// </remarks>
		/// <param name="sceneName">The name of the scene you wish to load</param>
		/// <param name="context">The parent of the scene, use this to later on unload the scene</param>
		void RegisterScene(string sceneName, object context);

		/// <summary>
		/// Unregister multiple scenes from the same context.
		/// </summary>
		/// <remarks>
		/// Note that this does not immediately invoke scene unloading. The scenes are added to
		/// a queue and processed once <see cref="Process(Action)"/> has been invoked.
		///
		/// <para>
		/// Unregistering a scene may not even trigger scene unloading at all if there is some other
		/// context that still requests the scenes you've unregistered.
		/// </para>
		/// </remarks>
		/// <param name="sceneNames">A list containing the names of the scenes you want to load</param>
		/// <param name="context">The parent of the scenes, this should be the same object that
		/// was used to register the scene</param>
		void UnregisterScenes(IEnumerable<string> sceneNames, object context);

		/// <summary>
		/// Unregister a scenes from a context.
		/// </summary>
		/// <remarks>
		/// Note that this does not immediately invoke scene unloading. The scene is added to
		/// a queue and processed once <see cref="Process(Action)"/> has been invoked.
		///
		/// <para>
		/// Unregistering a scene may not even trigger scene unloading at all if there is some other
		/// context that still requests the scene you've unregistered.
		/// </para>
		/// </remarks>
		/// <param name="sceneName">The scene you want to unload</param>
		/// <param name="context">The parent of the scenes, this should be the same object that
		/// was used to register the scene</param>
		void UnregisterScene(string sceneName, object context);

		/// <summary>
		/// Unregister all scenes for the given context.
		/// </summary>
		/// <param name="context">The parent of the scenes</param>
		void UnregisterContext(object context);

		/// <summary>
		/// Trigger scene modification processing. This will change the internal state of the service
		/// to processing and allow you to handle the process command.
		/// </summary>
		/// <returns>A command containing instructions on what scenes should be (un)loaded</returns>
		SceneLoadCommand Process();

		/// <summary>
		/// Check whether there currently are unprocessed changes
		/// </summary>
		/// <returns><see langword="true"/> if there currently are unprocessed changes in the scene queue.</returns>
		bool HasUnprocessedChanges();

		/// <summary>
		/// Request a scene to become the new active scene.
		/// </summary>
		/// <param name="sceneName">The name of the scene that should become active</param>
		void SetActiveScene(string sceneName);
	}
}
