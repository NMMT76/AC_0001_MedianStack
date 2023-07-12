using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AC_0001_MedianStack.AC_Helpers
{
	public static class ArrayHelpers
	{
		public static byte[][][] JaggedArray(int dimension1, int dimension2, int dimension3)
		{
			byte[][][] retval;
			retval = new byte[dimension1][][];
			for (int d1 = 0; d1 < dimension1; d1++)
			{
				retval[d1] = new byte[dimension2][];
				for (int d2 = 0; d2 < dimension2; d2++)
				{
					retval[d1][d2] = new byte[dimension3];
				}
			}
			return retval;
		}
	}
	public static class ImageFileHelpers
	{
		public static byte[][][] LoadImageToArray(string sourceimage)
		{
			byte[][][] retval;
			using (Image<Rgb24> image = Image.Load<Rgb24>(sourceimage))
			{
				int width = image.Width;
				int height = image.Height;
				retval = ArrayHelpers.JaggedArray(width,height,3);
				for (int x = 0; x < width - 1; x++)
				{
					for (int y = 0; y < height - 1; y++)
					{
						Rgb24 pixel = image[x, y];
						retval[x][y][0] = pixel.R;
						retval[x][y][1] = pixel.G;
						retval[x][y][2] = pixel.B;
					}
				}
			}
			return retval;
		}
#pragma warning disable S2368 // Public methods should not have multidimensional array parameters
		public static void SaveArrayToImage(byte[][][] imagebuffer,string destination)
#pragma warning restore S2368 // Public methods should not have multidimensional array parameters
		{
			int width = imagebuffer.GetLength(0);
			int height = imagebuffer[0].GetLength(0);
			using (Image<Rgb24> image = new Image<Rgb24>(width,height))
			{
				for (int x = 0; x < width - 1; x++)
				{
					for (int y = 0; y < height - 1; y++)
					{
						image[x, y] = new Rgb24(imagebuffer[x][y][0], imagebuffer[x][y][1], imagebuffer[x][y][2]);
					}
				}
				image.SaveAsPng(destination);
			}
		}
		public static byte[][][] CalculateMedianImage(List<byte[][][]> imagebuffers,int runlength)
		{
			byte[][][] retval;
			int width = imagebuffers[0].GetLength(0);
			int height = imagebuffers[0][0].GetLength(0);
			retval = ArrayHelpers.JaggedArray(width, height,3);
			List<byte> compr = new List<byte>();
			List<byte> compg = new List<byte>();
			List<byte> compb = new List<byte>();
			for (int x = 0; x < width - 1; x++)
			{
				for (int y = 0; y < height - 1; y++)
				{
					for (int rl = 0; rl < runlength - 1; rl++)
					{
						compr.Add(imagebuffers[rl][x][y][0]);
						compg.Add(imagebuffers[rl][x][y][1]);
						compb.Add(imagebuffers[rl][x][y][2]);
					}
					//Sort the lists and extract the value at the middle
					compr.Sort();
					retval[x][y][0] = compr[(int)(compr.Count / 2.0)];
					compg.Sort();
					retval[x][y][1] = compg[(int)(compg.Count / 2.0)];
					compb.Sort();
					retval[x][y][2] = compb[(int)(compb.Count / 2.0)];
					//
					compr.Clear(); compg.Clear(); compb.Clear();
				}
			}
			return retval;
		}
#pragma warning disable S2368 // Public methods should not have multidimensional array parameters
		public static Tuple<double,int,int> CalculateDiff(byte[][][] buffer1, byte[][][] buffer2)
#pragma warning restore S2368 // Public methods should not have multidimensional array parameters
		{
			int width = buffer1.GetLength(0);
			int height = buffer1[0].GetLength(0);
			double rmserror=0;
			int mindiff = 3*255, maxdiff = 0;
			for (int x = 0; x < width - 1; x++)
			{
				for (int y = 0; y < height - 1; y++)
				{
					for (int c = 0; c < 3; c++)
					{
						int absdiff = Math.Abs((int)buffer1[x][y][c] - (int)buffer2[x][y][c]);
						rmserror += Math.Pow(absdiff, 2);
						if ((absdiff > 0) && (absdiff < mindiff)) { mindiff = absdiff; }
						if (absdiff > maxdiff) { maxdiff = absdiff; }
					}
				}
				rmserror = Math.Sqrt(rmserror / (height * width * 3));
			}
			return new Tuple<double,int,int>(rmserror, mindiff, maxdiff);
		}
	}
}
