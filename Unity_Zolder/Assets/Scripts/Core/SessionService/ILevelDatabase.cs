// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Session
{
	// Inherit from this class to implement a database of levels for your game.
	public interface ILevelDatabase
	{
		/// <summary>
		/// Returns a list of all <see cref="ILevelData"/> found in the database.
		/// </summary>
		List<ILevelData> Levels { get; }
		/// <summary>
		/// Returns a hashset of all <see cref="ILevelData.LevelIdentifier"/> found in the database.
		/// </summary>
		HashSet<string> LevelIdentifiers { get; }

		/// <summary>
		/// Searches the level database for the specified identifier.
		/// </summary>
		/// <param name="identifier">The identifier for the level you need to find.</param>
		/// <returns>The <see cref="ILevelData"/> when found, otherwise it returns zero.</returns>
		ILevelData FindLevel(string identifier);
	}
}
