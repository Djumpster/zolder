// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Entities
{
	public abstract class EntityFilterBase : IEntityFilter
	{
		public int Count
		{
			get
			{
				int count = 0;
				foreach (IEntity entity in source)
				{
					if (Evaluate(entity))
					{
						count++;
					}
				}
				return count;
			}
		}

		private IEntityFilter source;
		private bool subscribed;

		private System.Action<IEntity> _entityAddedEvent = delegate { };
		public event System.Action<IEntity> EntityAddedEvent
		{
			add
			{
				_entityAddedEvent -= value;
				_entityAddedEvent += value;
				if (!subscribed)
				{
					SubscribeEvents();
				}
			}
			remove
			{
				_entityAddedEvent -= value;
			}
		}

		private event System.Action<IEntity> _entityRemovedEvent = delegate { };
		public event System.Action<IEntity> EntityRemovedEvent
		{
			add
			{
				_entityRemovedEvent -= value;
				_entityRemovedEvent += value;
			}
			remove
			{
				_entityRemovedEvent -= value;
			}
		}

		public EntityFilterBase(IEntityFilter source)
		{
			this.source = source;
		}

		private void SubscribeEvents()
		{
			subscribed = true;
			source.EntityAddedEvent += OnEntityAddedEvent;
			source.EntityRemovedEvent += OnEntityRemovedEvent;
		}

		public void Dispose()
		{
			source.EntityAddedEvent -= OnEntityAddedEvent;
			source.EntityRemovedEvent -= OnEntityRemovedEvent;
			source = null;
		}

		public IEnumerator<IEntity> GetEnumerator()
		{
			foreach (IEntity entity in source)
			{
				if (Evaluate(entity))
				{
					yield return entity;
				}
			}
		}

		protected abstract bool Evaluate(IEntity entity);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void OnEntityAddedEvent(IEntity newEntity)
		{
			if (Evaluate(newEntity))
			{
				_entityAddedEvent?.Invoke(newEntity);
			}
		}

		private void OnEntityRemovedEvent(IEntity removedEntity)
		{
			if (Evaluate(removedEntity))
			{
				_entityRemovedEvent?.Invoke(removedEntity);
			}
		}
	}
}