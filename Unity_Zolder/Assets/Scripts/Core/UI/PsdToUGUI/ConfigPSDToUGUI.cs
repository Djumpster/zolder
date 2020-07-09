// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.PSDtouGUI
{
	/// <summary>
	/// Scriptable object that holds configuration data used for the PSDtoUGUI converter.
	/// </summary>
	[Serializable]
	public class ConfigPSDToUGUI : ScriptableObject
	{
		[SerializeField] private string assetsPath;

		public string AssetsPath { get { return assetsPath; } }

		[SerializeField] private string prefabPath;

		public string PrefabPath { get { return prefabPath; } }

		[SerializeField] private string packingTag;

		public string PackingTag { get { return packingTag; } }

		[SerializeField] private int fontSize;

		public int FontSize { get { return fontSize; } }

		[SerializeField] private int maxTextureSize;

		public int MaxTextureSize { get { return maxTextureSize; } }

		[SerializeField] private TextureImporterCompression textureImporterCompression;

		public TextureImporterCompression TextureImporterCompression { get { return textureImporterCompression; } }

		[SerializeField] private TagFolderPair[] layerTagsAndFolderNames;

		public Dictionary<string, string> LayerTagsAndFolderNames
		{
			get
			{
				Dictionary<string, string> folderTags = new Dictionary<string, string>();
				TagFolderPair[] tagFolderPairs = layerTagsAndFolderNames;

				foreach (TagFolderPair tf in tagFolderPairs)
				{
					folderTags.Add(tf.Tag, tf.Folder);
				}
				return folderTags;
			}
		}
	}
}
#endif