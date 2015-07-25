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
    /// <summary>
    /// Kinect handle class to be used in Matlab.
    /// See src/Matlab for Matlab .m file examples.
    /// </summary>
    public class Kinect
    {
        private ConcurrentQueue<FaceData> faceBuffer = new ConcurrentQueue<FaceData>();
        KinectManager manager = new KinectManager();
        private FaceData singleBuffer;
        private byte[] imageArray;

        /// <summary>
        /// Whether to buffer frames in a concurrect queue or
        /// simply overwrite a single frame each time.
        /// </summary>
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
            // Async; Method immediately yields control to Matlab, as this is the
            // most intuitive approach. Users are using Matlab, let Matlab handle
            // the control flow. Also, threading in Matlab is awful.
            Task.Run(() =>
            {
                Extractor extractor = new Extractor();
                FaceDataStrategy faceStrategy = new FaceDataStrategy();
                // On each new face frame...
                faceStrategy.DataExtracted += faceData =>
                {
                    if (BufferFrames)
                        faceBuffer.Enqueue(faceData);
                    else
                        singleBuffer = faceData;
                };

                ByteArrayStreamStrategy colorStrategy = new ByteArrayStreamStrategy();
                // On each new image frame...
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

        /// <summary>
        /// This method name is horrible, but necessary. It is more
        /// important to have a horrible but descriptive method name.
        /// Otherwise the image format is not blindingly obvious.
        /// </summary>
        /// <returns>
        /// A byte array containing a Bgr32 image, 640x480x3
        /// but all in one contiguous block of memory.
        /// </returns>
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
