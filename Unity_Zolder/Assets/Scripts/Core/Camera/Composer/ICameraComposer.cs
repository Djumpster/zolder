// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Composer
{
	/// <summary>
	/// A camera composer can be used to create transitions
	/// between scenes or screens. A simple example of a composer
	/// is a fade-to-black.
	/// <para>
	/// More complex composers can be implemented by creating
	/// custom implementations of this interface.
	/// </para>
	/// </summary>
	/// <see cref="CameraRigComposerService"/>
	public interface ICameraComposer
	{
		/// <summary>
		/// Fired when the composer is starting.
		/// </summary>
		event Action ComposerStartingEvent;

		/// <summary>
		/// Fired when the composed has finished transitioning,
		/// and is currently fully visible.
		/// </summary>
		event Action ComposerStartedEvent;

		/// <summary>
		/// Fired when the composer is ending.
		/// </summary>
		event Action ComposerEndingEvent;

		/// <summary>
		/// Fired when the composer has finished transitioning,
		/// and is currently fully hidden.
		/// </summary>
		event Action ComposerEndedEvent;

		/// <summary>
		/// Gets the current state of the composer.
		/// </summary>
		/// <value>
		/// By default this will be set to <see cref="CameraComposerState.Ended"/>.
		/// Changes made to the state will also be reflected in the events, when
		/// the state transitions to <see cref="CameraComposerState.Starting"/>
		/// the <see cref="ComposerStartingEvent"/> will also be fired.
		/// </value>
		CameraComposerState State { get; }

		/// <summary>
		/// Show the composer. Will not do anything
		/// if the composer is already fully visible.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the <paramref name="duration"/> parameter is specified and has
		/// a value above <c>0</c> it will be used to override the default
		/// transition duration.
		/// </para>
		/// <para>
		/// If the <paramref name="duration"/> parameter is specified and has
		/// a value of <c>0</c> it will make the transition instantaneous.
		/// </para>
		/// </remarks>
		/// <param name="duration">The duration of the transition in seconds</param>
		/// <returns>A coroutine that handles the transition</returns>
		Coroutine ShowComposer(float duration = -1);

		/// <summary>
		/// Hide the composer. Will not do anything
		/// if the composer is already fully hidden.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the <paramref name="duration"/> parameter is specified and has
		/// a value above <c>0</c> it will be used to override the default
		/// transition duration.
		/// </para>
		/// <para>
		/// If the <paramref name="duration"/> parameter is specified and has
		/// a value of <c>0</c> it will make the transition instantaneous.
		/// </para>
		/// </remarks>
		/// <param name="duration">The duration of the transition in seconds</param>
		/// <returns>A coroutine that handles the transition</returns>
		Coroutine HideComposer(float duration = -1);

		/// <summary>
		/// Fully show the composer immediately. This will interrupt
		/// any ongoing transitions and set the visibility of the composer
		/// to the maximum value.
		/// </summary>
		void ShowComposerImmediately();

		/// <summary>
		/// Fully hide the composer immediately. This will interrupt
		/// any ongoing transitions and set the visibility of the composer
		/// to the minimum value.
		/// </summary>
		void HideComposerImmediately();
	}
}
