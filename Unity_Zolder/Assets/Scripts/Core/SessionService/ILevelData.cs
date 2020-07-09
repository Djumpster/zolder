// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Session
{
	/// <summary>
	/// Minimum interface of data describing a level. Most games will have much more per-level data,
	/// but this interface defines the minimum required to work with Util systems like objectives and 
	/// the GameSessionService. 
	/// </summary>
	public interface ILevelData
	{
		string LevelIdentifier { get; }
		List<string> LevelComponents { get; }
	}
}