// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Audio
{
	[System.Serializable]
	public class MetronomeSettings
	{
		public double BPM = 120d;
		public Quantizations Quantization = Quantizations.None;

		public MetronomeSettings()
		{
		}

		public MetronomeSettings(double bpm, Quantizations quantization)
		{
			BPM = bpm;
			Quantization = quantization;
		}
	}
}