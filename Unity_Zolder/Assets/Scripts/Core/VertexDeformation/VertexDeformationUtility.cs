// Copyright 2019 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
// Uncomment to enable debug rays when vertices move.
//#define VISUALIZE_DEFORMATIONS
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.VertexDeformation
{
	/// <summary>
	/// <para>
	/// The vertex deformation utility can be used to
	/// attach the vertices of the mesh on the same object
	/// to another mesh.
	/// </para>
	/// 
	/// <para>
	/// Usage:
	/// Attach this component to any SkinnedMeshRenderer who'se
	/// mesh should be parented to another, link the target SkinnedMeshRenderer
	/// in the Target field and use the Add and Add Multiple buttons to add anchor
	/// points on the surface of the target's mesh.
	/// 
	/// When the target mesh is modified via blendshapes, the vertices on the source object
	/// will be updated to blend and mostly match.
	/// </para>
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SkinnedMeshRenderer))]
	public class VertexDeformationUtility : MonoBehaviour
	{
		// Every field in this class is public for performance reasons with the editor script.
		// SerializedObject and SerializedProperty cannot properly deal with a MonoBehaviour
		// of this size; serializedObject.Update() alone could take up to 10ms for small meshes.

		// Pre-define an array holding 256 entries. This cannot be more because
		// a byte is used to internally keep track of deformation anchors.
		[SerializeField] public DeformationAnchor[] DeformationAnchors = new DeformationAnchor[256];

		[SerializeField] public List<VertexData> VertexDatas = new List<VertexData>();
		[SerializeField] public List<VertexDeformationData> VertexDeformationDatas = new List<VertexDeformationData>();

		[SerializeField] public SkinnedMeshRenderer Target;
		[SerializeField] public CachedData Cache;

		[SerializeField] public byte NumDeformationAnchors;

		protected void LateUpdate()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (HasAnyMeshChanged())
				{
					TryUpdateCache();
				}

				if (Target && !Mathf.Approximately(Target.transform.lossyScale.sqrMagnitude, Cache.TargetScale))
				{
					TryUpdateBlendShapeWeights(true);
				}
			}
