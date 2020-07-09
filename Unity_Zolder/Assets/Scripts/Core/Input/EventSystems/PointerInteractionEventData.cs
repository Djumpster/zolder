// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Custom EventData designed for PointerSources.
	/// This class should contain all data that's required for
	/// the various pointer implementations.
	/// 
	/// Used internally by the InputModule only.
	/// </summary>
	public class PointerInteractionEventData : PointerEventData
	{
		public enum PointerType
		{
			Invalid = 0,
			Ray = 1,
			Spot = 2
		}

		public Vector3 Origin { set; get; }
		public Vector3 Direction { set; get; }

		public float Radius { set; get; }

		public LayerMask InteractionLayerMask { get; set; }

		public PointerType Type { set; get; }

		public string Tag { set; get; }

		public PointerInteractionEventData(EventSystem eventSystem) : base(eventSystem)
		{
			Origin = Vector3.zero;
			Direction = Vector3.zero;

			Radius = 0;
			Type = PointerType.Invalid;
			Tag = PointerTags.GENERIC;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("<b>Type</b>: " + Type);
			sb.AppendLine("<b>Tag</b>: " + Tag);
			sb.AppendLine("<b>Origin</b>: " + Origin);

			if (Type == PointerType.Ray)
			{
				sb.AppendLine("<b>Direction</b>: " + Direction);
			}
			else if (Type == PointerType.Spot)
			{
				sb.AppendLine("<b>Radius</b>: " + Radius);
			}

			sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
			sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
			sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
			sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
			sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
			sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
			sb.AppendLine("<b>Current Rayast:</b>");
			sb.AppendLine(pointerCurrentRaycast.ToString());
			sb.AppendLine("<b>Press Rayast:</b>");
			sb.AppendLine(pointerPressRaycast.ToString());
			return sb.ToString();
		}
	}
}
