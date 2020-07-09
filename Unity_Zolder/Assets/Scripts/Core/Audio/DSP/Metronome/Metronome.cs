// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class Metronome : MonoBehaviour
	{
		public double NextQuantizationDspTime
		{
			get
			{
				if (!running)
				{
					return AudioSettings.dspTime;
				}

				double realitiveDspTime = AudioSettings.dspTime - startTime; // Offset so we're starting at 0
				double relativeQuantizationDspTime = SecondsPerQuantization - (realitiveDspTime % SecondsPerQuantization); // Relative
				double absolueQuantizationDspTime = relativeQuantizationDspTime += AudioSettings.dspTime; // Absolute

				return absolueQuantizationDspTime;
			}
		}
		public bool Running { get { return running; } }
		private double SecondsPerBar { get { return (480d / bpm) / 2d; } }
		private double SecondsPerBeat { get { return SecondsPerBar / 4d; } }
		private double SecondsPerQuantization { get { return SecondsPerBeat * (int)quantization; } }

		[SerializeField] private double bpm = 120d;
		[SerializeField] private bool debugMetronome;
		[SerializeField, Range(0, 1)] private float metronomeVolume = 0.1f;

		private Quantizations quantization;
		private AudioSource audioSource;
		private bool running;
		private double startTime;
		private double nextBeatTime;
		private int beatNumber;
		private int beatNumberThisBar;

		private MetronomeSettings queuedMetronomeSettings;
		private double queuedMetronomeStartTime;

		private Synth synth;
		private const double BEEP_DURATION = 0.05d;
		private const double BAR_BEEP_FREQUENCY = 1500d;
		private double beepStopDspTime;
		private double beepFrequency;
		private int sampleRate;

		public void StartOrQueueMetronome(MetronomeSettings metronomeSettings)
		{
			if (!running)
			{
				StartMetronome(metronomeSettings);
			}
			else
			{
				queuedMetronomeSettings = metronomeSettings;
				queuedMetronomeStartTime = NextQuantizationDspTime;
			}
		}

		public void StopMetronome()
		{
			running = false;
			queuedMetronomeSettings = null;
		}

		private void StartMetronome(MetronomeSettings metronomeSettings)
		{
			bpm = metronomeSettings.BPM;
			quantization = metronomeSettings.Quantization;

			startTime = AudioSettings.dspTime;
			nextBeatTime = AudioSettings.dspTime;
			beatNumber = 0;
			beatNumberThisBar = 0;
			running = true;
		}

		protected void Awake()
		{
			audioSource = gameObject.RequireComponent<AudioSource>();
			audioSource.spatialBlend = 0f;
			quantization = Quantizations.Bar;

			synth = new Synth();
		}

		protected void Start()
		{
			sampleRate = AudioSettings.outputSampleRate;
		}

		protected void OnAudioFilterRead(float[] data, int channels)
		{
			// Queue handling
			if (queuedMetronomeSettings != null && AudioSettings.dspTime >= queuedMetronomeStartTime)
			{
				bpm = queuedMetronomeSettings.BPM;
				quantization = queuedMetronomeSettings.Quantization;
				queuedMetronomeSettings = null;
			}

			if (!running || !debugMetronome)
			{
				return;
			}

			// Metronome beeping
			if (AudioSettings.dspTime < beepStopDspTime)
			{
				synth.Sine(data, channels, beepFrequency, metronomeVolume, sampleRate);
			}

			// Beat handling
			if (AudioSettings.dspTime >= nextBeatTime)
			{
				beatNumberThisBar = beatNumber % 4;

				//bool isQuantizationBeat = beatNumber % (int)quantization == 0;
				bool isStartBarBeat = beatNumberThisBar == 0;

				//playBarBeep = isStartBarBeat;
				//playBeatBeep = !isStartBarBeat;

				nextBeatTime += SecondsPerBeat;

				beepStopDspTime = AudioSettings.dspTime + BEEP_DURATION;
				beepFrequency = isStartBarBeat ? BAR_BEEP_FREQUENCY : BAR_BEEP_FREQUENCY / 2;

				//callQuantizationEvent |= isQuantizationBeat;

				beatNumber++;
			}
		}
	}
}