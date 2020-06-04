using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour, IWallElement
{
	[SerializeField] private float distanceFromEdge = .004f;

	public void SetDepth(float depth)
	{
		Vector3 localPos = transform.localPosition;
		localPos.z = depth - distanceFromEdge;
		transform.localPosition = localPos;
	}
}
