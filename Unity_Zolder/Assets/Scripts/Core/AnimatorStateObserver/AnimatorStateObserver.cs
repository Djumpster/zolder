// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.AnimatorStateObserver
{
	/// <summary>
	/// AnimatorStateObserver keeps track of the transitioning behaviour of the mecanim AnimatorController
	/// attached to a GameObject. Every time a transition to or from a state with a SendCharacterState component
	/// is started or finished, that SendCharacterState component will either mark itself as /entered/ 
	/// or /exited/. This information is then aggregated and used for telling higher-level systems that 
	/// the Avatar has just entered or left a specific state. 
	/// All of this should ensure that higher-level systems always have a reliable picture of the "meta" state
	/// of the Mecanim state machine, keeping in mind the possibility of multiple states being active simultaneously 
	/// (i.e. during transitions).
	/// </summary>
	public class AnimatorStateObserver : MonoBehaviour
	{
		public delegate void CharacterStateChangedHandler(string state);
		public event CharacterStateChangedHandler AnimatorStateEnteredEvent = delegate { };
		public event CharacterStateChangedHandler AnimatorStateExitedEvent = delegate { };
		private readonly List<string> states = new List<string>();
		private readonly List<string> heartBeats = new List<string>();
		private List<string> enteredStates = new List<string>();
		private List<string> exitedStates = new List<string>();

		public void SignalHeartbeat(string characterState, int stateIdentifier)
		{
			if (!heartBeats.Contains(characterState))
			{
				heartBeats.Add(characterState);
			}
		}

		/// <summary>
		/// Returns whether this instance is in the specified CharacterState.
		/// </summary>
		/// <returns><see langword="true" /> if this instance is in the specified characterState; otherwise, <see langword="false" />.</returns>
		/// <param name="characterState">Character state.</param>
		public bool IsInState(string characterState)
		{
			return states.Contains(characterState);
		}

		public bool IsInAllStates(params string[] states)
		{
			bool allStates = true;

			foreach (var item in states)
			{
				allStates = IsInState(item);
			}

			return allStates;
		}

		public bool IsInAnyOfStates(params string[] states)
		{
			foreach (var item in states)
			{
				if (IsInState(item))
				{
					return true;
				}
			}

			return false;
		}

		private void LateUpdate()
		{
			enteredStates.Clear();
			for (int i = 0; i < heartBeats.Count; i++)
			{
				if (!states.Contains(heartBeats[i]))
				{
					enteredStates.Add(heartBeats[i]);
					states.Add(heartBeats[i]);
				}
			}

			exitedStates.Clear();
			for (int i = states.Count - 1; i >= 0; i--)
			{
				if (!heartBeats.Contains(states[i]))
				{
					exitedStates.Add(states[i]);
					states.RemoveAt(i);
				}
			}

			heartBeats.Clear();

			if (AnimatorStateEnteredEvent != null)
			{
				foreach (string enteredState in enteredStates)
				{

					AnimatorStateEnteredEvent(enteredState);
				}
			}
			if (AnimatorStateExitedEvent != null)
			{
				foreach (string exitedState in exitedStates)
				{
					AnimatorStateExitedEvent(exitedState);
				}
			}
		}
	}
}
