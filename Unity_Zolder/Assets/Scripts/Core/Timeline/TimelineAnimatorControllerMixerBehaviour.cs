// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Mixer that blends the float values from the same parameter key of a track.
	/// </summary>
	public class TimelineAnimatorControllerMixerBehaviour : PlayableBehaviour
	{
		private Dictionary<string, float> floatParameters = new Dictionary<string, float>();

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame(playable, info, playerData);

			Animator animator = playerData as Animator;

			int inputCount = playable.GetInputCount();

			floatParameters.Clear();

			for (int i = 0; i < inputCount; i++)
			{
				ScriptPlayable<TimelineAnimatorControllerBehaviour> inputPlayble = (ScriptPlayable<TimelineAnimatorControllerBehaviour>)playable.GetInput(i);
				TimelineAnimatorControllerBehaviour input = inputPlayble.GetBehaviour();
				if (input != null)
				{
					for (int j = 0; j < input.AnimatorParameterContainer.FloatParameters.Count; j++)
					{
						Animations.AnimatorParameterContainer.FloatAnimatorParameter floatParameter = input.AnimatorParameterContainer.FloatParameters[j];
						if (!floatParameters.ContainsKey(floatParameter.ParameterName))
						{
							floatParameters.Add(floatParameter.ParameterName, floatParameter.Value * playable.GetInputWeight(i));
						}
						else
						{
							floatParameters[floatParameter.ParameterName] += floatParameter.Value * playable.GetInputWeight(i);
						}
					}
				}
			}

			foreach (var parameter in floatParameters)
			{
				animator.SetFloat(parameter.Key, parameter.Value);
			}
		}
	}
}
