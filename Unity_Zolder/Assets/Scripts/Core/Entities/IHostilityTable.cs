// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Entities
{
	public interface IHostilityTable
	{
		bool AreHostile(string entityTagA, string entityTagB);
	}
}