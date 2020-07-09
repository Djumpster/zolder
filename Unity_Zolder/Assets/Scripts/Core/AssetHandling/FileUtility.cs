// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Talespin.Core.Foundation.AssetHandling
{
	/// <summary>
	/// This class contains several static file utility functions.
	/// </summary>
	public static class FileUtility
	{
		/// <summary>
		/// Searchs recursively into a rootDirectory to locate any files with a specifc name.
		/// </summary>
		/// <returns>A list of full paths for files within root directory structure that match the filename.</returns>
		/// <param name="rootDirectory">The path of the root directory.</param>
		/// <param name="filename">The filename to look for.</param>
		public static List<string> SearchForFileNameRecursively(string rootDirectory, string filename)
		{
			List<string> filesFound = new List<string>();
			SearchForFileNameRecursively(rootDirectory, filename, filesFound);
			return filesFound;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Ensures that a path (starting with Assets/) within the project exists.
		/// If any of the folders in the path are missing, they will be created.
		/// </summary>
		/// <param name="path">The path to ensure.</param>
		public static void EnsurePath(string path)
		{
			var names = path.Split(new[] { '/', '\\' });
			var builder = new StringBuilder();
			var previous = "";
			var shouldRefresh = false;
			for (int i = 0; i < names.Length; i++)
			{
				if (!string.IsNullOrEmpty(names[i]))
				{
					builder.Append(names[i]);
					var folder = builder.ToString();

					if (!AssetDatabase.IsValidFolder(folder))
					{
						AssetDatabase.CreateFolder(previous, names[i]);
						shouldRefresh = true;
					}
					previous = folder;
					builder.Append('/');
				}
			}
			if (shouldRefresh)
			{
				AssetDatabase.Refresh();
			}
		}
#endif

		private static void SearchForFileNameRecursively(string rootDirectory, string filename, List<string> filesFound)
		{
			foreach (string directory in Directory.GetDirectories(rootDirectory))
			{
				try
				{
					foreach (string file in Directory.GetFiles(directory, filename))
					{
						filesFound.Add(file);
					}
					SearchForFileNameRecursively(directory, filename, filesFound);
				}
				catch(DirectoryNotFoundException exception)
				{
					Debug.LogWarning($"Directory: {directory} not found, This directory will be skipped in the search. Exception: " + exception);
				}
			}
		}
	}
}
