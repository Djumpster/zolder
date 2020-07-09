/*
https://github.com/kyubuns/OnionRingUnity
The MIT License (MIT)

Copyright (c) 2015 kyubuns
- http://kyubuns.net/

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;

namespace Talespin.Core.Foundation.PSDtouGUI
{
	/// <summary>
	/// Class used to remove the redundant pixels of a Sprite (2D and UI)
	/// Will also calculate the sprite border for the 9 slice.
	/// WARNING: The calculated sprite border might not always give correct results
	/// </summary>
	public class Sprite9Slicer
	{
		public static SlicedTexture Slice(Texture2D texture)
		{
			Color32[] pixels = texture.GetPixels32();
			Sprite9Slicer slicer = new Sprite9Slicer(texture, pixels);
			return slicer.Slice(pixels);
		}

		private int width;
		private int height;
		private int[] pixels;
		private readonly int safetyMargin = 2;
		private readonly int margin = 2;

		private Sprite9Slicer(Texture2D refTexture, Color32[] refPixels)
		{
			width = refTexture.width;
			height = refTexture.height;

			pixels = new int[refPixels.Length];
			for (int i = 0; i < refPixels.Length; ++i)
			{
				pixels[i] = refPixels[i].a > 0 ? refPixels[i].GetHashCode() : 0;
			}
		}

		private void CalcLine(ulong[] list, out int start, out int end)
		{
			start = 0;
			end = 0;
			int tmpStart = 0;
			int tmpEnd = 0;
			ulong tmpHash = list[0];
			for (int i = 0; i < list.Length; ++i)
			{
				if (tmpHash == list[i])
				{
					tmpEnd = i;
				}
				else
				{
					if (end - start < tmpEnd - tmpStart)
					{
						start = tmpStart;
						end = tmpEnd;
					}
					tmpStart = i;
					tmpEnd = i;
					tmpHash = list[i];
				}
			}
			if (end - start < tmpEnd - tmpStart)
			{
				start = tmpStart;
				end = tmpEnd;
			}

			end -= (safetyMargin * 2 + margin);
			if (end < start)
			{
				start = 0;
				end = 0;
			}
		}

		private ulong[] CreateHashListX(int aMax, int bMax)
		{
			var hashList = new ulong[aMax];
			for (int a = 0; a < aMax; ++a)
			{
				ulong line = 0;
				for (int b = 0; b < bMax; ++b)
				{
					line = (ulong)(line + (ulong)(pixels[b * width + a] * b)).GetHashCode();
				}

				hashList[a] = line;
			}
			return hashList;
		}

		private ulong[] CreateHashListY(int aMax, int bMax)
		{
			ulong[] hashList = new ulong[aMax];
			for (int a = 0; a < aMax; ++a)
			{
				ulong line = 0;
				for (int b = 0; b < bMax; ++b)
				{
					line = (ulong)(line + (ulong)(pixels[a * width + b] * b)).GetHashCode();
				}
				hashList[a] = line;
			}
			return hashList;
		}

		private SlicedTexture Slice(Color32[] originalPixels)
		{
			int xStart, xEnd;
			{
				ulong[] hashList = CreateHashListX(width, height);
				CalcLine(hashList, out xStart, out xEnd);
			}

			int yStart, yEnd;
			{
				ulong[] hashList = CreateHashListY(height, width);
				CalcLine(hashList, out yStart, out yEnd);
			}

			bool skipX = false;
			if (xEnd - xStart < 2)
			{
				skipX = true;
				xStart = 0;
				xEnd = 0;
			}

			bool skipY = false;
			if (yEnd - yStart < 2)
			{
				skipY = true;
				yStart = 0;
				yEnd = 0;
			}
			Texture2D output = GenerateSlicedTexture(xStart, xEnd, yStart, yEnd, originalPixels);
			int left = xStart + safetyMargin;
			int bottom = yStart + safetyMargin;
			int right = width - xEnd - safetyMargin - margin;
			int top = height - yEnd - safetyMargin - margin;
			if (skipX)
			{
				left = 0;
				right = 0;
			}
			if (skipY)
			{
				top = 0;
				bottom = 0;
			}
			return new SlicedTexture(output, new Border(left, bottom, right, top));
		}

		private Texture2D GenerateSlicedTexture(int xStart, int xEnd, int yStart, int yEnd, Color32[] originalPixels)
		{
			int outputWidth = width - (xEnd - xStart);
			int outputHeight = height - (yEnd - yStart);
			Color32[] outputPixels = new Color32[outputWidth * outputHeight];
			for (int x = 0, originalX = 0; x < outputWidth; ++x, ++originalX)
			{
				if (originalX == xStart)
				{
					originalX += (xEnd - xStart);
				}
				for (int y = 0, originalY = 0; y < outputHeight; ++y, ++originalY)
				{
					if (originalY == yStart)
					{
						originalY += (yEnd - yStart);
					}
					outputPixels[y * outputWidth + x] = originalPixels[originalY * width + originalX];
				}
			}
			Texture2D output = new Texture2D(outputWidth, outputHeight);
			output.SetPixels32(outputPixels);
			return output;
		}

		private int Get(int x, int y)
		{
			return pixels[y * width + x];
		}


		public class SlicedTexture
		{
			public SlicedTexture(Texture2D texture, Border border)
			{
				Texture = texture;
				Border = border;
			}

			public Texture2D Texture { get; private set; }
			public Border Border { get; private set; }
		}

		public class Border
		{
			public Border(int left, int bottom, int right, int top)
			{
				Left = left;
				Bottom = bottom;
				Right = right;
				Top = top;
			}

			public Vector4 ToVector4() { return new Vector4(Left, Bottom, Right, Top); }

			public int Left { get; private set; }
			public int Bottom { get; private set; }
			public int Right { get; private set; }
			public int Top { get; private set; }
		}
	}
}
