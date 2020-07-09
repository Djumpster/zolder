// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// The types of data stored in a data container.
	/// The data container uses string as the backing type for all supported structs.
	/// </summary>
	public enum DataContainerDataType
	{
		/// <summary>
		/// Null value.
		/// </summary>
		None,
		String,
		Bool,
		Double,
		DataContainer,
		String_Array,
		Bool_Array,
		Double_Array,
		DataContainer_Array
	}
}