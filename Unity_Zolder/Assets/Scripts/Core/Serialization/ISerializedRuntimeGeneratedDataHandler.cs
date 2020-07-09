// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation
{
	/// <summary>
	/// Implemented by <see cref="RuntimeSerializedClass"> to allow the <see cref="StateActionTemplate"> and <see cref="RuleTemplate">
	/// to be injected with the corresponding data exported from the story editor.
	/// Note: This is also implemented within the <see cref="EditorSerializedClass"> to avoid compilation errors
	/// </summary>
	public interface ISerializedRuntimeGeneratedDataHandler
	{
		void StoreRuntimeSerializedData(string type, string id, bool enabled, IRuntimeGeneratedData data);
	}
}
