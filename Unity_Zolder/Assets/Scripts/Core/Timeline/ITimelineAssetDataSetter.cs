// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// An interface that should be implemented on custom <see cref="PlayableAsset"/> to allow setting specific data in runtime constructed timeline assets.
	/// </summary>
	public interface ITimelineAssetDataSetter
	{
		void SetData(object data);
	}
}
