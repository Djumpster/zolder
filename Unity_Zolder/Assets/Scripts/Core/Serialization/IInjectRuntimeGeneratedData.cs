// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;

namespace Talespin.Core.Foundation
{
	/// <summary>
	/// Implement this interface on any Action or Rule to get access to the data exported
	/// from the Story Editor for this specific Node in the flow
	/// </summary>
	public interface IInjectRuntimeGeneratedData
	{
		/// <summary>
		/// This Function is called right after deserialization (i.e. before everything else)
		/// 
		/// NOTE: It will NOT get called if the <see cref="IRuntimeGeneratedData"> in the parameter is null
		/// </summary>
		void InjectRuntimeGeneratedData(IRuntimeGeneratedData runtimeGeneratedData, IDependencyInjector dependencyInjector);
	}
}
