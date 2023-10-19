using StbImageSharp;
using StbImageWriteSharp;
using StbRectPackSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyraTexturePacker
{
	static class Program
	{
		private const int BorderSize = 1;

		private const string IdName = "Id";
		private const string TextureAtlasName = "TextureAtlas";
		private const string ImageName = "Image";
		private const string TextureRegionName = "TextureRegion";
		private const string NinePatchRegionName = "NinePatchRegion";
		private const string LeftName = "Left";
		private const string TopName = "Top";
		private const string WidthName = "Width";
		private const string HeightName = "Height";
		private const string NinePatchLeftName = "NinePatchLeft";
		private const string NinePatchTopName = "NinePatchTop";
		private const string NinePatchRightName = "NinePatchRight";
		private const string NinePatchBottomName = "NinePatchBottom";

		class ImageInfo
		{
			public string Path;
			public ImageResult Image;
			public bool IsNinePatch;
			public int NinePatchLeft, NinePatchRight, NinePatchTop, NinePatchBottom;
		}

		private enum OutputType
		{
			Jpg,
			Png,
			Tga,
			Bmp,
			Hdr
		}

		private static OutputType DetermineOutputType(string outputFile)
		{
			var ext = Path.GetExtension(outputFile);
			if (string.IsNullOrEmpty(ext))
			{
				throw new Exception("Output file lacks extension. Hence it is not possible to determine output file type");
			}

			if (ext.StartsWith("."))
			{
				ext = ext.Substring(1);
			}

			ext = ext.ToLower();

			OutputType outputType;
			switch (ext)
			{
				case "jpg":
				case "jpeg":
					outputType = OutputType.Jpg;
					break;
				case "png":
					outputType = OutputType.Png;
					break;
				case "tga":
					outputType = OutputType.Tga;
					break;
				case "bmp":
					outputType = OutputType.Bmp;
					break;
				case "hdr":
					outputType = OutputType.Hdr;
					break;
				default:
					throw new Exception("Output format '" + ext + "' is not supported.");
			}

			return outputType;
		}

		private static string[] GetImageFiles(string inputFolder)
		{
			var allFiles = Directory.EnumerateFiles(inputFolder, "*.*", SearchOption.TopDirectoryOnly).ToArray();

			var imageFiles = new List<string>();

			foreach (var file in allFiles)
			{
				var ext = Path.GetExtension(file);
				if (string.IsNullOrEmpty(ext))
				{
					continue;
				}

				if (ext.StartsWith("."))
				{
					ext = ext.Substring(1);
				}

				ext = ext.ToLower();

				if (ext != "bmp" && ext != "jpg" && ext != "png" &&
					ext != "jpg" && ext != "psd" && ext != "pic" &&
					ext != "tga" && ext != "hdr")
				{
					// Not an image
					continue;
				}

				imageFiles.Add(file);
			}

			return imageFiles.ToArray();
		}

		private static void ProcessNinePatch(ImageInfo imageInfo)
		{
			var image = imageInfo.Image;
			Console.WriteLine("Nine Patch");

			// Nine Patch
			if (image.Width < 2 || image.Height < 2)
			{
				throw new Exception(string.Format("Nine Patch image lacks 1px border with black stretch lines"));
			}

			imageInfo.IsNinePatch = true;

			// Determine Left and Right
			var foundBlack = false;
			int blackSize = 0;
			for (var i = 0; i < image.Width; ++i)
			{
				var pos = i * 4;
				if (image.Data[pos] == 0 && image.Data[pos + 3] > 128)
				{
					if (!foundBlack)
					{
						imageInfo.NinePatchLeft = i - 1;
						foundBlack = true;
					}
					++blackSize;
				}
				else if (foundBlack)
				{
					break;
				}
			}

			imageInfo.NinePatchRight = image.Width - 2 - imageInfo.NinePatchLeft - blackSize;

			// Determine Top and Bottom
			foundBlack = false;
			blackSize = 0;
			for (var i = 0; i < image.Height; ++i)
			{
				var pos = i * image.Width * 4;
				if (image.Data[pos] == 0 && image.Data[pos + 3] > 128)
				{
					if (!foundBlack)
					{
						imageInfo.NinePatchTop = i - 1;
						foundBlack = true;
					}
					++blackSize;
				}
				else if (foundBlack)
				{
					break;
				}
			}

			imageInfo.NinePatchBottom = image.Height - 2 - imageInfo.NinePatchTop - blackSize;

			Console.WriteLine("Left: {0}, Right: {1}, Top: {2}, Bottom: {3}", imageInfo.NinePatchLeft, imageInfo.NinePatchRight, imageInfo.NinePatchTop, imageInfo.NinePatchBottom);

			if (imageInfo.NinePatchLeft == 0 && imageInfo.NinePatchRight == 0 &&
				imageInfo.NinePatchTop == 0 && imageInfo.NinePatchBottom == 0)
			{
				// Not a nine patch
				imageInfo.IsNinePatch = false;
			}

			// Erase info border
			var newWidth = image.Width - 2;
			var newHeight = image.Height - 2;
			var newData = new byte[newWidth * newHeight * 4];
			for (var y = 0; y < newHeight; ++y)
			{
				var sourcePos = ((y + 1) * image.Width + 1) * 4;
				var destPos = y * newWidth * 4;
				Array.Copy(image.Data, sourcePos, newData, destPos, newWidth * 4);
			}

			image.Width = newWidth;
			image.Height = newHeight;
			image.Data = newData;
		}

		private static Packer PackImages(string[] imageFiles, int width, int height)
		{
			Console.WriteLine("Atlas Size: {0}x{1}", width, height);

			var packer = new Packer(width, height);
			foreach (var file in imageFiles)
			{
				Console.WriteLine("Processing {0}...", file);

				ImageResult image;
				using (var stream = File.OpenRead(file))
				{
					image = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
				}

				var imageInfo = new ImageInfo
				{
					Path = file,
					Image = image
				};

				var name = Path.GetFileNameWithoutExtension(file);

				if (name.EndsWith(".9"))
				{
					ProcessNinePatch(imageInfo);
				}

				Console.WriteLine("Size: {0}x{1}, Components: {2}", image.Width, image.Height, image.SourceComp);

				var fullWidth = image.Width + BorderSize * 2;
				var fullHeight = image.Height + BorderSize * 2;
				var packRectangle = packer.PackRect(fullWidth, fullHeight, imageInfo);

				// Double the size of the packer until the new rectangle will fit
				while (packRectangle == null)
				{
					Packer newPacker = new Packer(packer.Width * 2, packer.Height * 2);

					Console.WriteLine("Image didnt fit. Thus atlas size had been doubled: {0}x{1}", newPacker.Width, newPacker.Height);

					// Place existing rectangles
					foreach (PackerRectangle existingRect in packer.PackRectangles)
					{
						newPacker.PackRect(existingRect.Width, existingRect.Height, existingRect.Data);
					}

					// Now dispose old packer and assign new one
					packer.Dispose();
					packer = newPacker;

					// Try to fit the rectangle again
					packRectangle = packer.PackRect(fullWidth, fullHeight, imageInfo);
				}
			}

			return packer;
		}

		private static byte[] BuildAtlasBitmap(Packer packer)
		{
			var bitmap = new byte[(long)packer.Width * packer.Height * 4];
			foreach (var packRectangle in packer.PackRectangles)
			{
				var imageInfo = (ImageInfo)packRectangle.Data;

				var image = imageInfo.Image;

				// Draw image on its position
				for (var y = 0; y < image.Height; ++y)
				{
					var sourcePos = (y * image.Width) * 4;
					var destPos = (((BorderSize + y + packRectangle.Y) * (long)packer.Width) + BorderSize + packRectangle.X) * 4;

					Array.Copy(image.Data, sourcePos, bitmap, destPos, image.Width * 4);
				}
			}

			return bitmap;
		}

		private static void WriteOutputImage(string outputFile, OutputType outputType, int width, int height, byte[] bitmap)
		{
			Console.WriteLine("Writing {0}", outputFile);
			using (var stream = File.Create(outputFile))
			{
				var imageWriter = new ImageWriter();
				switch (outputType)
				{
					case OutputType.Jpg:
						imageWriter.WriteJpg(bitmap, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, 90);
						break;
					case OutputType.Png:
						imageWriter.WritePng(bitmap, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
						break;
					case OutputType.Tga:
						imageWriter.WriteTga(bitmap, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
						break;
					case OutputType.Bmp:
						imageWriter.WriteBmp(bitmap, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
						break;
					case OutputType.Hdr:
						imageWriter.WriteHdr(bitmap, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
						break;
				}
			}
		}

		private static XDocument CreateOutputXML(string outputFile, Packer packer)
		{
			var doc = new XDocument();
			var root = new XElement(TextureAtlasName);
			root.SetAttributeValue(ImageName, Path.GetFileName(outputFile));
			doc.Add(root);

			foreach (var packRectangle in packer.PackRectangles)
			{
				var imageInfo = (ImageInfo)packRectangle.Data;

				var entry = new XElement(imageInfo.IsNinePatch ? NinePatchRegionName : TextureRegionName);

				var id = Path.GetFileNameWithoutExtension(imageInfo.Path);
				if (id.EndsWith(".9"))
				{
					id = id.Substring(0, id.Length - 2);
				}
				entry.SetAttributeValue(IdName, id);
				entry.SetAttributeValue(LeftName, packRectangle.X + BorderSize);
				entry.SetAttributeValue(TopName, packRectangle.Y + BorderSize);
				entry.SetAttributeValue(WidthName, packRectangle.Width - BorderSize * 2);
				entry.SetAttributeValue(HeightName, packRectangle.Height - BorderSize * 2);

				if (imageInfo.IsNinePatch)
				{
					entry.SetAttributeValue(NinePatchLeftName, imageInfo.NinePatchLeft);
					entry.SetAttributeValue(NinePatchTopName, imageInfo.NinePatchTop);
					entry.SetAttributeValue(NinePatchRightName, imageInfo.NinePatchRight);
					entry.SetAttributeValue(NinePatchBottomName, imageInfo.NinePatchBottom);
				}

				root.Add(entry);
			}

			return doc;
		}

		private static void Process(string inputFolder, string outputFile, int width, int height)
		{
			var outputType = DetermineOutputType(outputFile);
			var imageFiles = GetImageFiles(inputFolder);

			if (imageFiles.Length == 0)
			{
				throw new Exception("No image files found at " + inputFolder);
			}

			Console.WriteLine("{0} image files found at {1}.", imageFiles.Length, inputFolder);

			var packer = PackImages(imageFiles, width, height);

			// All images had been packed
			// Now build up the atlas bitmap
			var bitmap = BuildAtlasBitmap(packer);

			// Write output image
			WriteOutputImage(outputFile, outputType, packer.Width, packer.Height, bitmap);

			// Generate XML
			var xml = CreateOutputXML(outputFile, packer);

			// Write it
			var outputFileXml = Path.ChangeExtension(outputFile, "xml");
			Console.WriteLine("Writing {0}", outputFileXml);
			xml.Save(outputFileXml);

			Console.WriteLine("Success.");
		}

		public static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: MyraTexturePacker.exe <input_folder> <output_file> [width] [height]");
				return;
			}

			try
			{
				var width = 256;
				if (args.Length > 2)
				{
					width = int.Parse(args[2]);
				}

				var height = 256;
				if (args.Length > 3)
				{
					height = int.Parse(args[3]);
				}

				Process(args[0], args[1], width, height);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}