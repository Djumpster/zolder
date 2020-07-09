// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace Talespin.Core.Foundation.PSDtouGUI
{
	/// <summary>
	/// Renders an outline around the UI element it is next to (likely text).
	/// Can also be used to render a drop shadow. 
	/// </summary>
	public class UIOutline : BaseMeshEffect
	{
		public int iterations;
		[SerializeField] private Vector2 distance;
		[SerializeField] private Color effectColor = new Color(0f, 0f, 0f, 1f);
		[SerializeField] private bool useGraphicAlpha = true;
		[SerializeField] private bool useDropShadow = false;
		[SerializeField] private Vector2 shadowDistance;
		[SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 1f);

		public void ModifyVertices(List<UIVertex> vertexList)
		{
			if (!IsActive())
			{
				return;
			}
			int startSize = vertexList.Count;
			int estimatedSize = vertexList.Count * (iterations + 1);
			estimatedSize = useDropShadow == true ? estimatedSize * 2 : estimatedSize;
			while (estimatedSize >= 65535)
			{
				if (useDropShadow)
				{
					LogUtil.Warning(LogTags.UI, this, "Outline is causing too many vertices, disabling drop shadow.");
					useDropShadow = false;
				}
				else if (iterations > 0)
				{
					LogUtil.Warning(LogTags.UI, this, "Outline is causing too many vertices, reducing iterations.");
					iterations--;
				}
				estimatedSize = vertexList.Count * (iterations + 1);
				estimatedSize = useDropShadow == true ? estimatedSize * 2 : estimatedSize;
			}
			vertexList.InsertRange(0, new UIVertex[estimatedSize - startSize]);
			float degreesPerIteration = 360f / iterations;
			for (int i = 0; i < iterations; i++)
			{
				Quaternion rot = Quaternion.AngleAxis(degreesPerIteration * i, Vector3.forward);
				Vector3 pos = rot * distance;
				AddShadow(estimatedSize - ((i + 1) * startSize) - startSize, startSize, vertexList, estimatedSize - startSize, effectColor, pos.x, pos.y, ref vertexList);
			}
			if (useDropShadow)
			{
				AddShadow(0, (int)(vertexList.Count * 0.5f),
								vertexList, ((int)(estimatedSize * 0.5f)), shadowColor, shadowDistance.x, shadowDistance.y, ref vertexList);
			}
		}

		protected void AddShadow(int startIndex, int count, List<UIVertex> verts, int vertsStartIndex, Color32 color, float x, float y, ref List<UIVertex> shadowVerts)
		{
			for (int i = 0; i < count; i++)
			{
				UIVertex uIVertex = verts[vertsStartIndex + i];
				UIVertex newUIVertex = new UIVertex
				{
					color = color
				};
				if (useGraphicAlpha)
				{
					newUIVertex.color.a = (byte)(color.a * uIVertex.color.a / 255);
				}
				newUIVertex.position = new Vector3(uIVertex.position.x + x, uIVertex.position.y + y, uIVertex.position.z);
				newUIVertex.normal = uIVertex.normal;
				newUIVertex.tangent = uIVertex.tangent;
				newUIVertex.uv0 = uIVertex.uv0;
				newUIVertex.uv1 = uIVertex.uv1;
				shadowVerts[startIndex + i] = newUIVertex;
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			List<UIVertex> vertexStream = new List<UIVertex>();
			vh.GetUIVertexStream(vertexStream);
			ModifyVertices(vertexStream);
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexStream);
		}
	}
}