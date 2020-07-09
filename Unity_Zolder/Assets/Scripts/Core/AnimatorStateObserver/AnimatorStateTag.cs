// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.AnimatorStateObserver
{
	/// <summary>
	/// Class that contains common biomechanical states. These states can be extended
	/// in a project by adding a separate file in the project that extends this class with project specific states. 
	/// </summary>
	public class AnimatorStateTag
	{
		public const string IDLE = "Idle";
		public const string MOVING = "Moving";

		public const string UI_IDLE_IN = "UIIdleIn";
		public const string UI_IDLE_OUT = "UIIdleOut";
		public const string UI_SLIDE_IN = "UISlideIn";
		public const string UI_SLIDE_OUT = "UISlideOut";
	}
}
