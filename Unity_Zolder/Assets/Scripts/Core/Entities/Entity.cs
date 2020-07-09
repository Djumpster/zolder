// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// Serves as a handle to the root of a (gameObject) entity that can be described with multiple tags.
	/// Use this to look up any kind of game entity by the defined tags.
	/// </summary>
	public class Entity : IEntity
	{
		#region properties
		public IEnumerable<string> Tags { get { return tags; } }
		private readonly string[] tags = new string[0];
		#endregion

		#region members
		protected EntityDatabase entityDatabase;
		#endregion

		#region constructor
		public Entity(string[] tags, EntityDatabase entityDatabase, bool subscribe = true)
		{
			this.tags = tags;
			this.entityDatabase = entityDatabase;

			if (subscribe)
			{
				Subscribe();
			}
		}
		#endregion
		public bool Disposed { get; protected set; }

		#region public methods
		public virtual void Dispose()
		{
			if (Disposed)
			{
				return;
			}
			Unsubscribe();
			Disposed = true;
		}

		public bool HasTag(string tag)
		{
			return tags.Contains(tag);
		}

		public bool HasAll(List<string> tagset)
		{
			for (int i = 0; i < tagset.Count; i++)
			{
				if (!tags.Contains(tagset[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool HasAll(params string[] tagset)
		{
			for (int i = 0; i < tagset.Length; i++)
			{
				if (!tags.Contains(tagset[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool HasAny(List<string> tagset)
		{
			for (int i = 0; i < tagset.Count; i++)
			{
				for (int j = 0; j < tags.Length; j++)
				{
					if (tags[j] == tagset[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasAny(params string[] tagset)
		{
			for (int i = 0; i < tagset.Length; i++)
			{
				for (int j = 0; j < tags.Length; j++)
				{
					if (tags[j] == tagset[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsEnemyOf(IHostilityTable table, Entity entity)
		{
			for (int i = 0; i < entity.tags.Length; i++)
			{
				for (int j = 0; j < tags.Length; j++)
				{
					if (table.AreHostile(tags[j], entity.tags[i]))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsEnemyOf(IHostilityTable table, IEnumerable<string> otherTags)
		{
			foreach (string s in otherTags)
			{
				for (int j = 0; j < tags.Length; j++)
				{
					if (table.AreHostile(tags[j], s))
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region private methods
		protected void Subscribe()
		{
			entityDatabase.SubscribeEntity(this);
		}

		protected void Unsubscribe()
		{
			entityDatabase.UnsubscribeEntity(this);
		}
		#endregion
	}
}