// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Audio
{
	[System.Serializable]
	public class PlayAudioTrackPlayableAsset : PlayableAsset
	{
		[SerializeField] private string contextIdentifier;
		[SerializeField] private ExposedReference<AudioAsset> audioAsset;
		[SerializeField] private float fadeDuration;
		[SerializeField] private bool waitForQuantization;

		// Factory method that generates a playable based on this asset
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<PlayAudioTrackPlayableBehaviour> playable = ScriptPlayable<PlayAudioTrackPlayableBehaviour>.Create(graph);

			PlayAudioTrackPlayableBehaviour behaviour = playable.GetBehaviour();

			AudioAsset asset = audioAsset.Resolve(playable.GetGraph().GetResolver());

			behaviour.Initialize(contextIdentifier, asset, fadeDuration, waitForQuantization);

			return playable;
		}
	}
}
