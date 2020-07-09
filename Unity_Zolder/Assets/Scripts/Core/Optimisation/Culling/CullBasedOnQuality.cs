// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Settings;
using UnityEngine;

namespace Talespin.Core.Foundation.Optimisation
{
	public class CullBasedOnQuality : MonoBehaviour
	{
		[SerializeField] private GraphicsMode disableWhenEqualOrLowerTo = GraphicsMode.LOW;
		[SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();
		private GraphicsSettingsService settings;

		protected void Awake()
		{
			settings = GlobalDependencyLocator.Instance.Get<GraphicsSettingsService>();
			settings.SettingChangedEvent += OnSettingChangedEvent;
			Evaluate(settings.CurrentSetting);
		}

		void OnDestroy()
		{
			settings.SettingChangedEvent -= OnSettingChangedEvent;
		}

		private void OnSettingChangedEvent(GraphicsMode value)
		{
			Evaluate(value);
		}

		private void Evaluate(GraphicsMode value)
		{
			Set((int)value > (int)disableWhenEqualOrLowerTo);
		}

		private void Set(bool to)
		{
			if (objectsToDisable.Count == 0)
			{
				gameObject.SetActive(to);
			}
			foreach (GameObject go in objectsToDisable)
			{
				go.SetActive(to);
			}
		}
	}
}