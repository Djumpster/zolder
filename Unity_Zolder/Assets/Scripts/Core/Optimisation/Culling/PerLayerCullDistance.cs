// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Settings;
using UnityEngine;

namespace Talespin.Core.Foundation.Optimisation
{
	[RequireComponent(typeof(Camera))]
	public class PerLayerCullDistance : MonoBehaviour
	{
		#region members
		[SerializeField] private bool keepUpdating = false;

		[SerializeField, HideInInspector] private float[] lowEndCullDistances;
		[SerializeField, HideInInspector] private float[] midEndcullDistances;
		[SerializeField, HideInInspector] private float[] highEndcullDistances;

		private Camera _cam;
		private Camera cam { get { if (_cam == null) { _cam = GetComponent<Camera>(); } return _cam; } }

		private GlobalEvents globalEvents;
		private GraphicsSettingsService _graphicsSettingsService;
		private GraphicsSettingsService graphicsSettingsService
		{
			get
			{
				if (_graphicsSettingsService == null)
				{
					_graphicsSettingsService = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
				}
				return _graphicsSettingsService;
			}
		}
		#endregion

		#region Unity callbacks
		protected void Start()
		{
			SetDistances();
		}

		protected void OnEnable()
		{
			globalEvents = GlobalDependencyLocator.Instance.Get<GlobalEvents>();

			globalEvents.Subscribe<GraphicsSettingsChangedEvent>(OnSettingsChanged);
		}

		protected void OnDisable()
		{
			globalEvents.Unsubscribe<GraphicsSettingsChangedEvent>(OnSettingsChanged);
		}

		protected void OnSettingsChanged(GraphicsSettingsChangedEvent evt)
		{
			SetDistances();
		}

		protected void Update()
		{
			if (keepUpdating)
			{
				SetDistances();
			}
		}
		#endregion

		#region public methods
		public void SetDistances()
		{
			float[] cullDistances = null;

			if (graphicsSettingsService.CurrentSetting == GraphicsMode.LOW)
			{
				if (Application.isPlaying)
				{
					LogUtil.Log(LogTags.GRAPHICS, this, "Setting cull distances for a low-spec device.");
				}
				cullDistances = lowEndCullDistances;
			}
			else if (graphicsSettingsService.CurrentSetting == GraphicsMode.MEDIUM)
			{
				if (Application.isPlaying)
				{
					LogUtil.Log(LogTags.GRAPHICS, this, "Setting cull distances for a medium-spec device.");
				}
				cullDistances = midEndcullDistances;
			}
			else
			{
				if (Application.isPlaying)
				{
					LogUtil.Log(LogTags.GRAPHICS, this, "Setting cull distances for a high-spec device.");
				}
				cullDistances = highEndcullDistances;
			}

			cam.layerCullDistances = cullDistances;
		}

		public float GetCullDistanceForLayer(int layer)
		{
			if (graphicsSettingsService.CurrentSetting == GraphicsMode.LOW)
			{
				return lowEndCullDistances[layer];
			}
			else if (graphicsSettingsService.CurrentSetting == GraphicsMode.MEDIUM)
			{
				return midEndcullDistances[layer];
			}
			else
			{
				return highEndcullDistances[layer];
			}
		}
		#endregion
	}
}