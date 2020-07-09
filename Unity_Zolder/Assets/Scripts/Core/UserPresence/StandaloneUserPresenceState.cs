// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

#if UNITY_STANDALONE
namespace Talespin.Core.Foundation.UserPresence
{
	/// <summary>
	/// For standalone applications the user should always be considered
	/// present.
	/// </summary>
	public class StandaloneUserPresenceState : IUserPresenceState
	{
		/// <inheritdoc/>
		public bool IsUserPresent => true;
	}
}
#endif
