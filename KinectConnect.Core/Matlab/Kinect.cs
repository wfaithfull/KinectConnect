using KinectConnect.Core.SDK1x;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KinectConnect.Core.Matlab
{
    public class Kinect
    {
        private ConcurrentQueue<FaceData> faceBuffer = new ConcurrentQueue<FaceData>();
        KinectManager manager = new KinectManager();
        private FaceData singleBuffer;
        private MWNumericArray colorImage;

        public bool BufferFrames { get; set; }

        public Kinect(bool buffer = false)
        {
            BufferFrames = buffer;
        }

        public void Start()
        {
            Extractor extractor = new Extractor();
            FaceDataStrategy faceStrategy = new FaceDataStrategy();
            faceStrategy.DataExtracted += faceData =>
            {
                if (BufferFrames)
                    faceBuffer.Enqueue(faceData);
                else
                    singleBuffer = faceData;
            };
            ColorStreamStrategy colorStrategy = new ColorStreamStrategy();
            colorStrategy.DataExtracted += bitmap =>
            {
                int width = bitmap.Width;
                int height = bitmap.Height;

                double[,] underlying = new double[width, height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Color pixelColor = bitmap.GetPixel(i, j);
                        double b = pixelColor.GetBrightness();
                        underlying.SetValue(b, j, i); // Matlab arrays are inverted under the hood
                        colorImage = (MWNumericArray)underlying;
                    }
                }
            };
            extractor.RegisterStrategy(faceStrategy);
            extractor.Initialise(manager.Kinect);
        }

        public FaceData GetFaceFrame()
        {
            if (BufferFrames)
            {
                FaceData result;
                faceBuffer.TryDequeue(out result);
                return result;
            }
            else
            {
                return singleBuffer;
            }
        }

        public MWNumericArray GetColorImage()
        {
            return colorImage;
        }

        public void Stop()
        {
            manager.Kinect.Stop();
        }
    }
}
