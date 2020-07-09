// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Events;

namespace Talespin.Core.Foundation.Settings
{
	public class GraphicsSettingsChangedEvent : IEvent
	{
		public readonly GraphicsMode Mode;
		public readonly int Quality;
		public readonly int GlobalMaximumLOD;

		public GraphicsSettingsChangedEvent(GraphicsMode mode, int globalMaximumLOD)
		{
			Mode = mode;
			Quality = (int)mode;
			GlobalMaximumLOD = globalMaximumLOD;
		}
	}
}