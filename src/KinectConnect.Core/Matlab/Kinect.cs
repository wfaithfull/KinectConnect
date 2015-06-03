using KinectConnect.Core.SDK1x;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private byte[] imageArray;

        public bool BufferFrames { get; set; }

        public Kinect(bool buffer = false)
        {
            BufferFrames = buffer;
        }

        static double[] BytesToDoubles(byte[] bytes)
        {
            var doubles = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, doubles, 0, bytes.Length);
            return doubles;
        }

        public void Start()
        {
            Task.Run(() =>
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

                ByteArrayStreamStrategy colorStrategy = new ByteArrayStreamStrategy();
                colorStrategy.DataExtracted += bytes =>
                {
                    imageArray = bytes;
                };
                extractor.RegisterStrategy(faceStrategy);
                extractor.RegisterStrategy(colorStrategy);
                extractor.Initialise(manager.Kinect);
            });
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

        public byte[] GetImageBytesBgr32640x480()
        {
            return imageArray;
        }

        public void Stop()
        {
            Task.Run(() =>
            {
                manager.Kinect.Stop();
            });
        }
    }
}
