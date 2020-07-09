// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	public static class CurveExtensions
	{
		public static CurveSearchDataset BinarySearch(this ICurve curve, Func<Vector3, float> getDistance, int iterations)
		{
			Vector3 curveCenter = curve.Evaluate(.5f);
			CurveSearchDataset left = new CurveSearchDataset(0f, curve.Origin, getDistance(curve.Origin)),
									center = new CurveSearchDataset(.5f, curveCenter, getDistance(curveCenter)),
									right = new CurveSearchDataset(1f, curve.Destination, getDistance(curve.Destination));
			return BinarySearch(curve, getDistance, left, center, right, iterations);
		}

		public static CurveSearchDataset BinarySearch(this ICurve curve, Func<Vector3, float> getDistance, CurveSearchDataset leftData, CurveSearchDataset centerData, CurveSearchDataset rightData, int iterations)
		{
			if (iterations == 0) // we have used the max number of iterations, return the best result
			{
				if (leftData.squareDistance < rightData.squareDistance && leftData.squareDistance < centerData.squareDistance)
				{
					return leftData;
				}
				else
					if (rightData.squareDistance < leftData.squareDistance && rightData.squareDistance < centerData.squareDistance)
				{
					return rightData;
				}
				else
				{
					return centerData;
				}
			}
			else // check which of our 3 points (left, center or right) is closest to the point
			{
				if (leftData.squareDistance < rightData.squareDistance && leftData.squareDistance < centerData.squareDistance)
				{
					float newTLeft = leftData.curveTime - (centerData.curveTime - leftData.curveTime) / 2f;
					if (newTLeft < 0)
					{
						rightData.curveTime = centerData.curveTime;
						rightData.curvePosition = centerData.curvePosition;
						rightData.squareDistance = centerData.squareDistance;

						centerData.curveTime = (leftData.curveTime + centerData.curveTime) / 2f;
						centerData.curvePosition = curve.Evaluate(centerData.curveTime);
						centerData.squareDistance = getDistance(centerData.curvePosition);
					}
					else
					{
						leftData.curveTime = newTLeft;
						leftData.curvePosition = curve.Evaluate(leftData.curveTime);
						leftData.squareDistance = getDistance(leftData.curvePosition);

						rightData.curveTime = (leftData.curveTime + centerData.curveTime) / 2f;
						rightData.curvePosition = curve.Evaluate(rightData.curveTime);
						rightData.squareDistance = getDistance(rightData.curvePosition);

						centerData.curveTime = leftData.curveTime;
						centerData.curvePosition = leftData.curvePosition;
						centerData.squareDistance = leftData.squareDistance;
					}
				}
				else if (rightData.squareDistance < leftData.squareDistance && rightData.squareDistance < centerData.squareDistance)
				{
					float newTRight = rightData.curveTime + (rightData.curveTime - centerData.curveTime) / 2;
					if (newTRight > 1)
					{
						leftData.curveTime = centerData.curveTime;
						leftData.curvePosition = centerData.curvePosition;
						leftData.squareDistance = centerData.squareDistance;

						centerData.curveTime = (centerData.curveTime + rightData.curveTime) / 2f;
						centerData.curvePosition = curve.Evaluate(centerData.curveTime);
						centerData.squareDistance = getDistance(centerData.curvePosition);
					}
					else
					{
						leftData.curveTime = (centerData.curveTime + rightData.curveTime) / 2f;
						leftData.curvePosition = curve.Evaluate(leftData.curveTime);
						leftData.squareDistance = getDistance(leftData.curvePosition);

						centerData.curveTime = rightData.curveTime;
						centerData.curvePosition = rightData.curvePosition;
						centerData.squareDistance = rightData.squareDistance;

						rightData.curveTime = newTRight;
						rightData.curvePosition = curve.Evaluate(rightData.curveTime);
						rightData.squareDistance = getDistance(rightData.curvePosition);
					}
				}
				else
				{
					leftData.curveTime = (centerData.curveTime + leftData.curveTime) / 2f;
					leftData.curvePosition = curve.Evaluate(leftData.curveTime);
					leftData.squareDistance = getDistance(leftData.curvePosition);

					rightData.curveTime = (centerData.curveTime + rightData.curveTime) / 2f;
					rightData.curvePosition = curve.Evaluate(rightData.curveTime);
					rightData.squareDistance = getDistance(rightData.curvePosition);
				}
				return BinarySearch(curve, getDistance, leftData, centerData, rightData, iterations - 1);
			}
		}
	}
}
