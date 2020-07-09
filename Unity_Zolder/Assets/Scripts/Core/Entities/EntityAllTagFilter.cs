// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// An EntityFilters that filters entities from a source entity filter and selects only those entities that match *ALL* of the tag parameters.
	/// Typically, you provide the EntityDatabase as a source filter (to filter the full set of all entities, but using other filters as source 
	/// allows you to use a more complex composite filtering.
	/// </summary>
	public class EntityAllTagFilter : EntityFilterBase
	{
		private string[] tags;

		public EntityAllTagFilter(IEntityFilter source, params string[] tags) : base(source)
		{
			this.tags = tags;
		}

		protected override bool Evaluate(IEntity entity)
		{
			return entity.HasAll(tags);
		}
	}
}