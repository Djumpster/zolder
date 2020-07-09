// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.AppName
{
	/// <summary>
	/// This library is used to initialize the application and establish
	/// a brainCloud connection.
	/// <para>
	/// There are no hard-coded app ids or secrets, the system will automatically
	/// attempt to retrieve available organizations for the device based on:
	/// <list type="bullet">
	/// <item>IP address</item>
	/// <item>Device GUID</item>
	/// <item>Device hardware identifiers</item>
	/// </list>
	/// </para>
	/// <para>
	/// If there are none or multiple organizations available for the device
	/// the user will be presented with an organization selection screen which
	/// which will be used to retrieve configuration data for the specified organization.
	/// </para>
	/// </summary>
	/// <seealso cref="BrainCloudOrgConfigService"/>
	/// <seealso cref="BrainCloudAppConfigService"/>
	public class AppNameLibrary : ScriptableObject
	{
		/// <summary>
		/// The <see cref="PlayerPrefs"/> identifier used to store the
		/// cached organization identifier.
		/// </summary>
		public const string CACHED_ORG_ID_PLAYER_PREFS_KEY = "cached_org_id";

		/// <summary>
		/// This should be left empty in most cases, but can be filled in
		/// to override the automatic detection system and always use the
		/// specified organization name.
		/// </summary>
		public string OrgKeyName => orgKeyName;

		/// <summary>
		/// This is the application key name to identify the correct back-end with.
		/// Examples:
		/// <list type="bullet">
		/// <item>performanceFeedback</item>
		/// <item>sharedResponsibility</item>
		/// </list>
		/// </summary>
		public ICollection<string> AppKeyNames => appKeyNames;

		[Tooltip("If left empty the application will automatically try to detect the organization name")]
		[SerializeField] private string orgKeyName;
		[SerializeField] private string[] appKeyNames;
	}
}
