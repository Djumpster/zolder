// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Serialization
{
	public interface IEditorSerializedClass : IRuntimeSerializedClass
	{
		void Reset(object o);
		void AssignData(object o);
		IEditorSerializedClass Clone();
		IEditorSerializedClass GetOverrideClone();
		object InstantiateObject();
		bool DisplayDataGUI();
	}
}
