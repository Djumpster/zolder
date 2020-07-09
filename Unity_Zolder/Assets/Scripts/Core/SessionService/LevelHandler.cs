// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Session
{
	public delegate void LevelStartedHandler(ILevelData levelData);
	public delegate void LevelQuitHandler(ILevelData levelData);
	public delegate void LevelHandler(ILevelData levelData);
}
