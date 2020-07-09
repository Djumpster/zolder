// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Use UDateTime for making a DateTime field that is editable and serializable.
	/// Adapted from https://gist.github.com/EntranceJew/f329f1c6a0c35ac51763455f76b5eb95.
	/// </summary>
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver
	{
		private DateTime dateTime;

		[HideInInspector] [SerializeField] private string dateTimeString;

		public static implicit operator DateTime(UDateTime udt)
		{
			return (udt.dateTime);
		}

		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime() { dateTime = dt };
		}

		public void OnAfterDeserialize()
		{
			DateTime.TryParse(dateTimeString, out dateTime);
		}

		public void OnBeforeSerialize()
		{
			dateTimeString = dateTime.ToString();
		}
	}
}