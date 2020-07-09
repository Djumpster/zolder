// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Misc;
using UnityEngine;

namespace Talespin.Core.Foundation.Entities
{
	/// <summary>
	/// Serves as a handle to the root of a (gameObject) entity that can be described with multiple tags.
	/// Use this to look up any kind of game entity by the defined tags.
	/// Note: You MUST call InitializeEntity from your derived class!
	/// </summary>
	public abstract class MonoBehaviourEntity : MonoBehaviour, IEntity, IPositioned
	{
		#region properties
		public IEnumerable<string> Tags { get { return entity.Tags; } }

		public Transform Transform
		{
			get
			{
				if (cachedTransform == null && this != null)
				{
					cachedTransform = transform;
				}
				return cachedTransform;
			}
		}
#endregion

#region members
private Entity entity;
		private Transform cachedTransform;
		protected EntityDatabase entityDatabase;
		#endregion

		#region IPositioned implementation
		public string Name { get { return gameObject.name; } }

		public Vector3 Position { get { return Transform.position; } }

		public Quaternion Rotation { get { return Transform.rotation; } }

		public Vector3 Forward { get { return Transform.forward; } }

		public Vector3 Right { get { return Transform.right; } }

		public Vector3 Up { get { return Transform.up; } }
		#endregion


		#region public methods
		public bool HasTag(string tag)
		{
			return entity != null && entity.HasTag(tag);
		}

		public bool HasAll(List<string> tagset)
		{
			return entity != null && entity.HasAll(tagset);
		}

		public bool HasAll(params string[] tagset)
		{
			return entity != null && entity.HasAll(tagset);
		}

		public bool HasAny(List<string> tagset)
		{
			return entity != null && entity.HasAny(tagset);
		}

		public bool HasAny(params string[] tagset)
		{
			return entity != null && entity.HasAny(tagset);
		}
		public bool Disposed { get; private set; }

		public virtual void Dispose()
		{
			if (entityDatabase != null)
			{
				entityDatabase.UnsubscribeEntity(this);
				entityDatabase = null;
			}
			if (entity != null)
			{
				entity.Dispose();
				entity = null;
			}
			Disposed = true;
		}
		#endregion

		#region Unity callbacks
		protected virtual void OnDestroy()
		{
			if (entity != null)
			{
				Dispose();
			}
		}

		protected virtual void Awake()
		{
			cachedTransform = transform;
		}
		#endregion

		#region private method
		public void InitializeEntity(EntityDatabase entityDatabase, params string[] tags)
		{
			this.entityDatabase = entityDatabase;
			entity = new Entity(tags, entityDatabase, false);

			entityDatabase.SubscribeEntity(this);
		}
		#endregion
	}
}