#endif

			if (Target && TryUpdateBlendShapeWeights() && NumDeformationAnchors > 0)
			{
				UpdateVertices();
			}
		}

		/// <summary>
		/// Check if blendshape weights have been updated,
		/// and rebake the target mesh if so.
		/// </summary>
		/// <param name="forceUpdate">Set this to <see langword="true" /> if a bake
		/// should be forced regardless of blendshape updates</param>
		/// <returns><see langword="true" /> if the target mesh has been baked</returns>
		private bool TryUpdateBlendShapeWeights(bool forceUpdate = false)
		{
			bool changed = false;

			for (int i = 0; i < Cache.BlendShapeWeights.Length; i++)
			{
				float weight = Target.GetBlendShapeWeight(i);

				if (!Mathf.Approximately(weight, Cache.BlendShapeWeights[i]))
				{
					changed = true;
				}

				Cache.BlendShapeWeights[i] = weight;
			}

			if (changed || forceUpdate)
			{
				Cache.TargetMeshBaked = new Mesh();
				Cache.Target.BakeMesh(Cache.TargetMeshBaked);

				Cache.TargetMeshBakedVertices.Clear();
				Cache.TargetMeshBaked.GetVertices(Cache.TargetMeshBakedVertices);

				Cache.TargetScale = Target.transform.lossyScale.sqrMagnitude;
			}

			return changed || forceUpdate;
		}

		/// <summary>
		/// Update the vertices of the source mesh.
		/// This will calculate a delta for every vertex to
		/// every anchor point and process that with a weight.
		/// </summary>
		private void UpdateVertices()
		{
			Vector3[] newVertices = new Vector3[Cache.SelfMeshVertices.Count];

			for (int i = 0; i < newVertices.Length; i++)
			{
				VertexData vertexData = VertexDatas[i];

#if UNITY_EDITOR && VISUALIZE_DEFORMATIONS
				Vector3 initialVertexPosition = Vector3.zero;
#endif

				Vector3 vertexPositionDelta = Vector3.zero;
				int numDeformationAnchorsInRange = 0;


				for (byte j = 0; j < NumDeformationAnchors; j++)
				{
					VertexDeformationData vertexDeformationData = VertexDeformationDatas[NumDeformationAnchors * i + j];
					DeformationAnchor deformationAnchor = DeformationAnchors[j];

#if UNITY_EDITOR && VISUALIZE_DEFORMATIONS
					if(i == 0)
					{
						if(j == 0)
						{
							initialVertexPosition = deformationAnchor.InitialVertexPosition - vertexDeformationData.AnchorPointOffset;
						}

						Debug.DrawLine(initialVertexPosition, deformationAnchor.InitialVertexPosition, Color.green, 1);
					}
#endif

					// Calculate the delta of this anchor point
					Vector3 deformationAnchorPosition = Cache.TargetMeshBakedVertices[deformationAnchor.VertexIndex];
					Vector3 deformationAnchorPositionDelta = deformationAnchorPosition - deformationAnchor.InitialVertexPosition;

#if UNITY_EDITOR && VISUALIZE_DEFORMATIONS
					if(i == 0)
					{
						Debug.DrawRay(deformationAnchorPosition, deformationAnchorPositionDelta, Color.yellow, 1);

						Vector3 vertexPosition = initialVertexPosition + deformationAnchorPositionDelta;
						Debug.DrawLine(initialVertexPosition, vertexPosition, Color.red, 1);
					}
#endif

					// Calculate the weight this anchor point should have on this vertex
					float weight = 0;
					float distance = vertexDeformationData.AnchorPointOffset.sqrMagnitude;

					if (distance < deformationAnchor.Radius * deformationAnchor.Radius)
					{
						weight = 1 - (Mathf.Sqrt(distance) / deformationAnchor.Radius);
						numDeformationAnchorsInRange++;
					}

					Vector3 weightedDeformationAnchorPositionDelta = deformationAnchorPositionDelta * weight;

					// Apply the weighted delta to the vertex delta
					vertexPositionDelta += weightedDeformationAnchorPositionDelta;
				}

				if (numDeformationAnchorsInRange > 0)
				{
					// Normalize the vertex delta with the amount of deformation anchors in range
					vertexPositionDelta /= numDeformationAnchorsInRange;
				}

				// Apply the new vertex position
				newVertices[vertexData.VertexIndex] = vertexData.InitialVertexPosition + vertexPositionDelta;

#if UNITY_EDITOR && VISUALIZE_DEFORMATIONS
				if(i == 0)
				{
					Debug.DrawRay(initialVertexPosition, vertexPositionDelta, Color.blue, 1);
					Debug.Log($"Vertex Index={vertexData.VertexIndex} New Vertex Position={newVertices[vertexData.VertexIndex].ToString("F4")} Old Vertex Position={Cache.SelfMesh.vertices[vertexData.VertexIndex].ToString("F4")} Initial Vertex Position={vertexData.InitialVertexPosition.ToString("F4")} Delta={vertexPositionDelta.ToString("F4")}");
				}
#endif
			}

			Cache.SelfMesh.vertices = newVertices;
			Cache.SelfMesh.UploadMeshData(false);
		}

