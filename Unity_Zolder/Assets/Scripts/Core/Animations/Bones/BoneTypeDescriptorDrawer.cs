// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Serialization.Drawer;
using UnityEditor;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Bones
{
	[SerializedClassDrawer(TargetType = typeof(BoneTypeDescriptor))]
	public class BoneTypeDescriptorDrawer : ISerializedClassDrawer
	{
		/// <summary>
		/// Currently used for monobehaviours
		/// </summary>
		/// <param name="currentBone"></param>
		/// <param name="position"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public static BoneTypeDescriptor BoneTypePopup(BoneTypeDescriptor currentBone, Rect position, GUIContent label)
		{
			List<BoneTypeDescriptor> availableBones = BoneTypeLookup.GetAvailableBoneTypes();

			int selected = EditorGUI.Popup(position, label.text, GetSelectedIndex(currentBone, availableBones),
				availableBones.Select(b => b.Key).ToArray());

			BoneTypeDescriptor selectedBone = availableBones[selected];
			return selectedBone;
		}

		private static int GetSelectedIndex(BoneTypeDescriptor currentBone, List<BoneTypeDescriptor> availableBones)
		{
			int selectedIndex = -1;

			if (currentBone != null)
			{
				selectedIndex = availableBones.FindIndex(b => b.Key == currentBone.Key);
			}

			if (selectedIndex == -1)
			{
				selectedIndex = 0;
			}

			return selectedIndex;
		}

		public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
		{
			List<BoneTypeDescriptor> availableBones = BoneTypeLookup.GetAvailableBoneTypes();
			int selected = EditorGUILayout.Popup(label.text, GetSelectedIndex((BoneTypeDescriptor)value, availableBones), availableBones.Select(b => b.Key).ToArray());
			return availableBones[selected];
		}
	}
}
#endif