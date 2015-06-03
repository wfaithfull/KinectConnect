using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public class FaceDataStrategy : IEventedExtractorStrategy<FaceData>
    {
        private FaceTracker tracker;

        private ColorImageFormat colorFormat;
        private DepthImageFormat depthFormat;

        private byte[] colorData;
        private short[] depthData;

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

                FaceData serializable = frame.ToSerializableFaceData();
                FireDataExtracted(serializable);
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
        
        private void FireDataExtracted(FaceData data)
        {
            if (DataExtracted != null)
                DataExtracted(data);
        }

        public event Action<FaceData> DataExtracted;
    }
}
