// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// The EntityDatabase is a service that provides access to all entities in the game. 
	/// Generally, one is interested only in specific entities. Use EntityFilters to filter the
	/// full set to smaller subsets or even a single entity (like the player).
	/// </summary>
	public class EntityDatabase : IEntityFilter
	{
		private HashSet<IEntity> allEntities = new HashSet<IEntity>();
		private System.Action<IEntity> entityAddedEvent = delegate { };

		/// <summary>
		/// Calls on entity added for each existing entity!
		/// </summary>
		public event System.Action<IEntity> EntityAddedEvent
		{
			add
			{
				entityAddedEvent -= value;
				entityAddedEvent += value;
				foreach (IEntity entity in allEntities)
				{
					value(entity);
				}
			}
			remove
			{
				entityAddedEvent -= value;
			}
		}

		private event System.Action<IEntity> entityRemovedEvent = delegate { };
		public event System.Action<IEntity> EntityRemovedEvent
		{
			add
			{
				entityRemovedEvent -= value;
				entityRemovedEvent += value;
			}
			remove
			{
				entityRemovedEvent -= value;
			}
		}

		public void SubscribeEntity(IEntity entity)
		{
			if (entity == null)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Entity cannot be null.");
				return;
			}

			// Hashset enforces unique values, no need to do contains
			if (allEntities.Add(entity))
			{
				entityAddedEvent(entity);
			}
			else
			{
				return;
			}
		}

		public void UnsubscribeEntity(IEntity entity)
		{
			if (entity == null)
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Entity cannot be null.");
				return;
			}

			if (allEntities.Remove(entity))
			{
				entityRemovedEvent(entity);
			}
			else
			{
				return;
			}
		}

		public void Dispose()
		{
		}

		public IEnumerator<IEntity> GetEnumerator()
		{
			return allEntities.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
