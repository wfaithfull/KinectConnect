using KinectConnect.Core.SDK1x;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public bool BufferFrames { get; set; }

        public Kinect(bool buffer = false)
        {
            BufferFrames = buffer;
        }

        public void Start()
        {
            Extractor extractor = new Extractor();
            FaceDataStrategy faceStrategy = new FaceDataStrategy();
            faceStrategy.FaceFrameReady += (s, e) =>
            {
                if (BufferFrames)
                    faceBuffer.Enqueue(e);
                else
                    singleBuffer = e;
            };
            extractor.RegisterStrategy(faceStrategy);
            new Thread((e) =>
            {
                extractor.Start(manager.Kinect);
            }).Start();
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

        public void Stop()
        {
            manager.Kinect.Stop();
        }
    }
}
