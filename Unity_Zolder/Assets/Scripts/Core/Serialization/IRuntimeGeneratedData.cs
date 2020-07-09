// Copyright 2020 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation
{
	/// <summary>
	/// The interface for the configuration data acompanying every <see cref="RuntimeGeneratedFlowData.LinkData">
	/// and <see cref="RuntimeGeneratedFlowData.StateData">
	/// This is injected into the actions and rules which then can use it as they see fit.
	/// </summary>
	public interface IRuntimeGeneratedData
	{
		/// <summary>
		/// Outputs all data contained in this class as a human-readable formatted string.
		/// </summary>
		string DebugInfo { get; }
	}
}
