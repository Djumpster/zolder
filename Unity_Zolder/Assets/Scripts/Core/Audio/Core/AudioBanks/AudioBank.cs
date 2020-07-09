// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System.Linq;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Filter;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// A configuration of a single AudioBank. 
	/// </summary>
	[System.Serializable]
	public class AudioBank
	{
		public string AudioBankID => audioBankID;
		public int NumberOfChannels => numberOfChannels;
		public System.Type BehaviourType { get { return string.IsNullOrEmpty(bankBehaviour) ? null : bankBehaviour.LoadType(); } }

		[SerializeField] private string audioBankID;
		[SerializeField] private int numberOfChannels = 8;
		[TypeFilter(typeof(BaseAudioBankBehaviour)), SerializeField] private string bankBehaviour;
		[SerializeField] private AudioBankEntry[] entries;

		public AudioBankEntry GetAudioBankEntry(string audioID)
		{
			return entries.FirstOrDefault(e => e.AudioID == audioID);
		}
	}
}