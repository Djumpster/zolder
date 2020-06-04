using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour, IWallElement
{
	[SerializeField] private GameObject graphic;
	public void SetDepth(float depth)
	{
		Vector3 size = graphic.transform.localScale;
		Vector3 newSize = new Vector3(size.x, size.y, depth);
		graphic.transform.localScale = newSize;
	}
}
