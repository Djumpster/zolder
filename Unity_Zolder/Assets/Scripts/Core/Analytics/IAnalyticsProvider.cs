// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Analytics
{
	/// <summary>
	/// Provides a common interface to perform user analytics operations.
	/// </summary>
	public interface IAnalyticsProvider
	{
		/// <summary>
		/// Notify the analytics portal that a new game was started,
		/// this generally means that the user has logged in and happens
		/// after any custom user identifier and session identifier
		/// has been set.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		bool GameStart(IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that an existing game was stopped,
		/// this generally means that the user has logged out or the application
		/// was existed.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		bool GameOver(IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that a level was started.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="name">The unique name of the level</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		/// <seealso cref="LevelComplete(string, IDictionary{string, object})"/>
		/// <seealso cref="LevelFail(string, IDictionary{string, object})"/>
		/// <seealso cref="LevelQuit(string, IDictionary{string, object})"/>
		bool LevelStart(string name, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that a level was completed. When a level
		/// is completed it is marked as being completed successfully (thus passed).
		/// If there are score related triggers to determine the passing or failing
		/// of a level that should be done prior to calling this function.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="name">The unique name of the level</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		/// <seealso cref="LevelFail(string, IDictionary{string, object})"/>
		/// <seealso cref="LevelQuit(string, IDictionary{string, object})"/>
		bool LevelComplete(string name, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that a level was quit by the user.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="name">The unique name of the level</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		/// <seealso cref="LevelComplete(string, IDictionary{string, object})"/>
		/// <seealso cref="LevelFail(string, IDictionary{string, object})"/>
		bool LevelQuit(string name, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that a level was failed. When a level
		/// is failed it is marked as being completed unsuccessfully (thus not passed).
		/// If there are score related triggers to determine the passing or failing
		/// of a level that should be done prior to calling this function.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="name">The unique name of the level</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		/// <seealso cref="LevelComplete(string, IDictionary{string, object})"/>
		/// <seealso cref="LevelFail(string, IDictionary{string, object})"/>
		bool LevelFail(string name, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal that a screen was visited.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="name">The unique name of the screen</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		bool ScreenVisit(string name, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Notify the analytics portal of a custom event.
		/// </summary>
		/// <remarks>
		/// There are a predetermined number or parameters that will always
		/// be included in the event, the parameters may vary depending
		/// on the analytics implementations.
		/// <para>
		/// There may also be limits to the amount of events that can be sent,
		/// as well as the amount of data a single event can contain.
		/// </para>
		/// </remarks>
		/// <param name="eventName">The unique name the custom event</param>
		/// <param name="parameters">Any additional parameters to send</param>
		/// <returns>If the event was sent successfully this function will return <see langword="true"/></returns>
		bool Custom(string eventName, IDictionary<string, object> parameters = null);

		/// <summary>
		/// Set the user identifier, used to group events for an user anonymously.
		/// </summary>
		/// <param name="userId">The new user identifier, if set to <see langword="null"/>
		/// or empty, a default user identifier will be used</param>
		void SetUserId(string userId);

		/// <summary>
		/// Set the session identifier, used to group events within a single session.
		/// </summary>
		/// <param name="sessionId">The new session identifier, if set to <see langword="null"/>
		/// or empty, a default session identifier will be used</param>
		void SetSessionId(string sessionId);
	}
}
