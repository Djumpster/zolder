// Copyright 2019 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Attributes;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// ScriptableObject/Service which provides the configuration for all <see cref="AudioBank"/> configurations in the
	/// entire project.
	/// </summary>
	[ScriptableObjectServicePath("Audio")]
	public class AudioBanks : ScriptableObject
	{
		public AudioBank[] Banks => audioBanks;

		[SerializeField] private AudioBank[] audioBanks;

		/// <summary>
		/// Returns the <see cref="AudioBank"/> and <see cref="AudioBankEntry"/> associated with the audioID.
		/// </summary>
		/// <param name="audioID">The audioID which we are interested in.</param>
		/// <returns>A tuple containing the related <see cref="AudioBank"/> and <see cref="AudioBankEntry"/>.</returns>
		public (AudioBank, AudioBankEntry) GetAudioBankEntry(string audioID)
		{
			foreach (AudioBank audioBank in audioBanks)
			{
				AudioBankEntry audioBankEntry = audioBank.GetAudioBankEntry(audioID);

				if (audioBankEntry != null)
				{
					return (audioBank, audioBankEntry);
				}
			}

			return (null, null);
		}
	}
}