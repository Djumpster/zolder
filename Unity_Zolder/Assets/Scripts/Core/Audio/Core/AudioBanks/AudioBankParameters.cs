// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Container for holding parameters for AudioBanks to hook onto, allowing for modification of audio at runtime.
	/// </summary>
	public class AudioBankParameters : MonoBehaviour
	{
		private class ContexedAudioBankParameters
		{
			public readonly GameObject Context;

			private readonly Dictionary<string, float> parameters;

			public ContexedAudioBankParameters(GameObject context)
			{
				Context = context;
				parameters = new Dictionary<string, float>();
			}

			public void SetParameter(string parameterID, float value)
			{
				parameters[parameterID] = value;
			}

			public float GetParameter(string parameterID)
			{
				return parameters.TryGetValue(parameterID, out float value) ? value : 0;
			}
		}

		private Dictionary<GameObject, ContexedAudioBankParameters> contexts;

		public void SetParameter(string parameterID, float value, GameObject context = null)
		{
			if (contexts.TryGetValue(context, out ContexedAudioBankParameters parameters))
			{
				parameters.SetParameter(parameterID, value);
			}
			else
			{
				contexts[context] = new ContexedAudioBankParameters(context);
				contexts[context].SetParameter(parameterID, value);
			}
		}

		public float? GetParameter(string parameterID, GameObject context = null)
		{
			if (contexts.TryGetValue(context, out ContexedAudioBankParameters parameters))
			{
				return parameters.GetParameter(parameterID);
			}

			return null;
		}

		protected void Update()
		{
			// TODO cleanup where contexts are destroyed
		}
	}
}