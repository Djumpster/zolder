// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Talespin.Core.Foundation.PSDtouGUI
{
	/// <summary>
	/// Adds a gradient to the UI element it is next to.
	/// </summary>
#if UNITY_5_0 || UNITY_5_1
	public class GradientOverlay : BaseVertexEffect
#else
	public class GradientOverlay : BaseMeshEffect
#endif
	{
		public Color32 TopColor { get { return topColor; } set { topColor = value; } }
		[SerializeField] private Color32 topColor = Color.red;

		public Color32 BottomColor { get { return bottomColor; } set { bottomColor = value; } }
		[SerializeField] private Color32 bottomColor = Color.green;

#if UNITY_5_0 || UNITY_5_1
		public override void ModifyVertices(List<UIVertex> vertexList) 
#else
		public void ModifyVertices(List<UIVertex> vertexList)
#endif
		{
			if (!IsActive() || vertexList.Count == 0)
			{
				return;
			}

			Color topColor = this.topColor;
			Color bottomColor = this.bottomColor;

			int count = vertexList.Count;
			float bottomY = vertexList[0].position.y;
			float topY = vertexList[0].position.y;

			// Unity UI adds 4 mysterious misaligned vertices to the text and another 4 for each space.
			// These are all at the same position and need to be skipped to properly determine height.
			for (int j = 0; j < count - 3; j += 4)
			{
				float total = vertexList[j].position.y + vertexList[j + 1].position.y +
					vertexList[j + 2].position.y + vertexList[j + 3].position.y;

				if (Mathf.Approximately(total / 4f, vertexList[j].position.y))
				{
					continue;
				}

				for (int i = j; i < j + 4; i++)
				{
					float y = vertexList[i].position.y;

					if (y > topY)
					{
						topY = y;
					}
					else
					{
						if (y < bottomY)
						{
							bottomY = y;
						}
					}
				}
			}

			for (int i = 0; i < count; i++)
			{
				UIVertex uiVertex = vertexList[i];

				if (uiVertex.color.a != 0)
				{
					float prog = Mathf.InverseLerp(bottomY, topY, uiVertex.position.y);
					uiVertex.color = Color32.Lerp(bottomColor, topColor, prog);
					vertexList[i] = uiVertex;
				}
			}
		}

#if !UNITY_5_0 && !UNITY_5_1
		public override void ModifyMesh(VertexHelper vh)
		{
			List<UIVertex> vertexStream = new List<UIVertex>();
			vh.GetUIVertexStream(vertexStream);
			ModifyVertices(vertexStream);
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexStream);
		}
#endif
	}
}
