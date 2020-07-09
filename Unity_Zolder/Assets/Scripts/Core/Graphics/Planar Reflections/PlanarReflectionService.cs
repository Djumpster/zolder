// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	/// <summary>
	/// Service to handle all the work for rendering planar reflections. For every registered surface, a camera mirrored to the currently rendering camera will render its result into a rendertexture.
	/// Works for both non-VR and VR applications.
	/// </summary>
	public class PlanarReflectionService
	{
		public bool EnableReflections
		{
			get { return enableReflections; }
			set
			{
				OnToggleReflections(value);
				enableReflections = value;
			}
		}

		private readonly Hashtable reflectionCameras;
		private Hashtable reflectionHandles;

		private bool insideRendering;
		private bool enableReflections = true;

		public PlanarReflectionService()
		{
			reflectionCameras = new Hashtable();
			reflectionHandles = new Hashtable();
		}

		public void AddSurface(ReflectionSurface.ReflectionHandle handle, ReflectionSurface reflectionSurface)
		{
			reflectionSurface.OnWillRenderObjectEvent += OnWillRenderObject;
			GenerateRenderTextures(handle);
		}

		public void RemoveSurface(ReflectionSurface.ReflectionHandle handle, ReflectionSurface reflectionSurface)
		{
			reflectionSurface.OnWillRenderObjectEvent -= OnWillRenderObject;
			DestroyRenderTextures(handle);
		}

		private void OnWillRenderObject(ReflectionSurface.ReflectionHandle handle)
		{
			if (!EnableReflections)
			{
				return;
			}

			// Make sure we do not render recursively.
			if (insideRendering)
			{
				return;
			}

			insideRendering = true;

			Camera currentCamera = Camera.current;

			if (currentCamera == null)
			{
				return;
			}

			Camera reflectionCamera = GetOrCreateReflectionCamera(currentCamera);
			CopyCameraSettings(currentCamera, reflectionCamera);

			HandleReflections(handle, currentCamera, reflectionCamera);

			insideRendering = false;
		}

		private void OnToggleReflections(bool enabled)
		{
			var handles = new List<ReflectionSurface.ReflectionHandle>();
			foreach (ReflectionSurface.ReflectionHandle handle in reflectionHandles.Keys)
			{
				handles.Add(handle);
			}

			foreach (ReflectionSurface.ReflectionHandle handle in handles)
			{
				if (enabled)
				{
					GenerateRenderTextures(handle);
				}
				else
				{
					DestroyRenderTextures(handle);
				}
			}
		}

		private void HandleReflections(ReflectionSurface.ReflectionHandle handle, Camera currentCamera, Camera reflectionCamera)
		{
			if (reflectionHandles.Count == 0)
			{
				return;
			}

			Vector3 eyePos = currentCamera.transform.position;
			Matrix4x4 projectionMatrix = currentCamera.projectionMatrix;

			RenderTexture[] reflectionTextures = reflectionHandles[handle] as RenderTexture[];
			int textureIndex = 0;

			if (currentCamera.stereoEnabled)
			{
				if (currentCamera.stereoTargetEye == StereoTargetEyeMask.Both || currentCamera.stereoTargetEye == StereoTargetEyeMask.Left)
				{
					eyePos = currentCamera.transform.TransformPoint(new Vector3(-0.5f * currentCamera.stereoSeparation, 0, 0));
					projectionMatrix = currentCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
					textureIndex = 0;

					RenderReflection(handle, reflectionCamera, reflectionTextures[textureIndex], eyePos, currentCamera.transform.rotation, projectionMatrix);
					PassToMaterials(handle, reflectionTextures[textureIndex], textureIndex);
				}

				if (currentCamera.stereoTargetEye == StereoTargetEyeMask.Both || currentCamera.stereoTargetEye == StereoTargetEyeMask.Right)
				{
					eyePos = currentCamera.transform.TransformPoint(new Vector3(0.5f * currentCamera.stereoSeparation, 0, 0));
					projectionMatrix = currentCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
					textureIndex = 1;

					RenderReflection(handle, reflectionCamera, reflectionTextures[textureIndex], eyePos, currentCamera.transform.rotation, projectionMatrix);
					PassToMaterials(handle, reflectionTextures[textureIndex], textureIndex);
				}
			}
			else
			{
				RenderReflection(handle, reflectionCamera, reflectionTextures[textureIndex], eyePos, currentCamera.transform.rotation, projectionMatrix);
				PassToMaterials(handle, reflectionTextures[textureIndex], textureIndex);
			}
		}

		private void PassToMaterials(ReflectionSurface.ReflectionHandle handle, RenderTexture reflectionTexture, int textureIndex)
		{
			for (int i = 0; i < handle.Materials.Count; i++)
			{
				if (handle.Materials[i].HasProperty(handle.TextureParameterName + textureIndex))
				{
					handle.Materials[i].SetTexture(handle.TextureParameterName + textureIndex, reflectionTexture);
				}

				if (!string.IsNullOrEmpty(handle.BlurParameterName))
				{
					handle.Materials[i].SetFloat(handle.BlurParameterName, handle.Blurriness);
				}

				if (!string.IsNullOrEmpty(handle.IntensityParameterName))
				{
					handle.Materials[i].SetFloat(handle.IntensityParameterName, handle.Intensity);
				}
			}
		}

		private void RenderReflection(ReflectionSurface.ReflectionHandle handle, Camera reflectionCamera, RenderTexture targetTexture, Vector3 camPos, Quaternion camRot, Matrix4x4 camProjMatrix)
		{
			if (reflectionCamera.orthographic)
			{
				return;
			}

			reflectionCamera.ResetWorldToCameraMatrix();
			reflectionCamera.transform.position = camPos;
			reflectionCamera.transform.rotation = camRot;
			reflectionCamera.projectionMatrix = camProjMatrix;
			reflectionCamera.targetTexture = targetTexture;
			reflectionCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
			reflectionCamera.useOcclusionCulling = false;

			Vector3 pos = handle.Transform.position;
			Vector3 normal = handle.Transform.up;

			float d = -Vector3.Dot(normal, pos) - handle.ClippingPlaneOffset;
			Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

			Matrix4x4 reflection = CalculateReflectionMatrix(reflectionPlane);

			reflectionCamera.worldToCameraMatrix *= reflection;

			Vector4 clipPlane = CalculateCameraSpacePlane(reflectionCamera, pos, normal, 1.0f, handle.ClippingPlaneOffset);
			reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(clipPlane);

			reflectionCamera.transform.position = reflectionCamera.cameraToWorldMatrix.GetColumn(3);
			reflectionCamera.transform.rotation = Quaternion.LookRotation(reflectionCamera.cameraToWorldMatrix.GetColumn(2), reflectionCamera.cameraToWorldMatrix.GetColumn(1));

			reflectionCamera.cullingMask = handle.LayerMask;

			RenderSettings.fog = false;

			bool oldCulling = GL.invertCulling;
			GL.invertCulling = !oldCulling;

			reflectionCamera.Render();

			GL.invertCulling = oldCulling;
			RenderSettings.fog = true;
		}

		private void CopyCameraSettings(Camera src, Camera dest)
		{
			if (dest == null)
			{
				return;
			}

			dest.clearFlags = src.clearFlags;
			dest.backgroundColor = src.backgroundColor;
			if (src.clearFlags == CameraClearFlags.Skybox)
			{
				Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
				Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
				if (!sky || !sky.material)
				{
					mysky.enabled = false;
				}
				else
				{
					mysky.enabled = true;
					mysky.material = sky.material;
				}
			}

			dest.farClipPlane = src.farClipPlane;
			dest.nearClipPlane = src.nearClipPlane;
			dest.orthographic = src.orthographic;
			if (!src.stereoEnabled)
			{
				dest.fieldOfView = src.fieldOfView;
			}
			dest.aspect = src.aspect;
			dest.orthographicSize = src.orthographicSize;
		}

		private Camera GetOrCreateReflectionCamera(Camera currentCamera)
		{
			Camera reflectionCamera = reflectionCameras[currentCamera] as Camera;

			if (!reflectionCamera)
			{
				GameObject go = new GameObject("Mirror Refl Camera for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
				reflectionCamera = go.GetComponent<Camera>();
				reflectionCamera.enabled = false;
				reflectionCamera.transform.position = currentCamera.transform.position;
				reflectionCamera.transform.rotation = currentCamera.transform.rotation;
				reflectionCamera.gameObject.AddComponent<FlareLayer>();
				go.hideFlags = HideFlags.HideAndDontSave;
				reflectionCameras[currentCamera] = reflectionCamera;
			}

			return reflectionCamera;
		}

		private Vector4 CalculateCameraSpacePlane(Camera reflectionCamera, Vector3 planePosition, Vector3 planeNormal, float sideSign, float clippingPlaneOffset)
		{
			Vector3 offsetPos = planePosition + planeNormal * clippingPlaneOffset;
			Matrix4x4 m = reflectionCamera.worldToCameraMatrix;
			Vector3 cpos = m.MultiplyPoint(offsetPos);
			Vector3 cnormal = m.MultiplyVector(planeNormal).normalized * sideSign;
			return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
		}

		private Matrix4x4 CalculateReflectionMatrix(Vector4 reflectionPlane)
		{
			Matrix4x4 reflectionMat = Matrix4x4.identity;

			reflectionMat.m00 = (1F - 2F * reflectionPlane[0] * reflectionPlane[0]);
			reflectionMat.m01 = (-2F * reflectionPlane[0] * reflectionPlane[1]);
			reflectionMat.m02 = (-2F * reflectionPlane[0] * reflectionPlane[2]);
			reflectionMat.m03 = (-2F * reflectionPlane[3] * reflectionPlane[0]);

			reflectionMat.m10 = (-2F * reflectionPlane[1] * reflectionPlane[0]);
			reflectionMat.m11 = (1F - 2F * reflectionPlane[1] * reflectionPlane[1]);
			reflectionMat.m12 = (-2F * reflectionPlane[1] * reflectionPlane[2]);
			reflectionMat.m13 = (-2F * reflectionPlane[3] * reflectionPlane[1]);

			reflectionMat.m20 = (-2F * reflectionPlane[2] * reflectionPlane[0]);
			reflectionMat.m21 = (-2F * reflectionPlane[2] * reflectionPlane[1]);
			reflectionMat.m22 = (1F - 2F * reflectionPlane[2] * reflectionPlane[2]);
			reflectionMat.m23 = (-2F * reflectionPlane[3] * reflectionPlane[2]);

			reflectionMat.m30 = 0F;
			reflectionMat.m31 = 0F;
			reflectionMat.m32 = 0F;
			reflectionMat.m33 = 1F;

			return reflectionMat;
		}

		private void GenerateRenderTextures(ReflectionSurface.ReflectionHandle handle)
		{
			RenderTexture[] renderTextures = { GenerateRenderTexture(handle.Resolution, "Reflection_Left"), GenerateRenderTexture(handle.Resolution, "Reflection_Right") };
			reflectionHandles[handle] = renderTextures;
		}

		private RenderTexture GenerateRenderTexture(int resolution, string name)
		{
			RenderTexture renderTexture = new RenderTexture(resolution, resolution, 16)
			{
				name = name,
				isPowerOfTwo = true,
				hideFlags = HideFlags.DontSave,
				useMipMap = true,
				autoGenerateMips = true
			};

			return renderTexture;
		}

		private void DestroyRenderTextures(ReflectionSurface.ReflectionHandle handle)
		{
			RenderTexture[] renderTextures = reflectionHandles[handle] as RenderTexture[];

			if (renderTextures == null)
			{
				return;
			}

			for (int i = 0; i < renderTextures.Length; i++)
			{
				GameObject.DestroyImmediate(renderTextures[i]);
				renderTextures[i] = null;
			}

			reflectionHandles.Remove(handle);
		}
	}
}
