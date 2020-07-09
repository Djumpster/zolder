// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// An EntityFilters that filters entities from a source entity filter and selects only those entities that match *ONE* of the tag parameters.
	/// Typically, you provide the EntityDatabase as a source filter (to filter the full set of all entities, but using other filters as source 
	/// allows you to use a more complex composite filtering.
	/// </summary>
	public class EntityAnyTagFilter : EntityFilterBase
	{
		private List<string> tags = new List<string>();

		public EntityAnyTagFilter(IEntityFilter source, params string[] tags) : base(source)
		{
			this.tags.AddRange(tags);
		}

		public EntityAnyTagFilter(IEntityFilter source, List<string> tags) : base(source)
		{
			this.tags.AddRange(tags);
		}

		protected override bool Evaluate(IEntity entity)
		{
			return entity.HasAny(tags);
		}
	}
}