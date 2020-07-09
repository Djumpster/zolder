// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioAssetPlayableAsset : PlayableAsset
	{
		[SerializeField] private ExposedReference<AudioAsset> audioAsset;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<AudioAssetPlayableBehaviour> playable = ScriptPlayable<AudioAssetPlayableBehaviour>.Create(graph);

			AudioAssetPlayableBehaviour behaviour = playable.GetBehaviour();

			AudioAsset asset = audioAsset.Resolve(playable.GetGraph().GetResolver());

			behaviour.Initialize(asset);

			return playable;
		}
	}
}