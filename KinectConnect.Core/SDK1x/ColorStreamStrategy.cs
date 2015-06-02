﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public class ColorStreamStrategy : IEventedExtractorStrategy<Bitmap>
    {
        public Capabilities RequiredCapabilities()
        {
            return Capabilities.ColorStream;
        }

        public void Initialise(Microsoft.Kinect.KinectSensor sensor)
        {
            // Not needed
        }

        public void Extract(Microsoft.Kinect.AllFramesReadyEventArgs args)
        {
            using (var cif = args.OpenColorImageFrame())
            {
                if (DataExtracted != null)
                    DataExtracted(ImageToBitmap(cif));
            }
        }

        private Bitmap ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(Image.Width, Image.Height, PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, Image.Width, Image.Height),
                ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr, Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }


        public event Action<Bitmap> DataExtracted;
    }

    static class ImageExtensions
    {
        public static byte[][] to2DJagged(this byte[] buffer, int width, int height, int bytesPerPixel)
        {
            byte[][] bytes2d = new byte[height][];

            int rowCounter = 0;
            for (int i = 0; i < buffer.Length; i += width * bytesPerPixel)
            {
                byte[] row = new byte[width * bytesPerPixel];
                Buffer.BlockCopy(buffer, i, row, 0, width * bytesPerPixel);
                bytes2d[rowCounter++] = row;
            }

            return bytes2d;
        }
    }
}
