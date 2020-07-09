// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	/// <summary>
	/// A behaviour that can be placed on object to make them reflect their environment. Registers itself to the PlanarReflectionService that does all the rendering work.
	/// This class is pretty much a container with all the needed variables to correctly render the reflections.
	/// 
	/// Although there is technically no limitation, this is only properly visualized on flat surfaces.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class ReflectionSurface : MonoBehaviour
	{
		public class ReflectionHandle
		{
			public int Resolution;
			public float ClippingPlaneOffset;
			public int LayerMask;
			public float Blurriness;
			public float Intensity;
			public string TextureParameterName;
			public string BlurParameterName;
			public string IntensityParameterName;
			public List<Material> Materials;
			public Transform Transform;
		}

		public event Action<ReflectionHandle> OnWillRenderObjectEvent = delegate { };

		[SerializeField] private int reflectionResolution = 512;
		[SerializeField] private float clippingPlaneOffset = 0.07f;
		[SerializeField] private LayerMaskAsset layerMask;
		[SerializeField, Range(0, 7)] private float blurriness = 0;
		[SerializeField, Range(0, 1)] private float intensity = 1;
		[SerializeField] private string reflectionTextureParameterName = "_ReflectionTexture";
		[SerializeField, Tooltip("Leave empty to control through material")] private string blurParameterName = "_ReflectionBlur";
		[SerializeField, Tooltip("Leave empty to control through material")] private string intensityParameterName = "_ReflectionIntensity";
		[SerializeField] private bool useSharedMaterials = false;

		private PlanarReflectionService planarReflectionService;
		private bool reflectionEnabled;

		private Renderer planarRenderer;
		private List<Material> materials;
		private bool usingSharedMaterials;

		private ReflectionHandle reflectionHandle;

		protected void OnEnable()
		{
			planarReflectionService = GlobalDependencyLocator.Instance.Get<PlanarReflectionService>();

			planarRenderer = GetComponent<Renderer>();
			materials = new List<Material>(useSharedMaterials ? planarRenderer.sharedMaterials : planarRenderer.materials);
			usingSharedMaterials = useSharedMaterials;

			if (materials.Count > 0)
			{
				reflectionHandle = new ReflectionHandle()
				{
					Resolution = Mathf.ClosestPowerOfTwo(reflectionResolution),
					ClippingPlaneOffset = clippingPlaneOffset,
					LayerMask = ~(1 << LayerMask.NameToLayer("Reflector")) & layerMask.Mask.value,
					Blurriness = blurriness,
					Intensity = intensity,
					TextureParameterName = reflectionTextureParameterName,
					BlurParameterName = blurParameterName,
					IntensityParameterName = intensityParameterName,
					Materials = materials,
					Transform = transform
				};
			}

			planarReflectionService.AddSurface(reflectionHandle, this);
		}

		protected void OnDisable()
		{
			planarReflectionService.RemoveSurface(reflectionHandle, this);

			reflectionHandle = null;
		}

		protected void OnWillRenderObject()
		{
			if (enabled && reflectionHandle != null && planarRenderer.enabled)
			{
				bool addedSurfaces = false;
#if UNITY_EDITOR
				if (DetectChanges())
				{
					addedSurfaces = true;
					planarReflectionService.RemoveSurface(reflectionHandle, this);
					planarReflectionService.AddSurface(reflectionHandle, this);
				}
#endif
				if (!addedSurfaces && planarReflectionService.EnableReflections && !reflectionEnabled)
				{
					planarReflectionService.RemoveSurface(reflectionHandle, this);
					planarReflectionService.AddSurface(reflectionHandle, this);
				}

				OnWillRenderObjectEvent(reflectionHandle);
				reflectionEnabled = planarReflectionService.EnableReflections;
			}
		}

		private bool DetectChanges()
		{
			bool pendingChanges = false;

			int newResolution = Mathf.ClosestPowerOfTwo(reflectionResolution);
			if (reflectionHandle.Resolution != newResolution)
			{
				reflectionHandle.Resolution = newResolution;
				pendingChanges = true;
			}

			if (reflectionHandle.ClippingPlaneOffset != clippingPlaneOffset)
			{
				reflectionHandle.ClippingPlaneOffset = clippingPlaneOffset;
				pendingChanges = true;
			}

			int newLayerMask = ~(1 << LayerMask.NameToLayer("Reflector")) & layerMask.Mask.value;
			if (reflectionHandle.LayerMask != newLayerMask)
			{
				reflectionHandle.LayerMask = newLayerMask;
				pendingChanges = true;
			}

			materials = new List<Material>(useSharedMaterials ? planarRenderer.sharedMaterials : planarRenderer.materials);
			if (usingSharedMaterials != useSharedMaterials || !reflectionHandle.Materials.SequenceEqual(materials))
			{
				reflectionHandle.Materials = materials;
				pendingChanges = true;
			}

			if (reflectionHandle.Blurriness != blurriness)
			{
				reflectionHandle.Blurriness = blurriness;
				pendingChanges = true;
			}

			if (reflectionHandle.Intensity != intensity)
			{
				reflectionHandle.Intensity = intensity;
				pendingChanges = true;
			}

			if (!reflectionHandle.BlurParameterName.Equals(blurParameterName))
			{
				reflectionHandle.BlurParameterName = blurParameterName;
				pendingChanges = true;
			}

			if (!reflectionHandle.IntensityParameterName.Equals(intensityParameterName))
			{
				reflectionHandle.IntensityParameterName = intensityParameterName;
				pendingChanges = true;
			}

			return pendingChanges;
		}
	}
}
