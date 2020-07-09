// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Serialization
{
	public class DataTagAttribute : Attribute
	{
		public readonly string Tag;

		public DataTagAttribute(string tag)
		{
			Tag = tag;
		}
	}
}