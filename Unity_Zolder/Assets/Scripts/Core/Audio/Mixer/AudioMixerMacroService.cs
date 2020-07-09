// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Services;
using Talespin.Core.Foundation.TimeKeeping;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioMixerMacroService
	{
		private AudioMixerMacroMapping macroMapping;
		private Dictionary<AudioMixerMacroMapping.Macro, float> macroWeights;
		private AudioMixerService audioMixerService;
		private CoroutineService coroutineService;

		public AudioMixerMacroService(AudioMixerMacroMapping macroMapping, AudioMixerService audioMixerService,
			UnityCallbackService unityCallbackService, CoroutineService coroutineService)
		{
			this.coroutineService = coroutineService;
			this.audioMixerService = audioMixerService;
			this.macroMapping = macroMapping;

			CreateWeightsDictionary();

			unityCallbackService.UpdateEvent += OnUpdateEvent;
		}

		public void SetMacroWeight(string id, float weight)
		{
			AudioMixerMacroMapping.Macro macro = GetMacroByID(id);

			if (macro != null)
			{
				macroWeights[macro] = weight;
			}
		}

		public void FadeMacroWeight(string id, float weight, float duration = 0f)
		{
			AudioMixerMacroMapping.Macro macro = GetMacroByID(id);
			string coroutineID = "AudioMixerMacro" + id;

			coroutineService.StopContext(coroutineID);
			coroutineService.StartCoroutine(BlendToWeight(macro, weight, duration), coroutineID, coroutineID);
		}

		private void OnUpdateEvent()
		{
			foreach (AudioMixerMacroMapping.Macro macro in macroMapping.Macros)
			{
				float macroWeight = GetMacroWeight(macro.ID);

				//				Debug.Log("MacroWeight for " + macro.ID + " is " + macroWeight);

				foreach (AudioMixerMacroMapping.ParameterMapping mapping in macro.Mappings)
				{
					float parameterValue = mapping.GetParameterValue(macroWeight);

					//					Debug.Log("\tSetting: " + mapping.Parameter + " to " + parameterValue);

					audioMixerService.MainMixer.SetFloat(mapping.Parameter, parameterValue);
				}
			}
		}

		private void CreateWeightsDictionary()
		{
			macroWeights = new Dictionary<AudioMixerMacroMapping.Macro, float>();

			foreach (AudioMixerMacroMapping.Macro macro in macroMapping.Macros)
			{
				macroWeights.Add(macro, macro.DefaultWeight);
			}
		}

		private AudioMixerMacroMapping.Macro GetMacroByID(string id)
		{
			return macroMapping.Macros.FirstOrDefault(m => m.ID == id);
		}

		private float GetMacroWeight(string id)
		{
			AudioMixerMacroMapping.Macro macro = GetMacroByID(id);

			return macroWeights[macro];
		}

		private IEnumerator BlendToWeight(AudioMixerMacroMapping.Macro macro, float targetWeight, float duration)
		{
			float startWeight = macroWeights[macro];

			Timer t = new Timer(duration, true);
			while (t)
			{
				macroWeights[macro] = Mathf.Lerp(startWeight, targetWeight, t.progress);
				yield return null;
			}

			macroWeights[macro] = targetWeight;
		}
	}
}