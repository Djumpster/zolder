// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// EntityFilters are an enumeration of entities. The OnEntityAdded and OnEntityRemoved are triggered when entities are added or removed from the filter.
	/// </summary>
	public interface IEntityFilter : IEnumerable<IEntity>, IDisposable
	{
		event Action<IEntity> EntityAddedEvent;
		event Action<IEntity> EntityRemovedEvent;
	}
}