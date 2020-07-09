// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.PSDtouGUI
{
	/// <summary>
	/// Class that holds serializable pairs of string used in the ConfigPSDtoUGUI in order to save layer that have a
	/// certain tag to it's paired folder path.
	/// </summary>
	[Serializable]
	public class TagFolderPair
	{
		[SerializeField] private string tag;
		public string Tag
		{
			get
			{
				return tag.Replace(" ", "");
			}
		}
		[SerializeField] private string folder;
		public string Folder
		{
			get
			{
				return folder.Replace(" ", "");
			}
		}
	}
}
