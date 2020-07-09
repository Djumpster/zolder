// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Talespin.Core.Foundation.Parsing;
using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	public static class CurveJSONImporter
	{
		private struct ControlPoint
		{
			public Vector3 position,
							inHandle,
							outHandle;

			public ControlPoint(Vector3 position, Vector3 inHandle, Vector3 outHandle)
			{
				this.position = position;
				this.inHandle = inHandle;
				this.outHandle = outHandle;
			}
		}

		public static IDictionary<string, CompoundCurve> ParseCurveList(string jsonData, float scale = 1f)
		{
			Hashtable curveListData = JSON.JsonDecode(jsonData) as Hashtable;
			if (curveListData == null)
			{
				throw new ArgumentException("Invalid json");
			}

			return ParseCurveList(curveListData, scale);
		}

		public static IDictionary<string, CompoundCurve> ParseCurveList(Hashtable curveListData, float scale = 1f)
		{
			Dictionary<string, CompoundCurve> curves = new Dictionary<string, CompoundCurve>();
			foreach (DictionaryEntry curve in curveListData)
			{
				string name = curve.Key as string;
				ArrayList curveData = curve.Value as ArrayList;
				if (name == null)
				{
					throw new ArgumentException("Invalid json");
				}

				curves.Add(name, ParseCurve(curveData, name, scale));
			}
			return curves;
		}

		public static CompoundCurve ParseCurve(ArrayList curveData, string name = "", float scale = 1f, bool circle = false)
		{
			if (curveData == null)
			{
				throw new ArgumentException("Invalid json");
			}

			CompoundCurve curve = CompoundCurve.Create();
			curve.name = name;
			ControlPoint? firstPoint = null,
							lastPoint = null;
			foreach (object controlPoint in curveData)
			{
				Hashtable controlPointData = controlPoint as Hashtable;
				if (controlPointData == null)
				{
					throw new ArgumentException("Invalid json");
				}

				Vector3 pos = ParseVector3(controlPointData["position"] as Hashtable) * scale,
								inHandle = ParseVector3(controlPointData["inHandle"] as Hashtable) * scale,
								outHandle = ParseVector3(controlPointData["outHandle"] as Hashtable) * scale;
				ControlPoint point = new ControlPoint(pos, inHandle, outHandle);
				if (firstPoint == null)
				{
					firstPoint = point;
				}

				if (lastPoint != null)
				{
					curve.Add(CreateCurveBetween(lastPoint.Value, point));
				}

				lastPoint = point;
			}
			if (circle && curveData.Count > 1)
			{
				curve.Add(CreateCurveBetween(lastPoint.Value, firstPoint.Value));
			}

			curve.ReplaceStraightBeziersByLines();
			return curve;
		}

		private static CurveSegment CreateCurveBetween(ControlPoint a, ControlPoint b)
		{
			return Bezier.Create(a.position, a.outHandle, b.inHandle, b.position);
		}

		private static Vector3 ParseVector3(Hashtable data)
		{
			if (data == null || !data.ContainsKey("x") || !data.ContainsKey("y") || !data.ContainsKey("z"))
			{
				throw new ArgumentException("Invalid json");
			}

			float x = (float)data["x"],
					y = (float)data["y"],
					z = (float)data["z"];
			return new Vector3(-x, z, -y); //Conver to Unity coordinate space
		}
	}
}