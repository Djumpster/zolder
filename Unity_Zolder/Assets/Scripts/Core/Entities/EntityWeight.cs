// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Entities
{
	public struct EntityWeight
	{
		public IEntity entity;
		public float normalizedWeight;

		public EntityWeight(IEntity entity, float normalizedWeight)
		{
			this.entity = entity;
			this.normalizedWeight = normalizedWeight;
		}
	}
}
