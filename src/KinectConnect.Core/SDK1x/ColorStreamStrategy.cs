using Microsoft.Kinect;
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
    public abstract class ColorStreamStrategy<T> : IEventedExtractorStrategy<T>
    {
        public Capabilities RequiredCapabilities()
        {
            return Capabilities.ColorStream;
        }

        public void Initialise(Microsoft.Kinect.KinectSensor sensor)
        {
            // Not needed; Enabled by default.
        }

        public void Extract(AllFramesReadyEventArgs args)
        {
            T data = DoExtract(args);
            if (DataExtracted != null)
                DataExtracted(data);
        }

        protected abstract T DoExtract(AllFramesReadyEventArgs args);

        public event Action<T> DataExtracted;
    }

    public class BitmapStreamStrategy : ColorStreamStrategy<Bitmap>
    {
        
        protected override Bitmap DoExtract(AllFramesReadyEventArgs args)
        {
            using (var cif = args.OpenColorImageFrame())
            {
                return ImageToBitmap(cif);
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
    }

    public class ByteArrayStreamStrategy : ColorStreamStrategy<byte[]>
    {
        protected override byte[] DoExtract(AllFramesReadyEventArgs args)
        {
            using (var cif = args.OpenColorImageFrame())
            {
                Console.WriteLine(cif.Format.ToString());
                return cif.GetRawPixelData();
            }
        }
    }
}
