// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Text;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.CI
{
	/// <summary>
	/// Data class populated by the build process. Can be accessed during
	/// run-time to retrieve information about the current build. When in the
	/// Unity Editor these values can also be set.
	/// </summary>
	public class BuildInfoService : ScriptableObject
	{
		/// <summary>
		/// The source Git branch this build is based upon.
		/// </summary>
		public string SourceBranch
		{
#if UNITY_EDITOR
			set => sourceBranch = value;
#endif
			get => sourceBranch;
		}

		/// <summary>
		/// The source Git commit this build is based upon.
		/// </summary>
		public string GitCommitSHA
		{
#if UNITY_EDITOR
			set => gitCommitSha = value;
#endif
			get => gitCommitSha;
		}

		/// <summary>
		/// The human-readable string representation of the version
		/// number.
		/// </summary>
		public string Version
		{
#if UNITY_EDITOR
			set => version = value;
#endif
			get => version;
		}

		/// <summary>
		/// The build target of this build, in the format
		/// <c>BuildTargetGroup:BuildTarget</c>.
		/// </summary>
		public string BuildTarget
		{
#if UNITY_EDITOR
			set => buildTarget = value;
#endif
			get => buildTarget;
		}

		/// <summary>
		/// The name of the Stream this build was made for.
		/// </summary>
		[Obsolete("Streams are no longer being used in build scripts")]
		public string StreamName
		{
#if UNITY_EDITOR
			set => streamName = value;
#endif
			get => streamName;
		}

		/// <summary>
		/// The name of this build.
		/// </summary>
		public string BuildName
		{
#if UNITY_EDITOR
			set => buildName = value;
#endif
			get => buildName;
		}

		/// <summary>
		/// The date this build was made.
		/// </summary>
		public long BuildDate
		{
#if UNITY_EDITOR
			set => buildDate = value;
#endif
			get => buildDate;
		}

		/// <summary>
		/// The numeric representation of this build.
		/// </summary>
		public int BuildNumber
		{
#if UNITY_EDITOR
			set => buildNumber = value;
#endif
			get => buildNumber;
		}

		[SerializeField] private string sourceBranch;
		[SerializeField] private string gitCommitSha;
		[SerializeField] private string version;
		[SerializeField] private string buildTarget;
		[SerializeField] private string streamName;
		[SerializeField] private string buildName;
		[SerializeField] private long buildDate;
		[SerializeField] private int buildNumber;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("Build Date: ");
			builder.AppendLine(new DateTime(BuildDate).ToString());

			builder.Append("Build Target: ");
			builder.AppendLine(BuildTarget);

			builder.Append("Version: ");
			builder.AppendLine(Version);

			builder.Append("Build Name: ");
			builder.AppendLine(BuildName);

			builder.Append("Git commit: ");
			builder.AppendLine(GitCommitSHA);

			builder.Append("Git branch: ");
			builder.AppendLine(SourceBranch);

			return builder.ToString();
		}
	}
}
