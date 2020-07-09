// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Full-screen fade from one color to another.
	/// </summary>
	[System.Serializable]
	public class TimelineCameraFadePlayableBehaviour : PlayableBehaviour
	{
		public TimelineClip Clip
		{
			get { return clip; }
			set
			{
				clip = value;
				clip.displayName = "Fade";
			}
		}

		private TimelineClip clip;
		private bool alsoFadeUI;

		private Camera fadeCamera;
#pragma warning disable CS0618 // Type or member is obsolete
		private GUITexture fadeTexture;
#pragma warning restore CS0618 // Type or member is obsolete

		private Color fromColor;
		private Color toColor;
		private float progress = 0f;

		public void Initialize(Color fromColor, Color toColor, bool alsoFadeUI)
		{
			this.fromColor = fromColor;
			this.toColor = toColor;
			this.alsoFadeUI = alsoFadeUI;
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			base.PrepareFrame(playable, info);

			progress = (float)(playable.GetTime() / playable.GetDuration());
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame(playable, info, playerData);

			if (!Application.isPlaying)
			{
				DrawFade();
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			base.OnBehaviourPlay(playable, info);

			// doing this here instead of in Initialize so you can scrub the time line while not playing and repeat the clip.

			GameObject cam = new GameObject("FaderCamera")
			{
				layer = LayerMask.NameToLayer("FadeCameraLayer")
			};

			fadeCamera = cam.AddComponent<Camera>();
			fadeCamera.depth = 80;
			fadeCamera.clearFlags = CameraClearFlags.Depth;
			fadeCamera.cullingMask = 1 << LayerMask.NameToLayer("FadeCameraLayer");

#pragma warning disable CS0618 // Type or member is obsolete
			fadeTexture = fadeCamera.gameObject.AddComponent<GUITexture>();
#pragma warning restore CS0618 // Type or member is obsolete
			fadeTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);

			Texture2D blackTexture = new Texture2D(1, 1);
			blackTexture.SetPixel(0, 0, Color.white);
			blackTexture.Apply();
			fadeTexture.texture = blackTexture;

			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(fadeCamera.gameObject);
			}
#pragma warning disable CS0618 // Type or member is obsolete
			cam.AddComponent<GUILayer>();
#pragma warning restore CS0618 // Type or member is obsolete

			progress = (float)(playable.GetTime() / playable.GetDuration());
			fadeTexture.color = Color.Lerp(fromColor, toColor, Mathf.Clamp01(progress));
			fadeCamera.gameObject.SetActive(true);

			Camera.onPostRender -= OnCameraPostRender;
			Camera.onPostRender += OnCameraPostRender;

			if (!Application.isPlaying)
			{
				DrawFade();
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			base.OnBehaviourPause(playable, info);

			if (fadeCamera != null && fadeCamera.gameObject)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(fadeCamera.gameObject);
				}
				else
				{
					Object.DestroyImmediate(fadeCamera.gameObject);
				}
			}

			Camera.onPostRender -= OnCameraPostRender;
		}

		private void OnCameraPostRender(Camera cam)
		{
			DrawFade();
		}

		private void DrawFade()
		{
			if (fadeCamera != null)
			{
				fadeTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
				fadeTexture.color = Color.Lerp(fromColor, toColor, Mathf.Clamp01(progress));

				if (alsoFadeUI)
				{
					GL.PushMatrix();
					GL.LoadOrtho();
					UnityEngine.Graphics.DrawTexture
					(
						new Rect(0, 0, 1, 1),
						fadeTexture.texture,
						new Rect(0, 0, 1, 1),
						0, 0, 0, 0,
						fadeTexture.color
					);
					GL.PopMatrix();
				}
			}
		}
	}
}