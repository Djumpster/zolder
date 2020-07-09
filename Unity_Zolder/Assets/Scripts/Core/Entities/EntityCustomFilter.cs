// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// An EntityFilters that filters entities from a source entity filter and selects only those entities that the input function returns true for.
	/// Typically, you provide the EntityDatabase as a source filter (to filter the full set of all entities, but using other filters as source 
	/// allows you to use a more complex composite filtering.
	/// </summary>
	public class EntityCustomFilter : EntityFilterBase
	{
		private System.Func<IEntity, bool> function;

		public EntityCustomFilter(IEntityFilter source, System.Func<IEntity, bool> function) : base(source)
		{
			this.function = function;
		}

		protected override bool Evaluate(IEntity entity)
		{
			return function(entity);
		}
	}
}