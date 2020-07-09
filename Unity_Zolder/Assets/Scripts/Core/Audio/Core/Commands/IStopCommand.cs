// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Audio
{
	public interface IStopCommand : IAudioCommand
	{
		DAHDSR VolumeEnvelope { get; set; }
	}
}