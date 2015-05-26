using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public class FaceDataStrategy : IExtractorStrategy
    {
        private FaceTracker tracker;

        private ColorImageFormat colorFormat;
        private DepthImageFormat depthFormat;

        private byte[] colorData;
        private short[] depthData;

        public delegate void FaceFrameReadyHandler(object sender, FaceData frame);
        public event FaceFrameReadyHandler FaceFrameReady;
        private void FireFaceFrameReady(FaceData frame)
        {
            if (FaceFrameReady != null)
            {
                FaceFrameReady(this, frame);
            }
        }

        public Capabilities RequiredCapabilities()
        {
            return Capabilities.All;
        }

        public void Extract(AllFramesReadyEventArgs args)
        {
            using (ColorImageFrame cif = args.OpenColorImageFrame())
            using (DepthImageFrame dif = args.OpenDepthImageFrame())
            using (SkeletonFrame sf = args.OpenSkeletonFrame())
            {

                cif.CopyPixelDataTo(colorData);
                dif.CopyPixelDataTo(depthData);

                Skeleton[] skeletonBuffer = new Skeleton[sf.SkeletonArrayLength];
                sf.CopySkeletonDataTo(skeletonBuffer);
                var skeleton = skeletonBuffer.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

                FaceTrackFrame frame = tracker.Track(
                    colorFormat,
                    colorData,
                    depthFormat,
                    depthData,
                    skeleton
                );

                //if (frame.TrackSuccessful)
                {
                    FaceData serializable = frame.ToSerializableFaceData();
                    FireFaceFrameReady(serializable);
                }
            }
        }

        public void Initialise(KinectSensor sensor)
        {
            tracker = new FaceTracker(sensor);

            colorFormat = sensor.ColorStream.Format;
            depthFormat = sensor.DepthStream.Format;

            colorData = new byte[sensor.ColorStream.FramePixelDataLength];
            depthData = new short[sensor.DepthStream.FramePixelDataLength];
        }
    }
}
