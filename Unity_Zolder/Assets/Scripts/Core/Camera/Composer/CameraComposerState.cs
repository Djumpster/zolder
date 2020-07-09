// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

namespace Talespin.Core.Foundation.Cameras.Composer
{
	/// <summary>
	/// All available states that the camera composer can be in.
	/// </summary>
	public enum CameraComposerState
	{
		/// <summary>
		/// The composer is currently not active.
		/// </summary>
		Ended,

		/// <summary>
		/// The composer is starting, it is currently transitioning
		/// from <see cref="Ended"/> to <see cref="Started"/>.
		/// <para>
		/// The state of the composer will update to <see cref="Started"/>
		/// once its animation has finished.
		/// </para>
		/// </summary>
		Starting,

		/// <summary>
		/// The composer is currently active.
		/// </summary>
		Started,

		/// <summary>
		/// This composer is ending, it is currently transitioning
		/// from <see cref="Started"/> to <see cref="Ended"/>.
		/// <para>
		/// The state of the composer will update to <see cref="Ended"/>
		/// once its animation has finished.
		/// </para>
		/// </summary>
		Ending,
	}
}
