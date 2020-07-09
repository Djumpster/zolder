// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Storage
{
	/// <summary>
	/// A reference to a float value in a data container.
	/// Can be used to read it without passing the data container.
	/// </summary>
	public class DataContainerContentReference : IFloatReference
	{
		private readonly IDataContainer dataContainer;
		private readonly string floatID;
		public float Value
		{
			get
			{
				return dataContainer.GetFloat(floatID);
			}
		}

		public DataContainerContentReference(IDataContainer data, string floatID)
		{
			this.floatID = floatID;
			dataContainer = data;
		}
	}
}