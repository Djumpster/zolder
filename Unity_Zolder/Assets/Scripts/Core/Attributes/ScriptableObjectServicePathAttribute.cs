// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Attributes
{
	/// <summary>
	/// Add this as a class attribute to indicate the generated scriptable object should be placed in a subfolder
	/// of Assets/Data/Config/Resources. The auto-generation script will take this into account.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ScriptableObjectServicePathAttribute : Attribute
	{
		public string SubFolderPath;

		public ScriptableObjectServicePathAttribute(string subfolderPath)
		{
			SubFolderPath = subfolderPath;
		}
	}
}