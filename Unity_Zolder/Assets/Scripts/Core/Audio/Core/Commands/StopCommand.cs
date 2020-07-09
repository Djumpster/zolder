// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Audio
{
	public class StopCommand : IStopCommand
	{
		public DAHDSR VolumeEnvelope { get; set; }

		public StopCommand() : this(new AudioConfiguration())
		{
		}

		public StopCommand(AudioAsset audioAsset) : this(audioAsset.AudioConfiguration)
		{
		}

		public StopCommand(AudioConfiguration audioConfiguration) : this(audioConfiguration.VolumeEnvelope)
		{
		}

		public StopCommand(DAHDSR volumeEnvelope)
		{
			VolumeEnvelope = volumeEnvelope;
		}
	}
}