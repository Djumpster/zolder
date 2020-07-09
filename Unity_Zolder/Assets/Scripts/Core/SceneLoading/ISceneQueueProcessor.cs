// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Session;

namespace Talespin.Core.Foundation.SceneLoading
{
	/// <summary>
	/// The scene queue processor is an application-specific implementation of scene loading.
	/// Without an implementation of this interface, scene loading using <see cref="LoadScenesAction"/> and
	/// <see cref="SessionService"/> will not work.
	///
	/// <para>
	/// The purpose of this interface is to allow for customizability on an application-level
	/// in how scene loading should behave. A simple example would be:
	///
	/// <list type="number">
	/// <item>Unload scenes</item>
	/// <item>Load scenes</item>
	/// <item>Set active scene</item>
	/// </list>
	///
	/// However in many cases this is not sufficient.
	/// </para>
	///
	/// <para>
	/// For example in a VR application you want to "hide" the camera to prevent nausea-inducing stutters
	/// whilst the application is loading, which would complicate the behaviour to be:
	///
	/// <list type="number">
	/// <item>Start camera fade</item>
	/// <item>*wait for fade to complete*</item>
	/// <item>Unload scenes</item>
	/// <item>Load scenes</item>
	/// <item>Set active scene</item>
	/// <item>Stop camera fade</item>
	/// <item>*wait for fade to complete*</item>
	/// </list>
	/// </para>
	/// </summary>
	public interface ISceneQueueProcessor
	{
		/// <summary>
		/// This method will be invoked when a scene load command has been generated
		/// and a scene transition has been requested.
		/// </summary>
		/// <param name="command">The command containing the specifics of the scene transition</param>
		void Process(SceneLoadCommand command);
	}
}
