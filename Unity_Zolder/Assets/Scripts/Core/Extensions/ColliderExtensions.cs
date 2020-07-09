// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class ColliderExtensions
	{
		public static Vector3 GetLocalCenter(this Collider col)
		{
			if (col is SphereCollider)
			{
				return ((SphereCollider)col).center;
			}
			else if (col is BoxCollider)
			{
				return ((BoxCollider)col).center;
			}
			else if (col is CapsuleCollider)
			{
				return ((CapsuleCollider)col).center;
			}
			else if (col is CharacterController)
			{
				return ((CharacterController)col).center;
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}

		public static Vector3 GetLocalCenter(this Collider2D col)
		{
			return col.offset;
		}

		public static Vector3 GetWorldCenter(this Collider col)
		{
			return col.transform.position + (col.transform.rotation * Vector3.Scale(GetLocalCenter(col), col.transform.lossyScale));
		}

		public static Vector3 GetWorldCenter(this Collider2D col)
		{
			return col.transform.position + (col.transform.rotation * Vector3.Scale(GetLocalCenter(col), col.transform.lossyScale));
		}

		public static Rect ScreenRect(this Collider orig, Camera cam)
		{
			if (orig is BoxCollider)
			{
				BoxCollider box = (BoxCollider)orig;
				Bounds b = new Bounds(box.center, box.size);
				Transform trans = orig.transform;
				return b.ScreenRect(trans, cam);
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}

		/// <summary>
		/// Workaround for Collider.Bounds as it does not work with rotations
		/// https://docs.unity3d.com/ScriptReference/Bounds.html
		/// </summary>
		/// <param name="collider">BoxCollider</param>
		/// <param name="Point">Point</param>
		/// <param name="margin">Margin that the point can be inside the box and still
		/// return false</param>
		/// <returns></returns>
		public static bool ColliderContainsPoint(this Collider col, Vector3 Point, float margin = 0)
		{
			if (col is BoxCollider)
			{
				BoxCollider boxCol = col as BoxCollider;
				Vector3 localPos = boxCol.transform.InverseTransformPoint(Point) - boxCol.center;
				float halfX = (boxCol.size.x * 0.5f) - margin;
				float halfY = (boxCol.size.y * 0.5f) - margin;
				float halfZ = (boxCol.size.z * 0.5f) - margin;

				if (localPos.x < halfX &&
					localPos.x > -halfX &&
					localPos.y < halfY &&
					localPos.y > -halfY &&
					localPos.z < halfZ &&
					localPos.z > -halfZ)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
