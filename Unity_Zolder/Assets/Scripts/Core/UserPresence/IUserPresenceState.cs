// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

namespace Talespin.Core.Foundation.UserPresence
{
	/// <summary>
	/// Indicator for whether the user is currently present or not.
	/// </summary>
	public interface IUserPresenceState
	{
		/// <summary>
		/// Is the user currently present?
		/// </summary>
		bool IsUserPresent { get; }
	}
}
