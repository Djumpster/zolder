// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class MetronomeService : System.IDisposable
	{
		public double NextQuantizationDspTime { get { return metronome.NextQuantizationDspTime; } }
		// public bool Running { get { return metronome.Running; } }
		// public Quantizations Quantization { get { return metronome.Quantization; } set { metronome.Quantization = value; } }

		private readonly Metronome metronome;
		private GameObject gameObject;

		public MetronomeService()
		{
			gameObject = new GameObject("Metronome");
			metronome = gameObject.AddComponent<Metronome>();
			Object.DontDestroyOnLoad(gameObject);
		}

		public void StartOrQueueMetronome(MetronomeSettings metronomeSettings)
		{
			metronome.StartOrQueueMetronome(metronomeSettings);
		}

		public void StopMetronome()
		{
			metronome.StopMetronome();
		}

		public void Dispose()
		{
			Object.Destroy(gameObject);
		}
	}
}