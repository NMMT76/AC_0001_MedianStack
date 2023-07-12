using System;
using System.Collections.Generic;
using System.Diagnostics;
using AC_0001_MedianStack.AC_Helpers;
namespace AC_0001_MedianStack
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			Stopwatch stpw = new Stopwatch();
			stpw.Start();
			//Source directory for the images
			string baseimagesourcedir = System.Environment.CurrentDirectory+@"\MedianStackImages\";
			string outdir = System.Environment.CurrentDirectory;

			List < byte[][][]> imagebuffers = new List<byte[][][]>();
			
			//Load images into the list of imagebuffers
			for (int counter = 0; counter < 10; counter++)
			{
				string imagefile = (counter+1).ToString().PadLeft(4,'0');
				byte[][][] imagebuffer = ImageFileHelpers.LoadImageToArray($"{baseimagesourcedir}\\{imagefile}.png");
				imagebuffers.Add(imagebuffer);
			}

			ImageFileHelpers.SaveArrayToImage(imagebuffers[0], outdir + "\\median00.png");


			//Declare the output buffers
			byte[][][] buffermed5,buffermed10;

			//Calculate the median buffers
			buffermed5 = ImageFileHelpers.CalculateMedianImage(imagebuffers, 5);
			buffermed10 = ImageFileHelpers.CalculateMedianImage(imagebuffers, 10);

			OutputDiff(ImageFileHelpers.CalculateDiff(imagebuffers[0], buffermed10), nameof(imagebuffers), nameof(buffermed10));
			OutputDiff(ImageFileHelpers.CalculateDiff(buffermed5, buffermed10), nameof(buffermed5), nameof(buffermed10));

			//Output the median images
			ImageFileHelpers.SaveArrayToImage(buffermed5, outdir + "\\median05.png");
			ImageFileHelpers.SaveArrayToImage(buffermed10, outdir + "\\median10.png");

			//Print time elapsed
			long milis = stpw.ElapsedMilliseconds;
			stpw.Stop();
			Console.WriteLine($"Process took {milis}ms");
		}
		public static void OutputDiff(Tuple<double,int,int> diffs,string name1,string name2)
		{
			Console.WriteLine($"Diff between {name1} and {name2}");
			Console.WriteLine($"RMS : {diffs.Item1} - Min : {diffs.Item2} - Max : {diffs.Item3}");
		}
	}
}
