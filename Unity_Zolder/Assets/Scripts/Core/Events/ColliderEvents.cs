// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Events
{
	public class ColliderEvents : MonoBehaviour
	{
		public event System.Action<Collider> TriggerEnterEvent = delegate { };
		public event System.Action<Collider> TriggerStayEvent = delegate { };
		public event System.Action<Collider> TriggerExitEvent = delegate { };
		public event System.Action<Collision> CollisionEnterEvent = delegate { };
		public event System.Action<Collision> CollisionStayEvent = delegate { };
		public event System.Action<Collision> CollisionExitEvent = delegate { };
		public event System.Action<ControllerColliderHit> ControllerColliderHitEvent = delegate { };

		public event System.Action<Collider2D> TriggerEnter2DEvent = delegate { };
		public event System.Action<Collider2D> TriggerStay2DEvent = delegate { };
		public event System.Action<Collider2D> TriggerExit2DEvent = delegate { };

		public event System.Action<Collision2D> CollisionEnter2DEvent = delegate { };
		public event System.Action<Collision2D> CollisionStay2DEvent = delegate { };
		public event System.Action<Collision2D> CollisionExit2DEvent = delegate { };

		protected void OnTriggerEnter(Collider collider)
		{
			TriggerEnterEvent(collider);
		}

		protected void OnTriggerExit(Collider collider)
		{
			TriggerExitEvent(collider);
		}

		protected void OnTriggerStay(Collider collider)
		{
			TriggerStayEvent(collider);
		}
		protected void OnTriggerEnter2D(Collider2D collider)
		{
			TriggerEnter2DEvent(collider);
		}

		protected void OnTriggerExit2D(Collider2D collider)
		{
			TriggerExit2DEvent(collider);
		}

		protected void OnTriggerStay2D(Collider2D collider)
		{
			TriggerStay2DEvent(collider);
		}

		protected void OnCollisionEnter(Collision collision)
		{
			CollisionEnterEvent(collision);
		}

		protected void OnCollisionStay(Collision collision)
		{
			CollisionStayEvent(collision);
		}

		protected void OnCollisionExit(Collision collision)
		{
			CollisionExitEvent(collision);
		}

		protected void OnControllerColliderHit(ControllerColliderHit collider)
		{
			ControllerColliderHitEvent(collider);
		}

		protected void OnCollisionEnter2D(Collision2D collision)
		{
			CollisionEnter2DEvent(collision);
		}

		protected void OnCollisionStay2D(Collision2D collision)
		{
			CollisionStay2DEvent(collision);
		}

		protected void OnCollisionExit2D(Collision2D collision)
		{
			CollisionExit2DEvent(collision);
		}
	}
}