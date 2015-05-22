using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KinectConnect.SDK1x.Extraction
{
    public class ImageStreamExtractor
    {
        public ImageStreamExtractor(KinectSensor sensor)
        {
            sensor.ColorFrameReady += sensor_ColorFrameReady;
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame cif = e.OpenColorImageFrame())
            {
                if (cif == null)
                    return;
                BitmapSource src = ImageToBitmap(cif);
                FireBitmapReady(src);
            }
        }

        public delegate void BitmapReadyHandler(object sender, BitmapSource bitmap);
        public event BitmapReadyHandler BitmapReady;
        private void FireBitmapReady(BitmapSource bitmap)
        {
            if (BitmapReady != null)
            {
                BitmapReady(this, bitmap);
            }
        }

        static BitmapSource ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            BitmapSource bmap = BitmapSource.Create(
             Image.Width,
             Image.Height,
             96, 96,
             PixelFormats.Bgr32,
             null,
             pixeldata,
             Image.Width * Image.BytesPerPixel);
            return bmap;
        }
    }
}