#if UNITY_EDITOR
		protected void Awake()
		{
			if (!Application.isPlaying)
			{
				if (Cache == null)
				{
					Cache = new CachedData();
					TryUpdateCache();
				}
			}
		}

		protected void OnDestroy()
		{
			if (Cache != null && Cache.Self)
			{
				Cache.Self.sharedMesh = Cache.SelfOriginalMesh;
			}
		}

		protected void OnDrawGizmosSelected()
		{
			if (Cache == null || !Target || !enabled)
			{
				return;
			}

			Gizmos.color = Color.green;
			Matrix4x4 worldMatrix = Matrix4x4.TRS(Target.transform.position, Target.transform.rotation, Vector3.one);

			for (byte i = 0; i < NumDeformationAnchors; i++)
			{
				Vector3 vertex = Cache.TargetMeshBakedVertices[DeformationAnchors[i].VertexIndex];
				Vector3 vertexPosition = worldMatrix.MultiplyPoint3x4(vertex);

				Gizmos.DrawWireSphere(vertexPosition, DeformationAnchors[i].Radius);
			}
		}

		/// <summary>
		/// Calculate the offset to every
		/// anchor point for every vertex.
		/// </summary>
		public void CalculateVertexAnchorPointOffsets()
		{
			for (int i = 0; i < Cache.SelfMeshVertices.Count; i++)
			{
				VertexData vertex = VertexDatas[i];

				// Calculate the distance to all deformation anchors
				for (byte j = 0; j < NumDeformationAnchors; j++)
				{
					Vector3 position = Cache.TargetMeshVertices[DeformationAnchors[j].VertexIndex];
					Vector3 distance = position - vertex.InitialVertexPosition;

					VertexDeformationDatas[NumDeformationAnchors * i + j] = new VertexDeformationData
					{
						AnchorPointOffset = distance
					};
				}
			}
		}

		/// <summary>
		/// Check if:
		/// <list type="bullet">
		/// <item><description>The Target was updated</description></item>
		/// <item><description>The Target's mesh was updated</description></item>
		/// <item><description>Our SkinnedMeshRenderer was updated</description></item>
		/// <item><description>Our SkinnedMeshRenderer's mesh was updated</description></item>
		/// </list>
		/// 
		/// And recalculate everything required if so.
		/// </summary>
		private void TryUpdateCache()
		{
			bool requireRecalculation = false;

			if (Target != Cache.Target)
			{
				Cache.Target = Target;
				Cache.BlendShapeWeights = new float[0];
				Cache.TargetMesh = null;
				Cache.TargetMeshVertices = new List<Vector3>();
				Cache.TargetMeshBaked = null;
				Cache.TargetMeshBakedVertices = new List<Vector3>();

				if (Target)
				{
					Cache.TargetMesh = Target.sharedMesh;
					Cache.TargetScale = Target.transform.lossyScale.sqrMagnitude;

					if (Target.sharedMesh)
					{
						Cache.TargetMesh.GetVertices(Cache.TargetMeshVertices);

						Cache.BlendShapeWeights = new float[Target.sharedMesh.blendShapeCount];
						Cache.TargetMeshBakedVertices.Capacity = Target.sharedMesh.vertexCount;

						TryUpdateBlendShapeWeights(true);
					}

					requireRecalculation = true;
				}

				NumDeformationAnchors = 0;
			}

			SkinnedMeshRenderer self = GetComponent<SkinnedMeshRenderer>();

			if (self != Cache.Self || (self && self.sharedMesh != Cache.SelfMesh))
			{
				Cache.Self = self;
				Cache.SelfOriginalMesh = null;
				Cache.SelfMesh = null;
				Cache.SelfMeshVertices = new List<Vector3>();

				if (self && self.sharedMesh)
				{
					Cache.SelfOriginalMesh = Cache.Self.sharedMesh;
					Cache.Self.sharedMesh = Instantiate(Cache.Self.sharedMesh);

					Cache.SelfMesh = Cache.Self.sharedMesh;
					Cache.SelfMesh.MarkDynamic();
					Cache.SelfMesh.GetVertices(Cache.SelfMeshVertices);

					// Create new VertexDatas if required
					int newVertexDatasCount = Mathf.Max(0, Cache.SelfMesh.vertexCount - VertexDatas.Count);
					VertexData[] newVertexDatas = new VertexData[newVertexDatasCount];
					VertexDatas.AddRange(newVertexDatas);

					// Create new VertexDeformationDatas if required
					int numRequiredVertexDeformationDatas = Cache.SelfMeshVertices.Count * NumDeformationAnchors;
					int newVertexDeformationDatasCount = Mathf.Max(0, numRequiredVertexDeformationDatas - VertexDeformationDatas.Count);
					VertexDeformationData[] newVertexDeformationDatas = new VertexDeformationData[newVertexDeformationDatasCount];
					VertexDeformationDatas.AddRange(newVertexDeformationDatas);

					// Initialize VertexDatas
					for (int i = 0; i < Cache.SelfMeshVertices.Count; i++)
					{
						VertexDatas[i] = new VertexData
						{
							VertexIndex = i,
							InitialVertexPosition = Cache.SelfMeshVertices[i]
						};
					}
				}

				requireRecalculation = true;
			}

			if (requireRecalculation && Target)
			{
				CalculateVertexAnchorPointOffsets();
			}
		}

		private bool HasAnyMeshChanged()
		{
			if (Cache == null)
			{
				return true;
			}

			if (Target != Cache.Target)
			{
				return true;
			}

			if (Target && Target.sharedMesh != Cache.TargetMesh)
			{
				return true;
			}

			SkinnedMeshRenderer self = GetComponent<SkinnedMeshRenderer>();
			if (self != Cache.Self)
			{
				return true;
			}

			if (self && self.sharedMesh != Cache.SelfMesh)
			{
				return true;
			}

			return false;
		}

		[ContextMenu("Clear Cache")]
		private void ForceClearCache()
		{
			if (Cache != null && Cache.Self)
			{
				Cache.Self.sharedMesh = Cache.SelfOriginalMesh;
			}

			VertexDatas = new List<VertexData>();

			TryUpdateCache();
		}
#endif
	}
}
