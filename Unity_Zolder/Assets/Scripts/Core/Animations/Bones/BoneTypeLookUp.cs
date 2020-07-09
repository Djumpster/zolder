// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Talespin.Core.Foundation.Reflection;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations.Bones
{
	/// <summary>
	/// Class to look up bones in a hierachy using BoneTypeDescriptors
	/// Construct with the root of the hierachy and use GetBone to get the actual component
	/// </summary>
	public class BoneTypeLookup
	{
		private readonly Dictionary<string, Transform> _boneLookup;

		private static List<BoneTypeDescriptor> _availableBones;

		public BoneTypeLookup(Component root)
		{
			_boneLookup = new Dictionary<string, Transform>();

			List<BoneTypeDescriptor> boneList = GetAvailableBoneTypes();

			// make sure all bones are in the lookup to avoid having to do the expensive 'ContainsKey' in the GetBone function.
			foreach (BoneTypeDescriptor boneType in boneList)
			{
				if (!_boneLookup.ContainsKey(boneType.Key))
				{
					_boneLookup.Add(boneType.Key, null);
				}
			}

			_boneLookup[""] = root.transform;

			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				//Don't take the first in the list.
				if (t == root.transform)
				{
					continue;
				}

				BoneTypeDescriptor boneType = boneList.FirstOrDefault(b => b.Name == t.name);
				if (boneType != null)
				{
					_boneLookup[boneType.Key] = t;
				}
			}
		}

		/// <summary>
		/// Gets the component with the name defined in the descriptor
		/// </summary>
		/// <param name="bone"></param>
		/// <returns></returns>
		public Transform GetBone(BoneTypeDescriptor bone)
		{
			//Root is not a valid bone type.
			if (bone.Key == "Root")
			{
				return _boneLookup[""];
			}

			// we avoid ContainsKey due to high memory trashing
			return _boneLookup[bone.Key];
		}

		/// <summary>
		/// Gets all the bonetypedescriptors using reflection and caches them for next time usage
		/// </summary>
		/// <returns></returns>
		public static List<BoneTypeDescriptor> GetAvailableBoneTypes()
		{
			if (_availableBones != null)
			{
				return _availableBones;
			}

			_availableBones = new List<BoneTypeDescriptor>();

			IEnumerable<Type> types = Reflect.AllTypesFrom<BoneTypeDescriptor>();

			foreach (Type type in types)
			{
				IEnumerable<FieldInfo> fieldInfos = Reflect.GetFieldInfos(type);
				object instance = FormatterServices.GetUninitializedObject(type);

				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					if (fieldInfo.FieldType == typeof(BoneTypeDescriptor))
					{
						var boneType = fieldInfo.GetValue(instance) as BoneTypeDescriptor;

						Debug.Assert(boneType != null, "No BoneTypes Found");

						// ReSharper disable once PossibleNullReferenceException
						boneType.Key = fieldInfo.Name;

						//TODO: currently bonetypes must be unique througout the entire code but this can be improved
						//TODO: in the future by taking into account the class and/or namespace
						Debug.Assert(_availableBones.All(b => b.Key != boneType.Key),
							"Duplicate BoneType:" + boneType.Key);

						_availableBones.Add(boneType);
					}
				}
			}

			return _availableBones;
		}

		/// <summary>
		/// Used to inject bones into the available bones array for lookup during runtime.
		/// NOTE: Make sure that you add bones at start of the application. Late additions can cause objects
		/// that use bonelookup to not contain these bones.
		/// </summary>
		/// <param name="descriptor">descriptions of the bones</param>
		public static void AddBones(params BoneTypeDescriptor[] descriptor)
		{
			if (_availableBones == null)
			{
				GetAvailableBoneTypes();
			}

			for (int i = 0; i < descriptor.Length; i++)
			{
				if (!_availableBones.Contains(descriptor[i]))
				{
					_availableBones.Add(descriptor[i]);
				}
			}
		}
	}
}
