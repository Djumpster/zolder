// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Talespin.Core.Foundation.Reflection;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// The tags to be used for audio in UI.
	/// </summary>
	public abstract class UIAudioTypeBase
	{
		public static List<string> GetAvailableTypes()
		{
			List<string> types = new List<string>();

			IEnumerable<Type> tags = Reflect.AllTypesFrom<UIAudioTypeBase>();

			foreach (Type tag in tags)
			{
				List<FieldInfo> fieldInfos = new List<FieldInfo>(tag.GetFields(BindingFlags.Public | BindingFlags.Static));

				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					if (fieldInfo.IsLiteral)
					{
						types.Add((string)fieldInfo.GetValue(null));
					}
				}
			}

			return types;
		}
	}
}