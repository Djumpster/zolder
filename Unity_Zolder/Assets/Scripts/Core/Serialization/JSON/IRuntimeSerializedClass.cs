// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Serialization
{
	public interface IRuntimeSerializedClass
	{
		Type DataType { get; }
		string Identifier { get; }
		string FullTypeString { get; }

		/// <summary>
		/// The name is a cached value of the last known
		/// name. This can be used to determine what type it
		/// was in case the target type GUID goes missing.
		/// </summary>
		string Name { get; }
		bool Enabled { set; get; }
	}
}
