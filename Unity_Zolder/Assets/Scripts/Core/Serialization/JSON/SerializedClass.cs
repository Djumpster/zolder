// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Serialization
{
	/// <summary>
	/// <para>
	/// You're probably looking for either <see cref="RuntimeSerializedClass{T}"/> or <see cref="EditorSerializedClass{T}"/>.
	/// They used to be combined in this class, but now the drawing logic has been separated from the data.
	/// </para>
	/// 
	/// <para>
	/// The purpose of this class now is to keep the existing interface the same, since the runtime (non editor)
	/// variant of the serialized class is now different from the editor implementation.
	/// </para>
	/// </summary>
	/// <seealso cref="RuntimeSerializedClass{T}"/>
	/// <seealso cref="EditorSerializedClass{T}"/>
	public class SerializedClass<T> :
#if UNITY_EDITOR
		EditorSerializedClass<T>
#else
		RuntimeSerializedClass<T>
#endif
		where T : class
	{
	}
}
