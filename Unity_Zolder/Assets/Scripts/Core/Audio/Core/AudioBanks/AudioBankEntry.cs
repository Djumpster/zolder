// Copyright 2019 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Talespin.Core.Foundation.Audio
{

	/// <summary>
	/// Entry which associates an AudioConfiguration with ID.
	/// </summary>
	[System.Serializable]
	public class AudioBankEntry
	{
		public string AudioID { get { return !string.IsNullOrEmpty(audioIDOld) ? audioIDOld : audioID; } }
		public AudioConfiguration AudioConfiguration => audioConfiguration;

		[SerializeField, ConstantTag(typeof(string), typeof(AudioIDBase))] private string audioID;
		[SerializeField, FormerlySerializedAs("audioID")] private string audioIDOld;
		[SerializeField] private AudioConfiguration audioConfiguration;
	}
}
