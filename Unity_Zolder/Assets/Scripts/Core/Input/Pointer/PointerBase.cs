// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// In most implementations you can extend this class instead of implementing <see cref="IPointer"/> yourself.
	/// This class handles pointer identification and registration.
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class PointerBase : MonoBehaviour, IPointer
	{
		public Transform Transform => sourceInfo.Transform;

		public string Tag => tag;
		public bool IsActive { get { return isActive; } }
		public PointerHit Hit { private set; get; }

		/// <summary>
		/// The layer mask setting for this specific pointer.
		/// </summary>
		public LayerMask InteractionLayerMask { get { return interactionLayerMask; } }

		public PointerInteractionEventData EventData { private set; get; }

		public int GUID => GetInstanceID();

		[ConstantTag(typeof(string), typeof(PointerTagsBase)), SerializeField] private new string tag = PointerTags.GENERIC;
		[SerializeField] protected PointerSourceInfo sourceInfo;
		[SerializeField] protected bool isActive = true;
		[SerializeField, Tooltip("The layers this Pointer is interacting with.")] protected LayerMask interactionLayerMask = ~Physics.IgnoreRaycastLayer;

		protected PointerInteractionService pointerService;

		protected virtual void Awake()
		{
			pointerService = GlobalDependencyLocator.Instance.Get<PointerInteractionService>();
		}

		protected virtual void OnEnable()
		{
			if (sourceInfo != null && Transform != null)
			{
				pointerService.RegisterPointer(this);
			}
		}

		protected virtual void OnDisable()
		{
			pointerService.UnregisterPointer(this);
		}

		protected virtual void LateUpdate()
		{
			if (!IsRegistered() && sourceInfo != null && Transform != null)
			{
				pointerService.RegisterPointer(this);
			}
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			sourceInfo = GetComponent<PointerSourceInfo>();
		}
#endif

		public virtual void SetHit(PointerHit hit)
		{
			Hit = hit;
		}

		public virtual void SetEventData(PointerInteractionEventData eventData)
		{
			EventData = eventData;
		}

		protected bool IsRegistered()
		{
			return pointerService.IsPointerRegistered(this);
		}

		public void SetPointerActive(bool isActive)
		{
			this.isActive = isActive;
		}
	}
}
