// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Maths
{
	public static class Sorting
	{
		public delegate int Compare<T>(T A, T B);

		#region BubbleSort
		public static void BubbleSort<T>(this T[] data, Compare<T> compare) //Stable (Keeps order of 'equal' elements), but slow
		{
			bool sorted = false;
			while (!sorted)
			{
				sorted = true;
				for (int i = 0; i < (data.Length - 1); i++)
				{
					int comp = compare(data[i], data[i + 1]);
					if (comp < 0)
					{
						Swap(data, i, i + 1);
						sorted = false;
					}
				}
			}
		}
		#endregion

		#region QuickSort
		public static void QuickSort<T>(this T[] data, Compare<T> compare) // Unstable (Shuffles order of 'equal' elements), but fast
		{
			QuickSort(data, compare, 0, data.Length - 1);
		}

		private static void QuickSort<T>(T[] data, Compare<T> compare, int left, int right)
		{
			if (left < right) //The sub-array is bigger than one element.
			{
				int pivot = (left + right) / 2; //The pivot element will be used as the comparison element
				int pivotPos = Partition(data, compare, left, right, pivot);
				QuickSort(data, compare, left, pivotPos - 1);
				QuickSort(data, compare, pivotPos + 1, right);
			}
		}

		private static int Partition<T>(T[] data, Compare<T> compare, int left, int right, int pivot) // Returns the final position of the pivot.
		{
			T pivotVal = data[pivot];
			Swap(data, pivot, right); //Move the 'value' of the pivot out of the range of the for-loop.

			int finalPivotPos = left; //The 'final' position of the pivot starts out as the left val and moves to the right as it finds elements that should be to the left of the pivot. 
			for (int i = left; i < right; i++)
			{
				if (compare(data[i], pivotVal) < 0)
				{
					Swap(data, i, finalPivotPos++);
				}
			}

			Swap(data, right, finalPivotPos); //Move the 'value' of the pivot to its final position.
			return finalPivotPos;
		}
		#endregion

		private static void Swap<T>(T[] data, int i, int j)
		{
			T temp = data[i];
			data[i] = data[j];
			data[j] = temp;
		}
	}
}
