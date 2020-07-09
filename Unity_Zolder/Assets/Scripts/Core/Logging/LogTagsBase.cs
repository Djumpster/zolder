// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Talespin.Core.Foundation.Reflection;

namespace Talespin.Core.Foundation.Logging
{
	/// <summary>
	/// The tags to be used for filtering using ConsolePro.
	/// </summary>
	public abstract class LogTagsBase
	{
		public static List<string> GetAvailableTypes()
		{
			List<string> types = new List<string>();

			IEnumerable<Type> logTags = Reflect.AllTypesFrom<LogTagsBase>();

			foreach (Type logTag in logTags)
			{
				List<FieldInfo> fieldInfos = new List<FieldInfo>(logTag.GetFields(BindingFlags.Public | BindingFlags.Static));

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
