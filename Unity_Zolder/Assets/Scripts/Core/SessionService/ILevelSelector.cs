// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Session
{
	public delegate void LevelSelectedHandler(ILevelData previousLevelData, ILevelData newLevelData);

	/// <summary>
	/// Used to designate a level that should be loaded the next time
	/// the <see cref="SessionService"/> is started.
	/// <para>
	/// The <see cref="CurrentLevel"/> property of this instance
	/// may change mid-session and should not be used to check what
	/// the currently active level is, use <see cref="ILevelActivator"/>
	/// for that information.
	/// </para>
	/// </summary>
	/// <seealso cref="SessionService"/>
	/// <seealso cref="ILevelLoader"/>
	/// <seealso cref="ILevelActivator"/>
	/// <seealso cref="ILevelProgressor"/>
	public interface ILevelSelector
	{
		event LevelSelectedHandler OnLevelSelected;

		ILevelData CurrentLevel { get; }

		void SelectLevel(ILevelData level);

		bool CanSelectLevel(ILevelData level);
	}
}
