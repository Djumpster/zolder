// Copyright 2020 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Session
{
	/// <summary>
	/// A base interface for application-specific level progression systems.
	/// This should not be implemented on an application-shared scope, because there
	/// could be variety in how progression works between apps. To disable progression
	/// all together, simply implement this interface and <code><see langword="return"/> <see langword="true"/>;</code>.
	/// </summary>
	public interface ILevelProgressor
	{
		/// <summary>
		/// Checks whether the ILevelProgressor has been Initialized; useful if the implementor needs to check the backend or similar
		/// </summary>				
		bool Initialized { get; }

		/// <summary>
		/// Attempt to unlock a specific level. This may fail if the implementation
		/// decides that the requested level should not be unlocked yet.
		/// </summary>
		/// <param name="levelData">The level to unlock</param>
		/// <returns><see langword="true"/> if the level has been successfully unlocked. <see langword="false"/> otherwise</returns>
		bool TryUnlockLevel(ILevelData levelData);

		/// <summary>
		/// Check whether or not the provided level is currently unlocked.
		/// </summary>
		/// <param name="levelData">The level to check</param>
		/// <returns><see langword="true"/> if the level is currently unlocked. <see langword="false"/> otherwise</returns>
		bool IsLevelUnlocked(ILevelData levelData);

		/// <summary>
		/// Attempt to complete a specific level. This may fail if the implementation
		/// decides that the requested level may not be completed.
		/// </summary>
		/// <param name="levelData">The level to complete</param>
		/// <returns><see langword="true"/> if the level has been successfully completed. <see langword="false"/> otherwise</returns>
		bool TryCompleteLevel(ILevelData levelData);

		/// <summary>
		/// Check whether or not the provided level has been completed.
		/// </summary>
		/// <param name="levelData">The level to check</param>
		/// <returns><see langword="true"/> if the level is currently completed. <see langword="false"/> otherwise</returns>
		bool IsLevelCompleted(ILevelData levelData);
	}
}
