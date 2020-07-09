// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// A camera transition command that can be executed by an <see cref="ICameraController"/>.
	/// </summary>
	public interface ICameraTransitionCommand
	{
		/// <summary>
		/// Whether the transition has been completed.
		/// </summary>
		bool HasBeenCompleted { get; }

		/// <summary>
		/// Whether the transition has been started.
		/// </summary>
		bool HasBeenStarted { get; }

		/// <summary>
		/// Initialize the transition with data from the controller.
		/// </summary>
		/// <param name="cameraController">The controller that will execute this command.</param>
		void Initialize(ICameraController cameraController);

		/// <summary>
		/// Executes the transition with the given deltaTime until it is completed.
		/// </summary>
		/// <param name="deltaTimeSeconds">Amount of time in seconds to advance the transition.</param>
		void Execute(float deltaTimeSeconds);
	}
}
