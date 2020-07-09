// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Serialization
{
	public interface ICustomDataDrawer
	{
		IEnumerable<string> Tags { get; }

		bool Draw(DataEntry entry); //Use the bool return-type to specificy if you've modified the collection.
	}
}