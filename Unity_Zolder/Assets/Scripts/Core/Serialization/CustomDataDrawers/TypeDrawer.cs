// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Reflection;

namespace Talespin.Core.Foundation.Serialization
{
	public class TypeDrawer : StringPopupDrawer
	{
		public override IEnumerable<string> Tags
		{
			get { yield return "type"; }
		}

		protected override IEnumerable<string> GetOptions(DataEntry entry)
		{
			string baseTypeName = entry.Tags.Where(tag => tag.StartsWith("type")).Select(typeTag => typeTag.Split('_').LastOrDefault()).FirstOrDefault();
			if (!string.IsNullOrEmpty(baseTypeName))
			{
				Type baseType = Type.GetType(baseTypeName);
				if (baseType != null)
				{
					return Reflect.AllTypeStringsFrom(baseType);
				}
			}
			return new string[0];
		}
	}
}
#endif