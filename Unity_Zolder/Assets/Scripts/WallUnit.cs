using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUnit : MonoBehaviour
{
	[SerializeField] private float shelfDepth = .32f;

	private IWallElement[] allElements;

	private float currentDepth = 0f;

	private void Start()
	{
		allElements = gameObject.GetComponentsInChildren<IWallElement>();
	}

	private void AdjustDepth ()
	{
		foreach (var shelf in allElements)
		{
			shelf.SetDepth(shelfDepth);
		}
		currentDepth = shelfDepth;
	}

	void Update()
    {
        if (!Mathf.Approximately(currentDepth, shelfDepth))
		{
			AdjustDepth();
		}
    }
}
