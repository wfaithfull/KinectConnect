using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.SDK1x.Extraction
{
    public class FaceDataExtractor
    {
        #region Constants

        private const ColorImageFormat DEFAULT_COLOR_FORMAT = ColorImageFormat.RgbResolution640x480Fps30;
        private const DepthImageFormat DEFAULT_DEPTH_FORMAT = DepthImageFormat.Resolution640x480Fps30;
        private static TransformSmoothParameters DEFAULT_SKELETON_TRANSFORM_PARAMS = new TransformSmoothParameters()
        {
            Correction = 0.5f,
            JitterRadius = 0.05f,
            MaxDeviationRadius = 0.05f,
            Prediction = 0.5f,
            Smoothing = 0.5f
        };

        #endregion

        private FaceTracker tracker;

        private ColorImageFormat colorFormat;
        private DepthImageFormat depthFormat;

        private byte[] colorData;
        private short[] depthData;

        public FaceDataExtractor(KinectSensor sensor)
        {
            PrepareKinectSensor(sensor);

            tracker = new FaceTracker(sensor);

            colorFormat = sensor.ColorStream.Format;
            depthFormat = sensor.DepthStream.Format;

            colorData = new byte[sensor.ColorStream.FramePixelDataLength];
            depthData = new short[sensor.DepthStream.FramePixelDataLength];

            sensor.AllFramesReady += sensor_AllFramesReady;
            Debug.WriteLine("Hello");
        }

        private static void PrepareKinectSensor(KinectSensor sensor)
        {
            if (sensor.Status != KinectStatus.Connected)
                throw new ArgumentException("Kinect sensor must be connected and ready.");

            if (!sensor.ColorStream.IsEnabled)
                sensor.ColorStream.Enable(DEFAULT_COLOR_FORMAT);

            if (!sensor.DepthStream.IsEnabled)
                sensor.DepthStream.Enable(DEFAULT_DEPTH_FORMAT);

            if (!sensor.SkeletonStream.IsEnabled)
                sensor.SkeletonStream.Enable(DEFAULT_SKELETON_TRANSFORM_PARAMS);

            if (!sensor.IsRunning)
                sensor.Start();
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using(ColorImageFrame cif = e.OpenColorImageFrame()) 
            using(DepthImageFrame dif = e.OpenDepthImageFrame())
            using(SkeletonFrame sf = e.OpenSkeletonFrame()) 
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

                if (frame.TrackSuccessful)
                {
                    FireFaceFrameReady(frame);
                }
            }
        }

        public delegate void FaceFrameReadyHandler(object sender, FaceTrackFrame frame);
        public event FaceFrameReadyHandler FaceFrameReady;
        private void FireFaceFrameReady(FaceTrackFrame frame)
        {
            if (FaceFrameReady != null)
            {
                FaceFrameReady(this, frame);
            }
        }
    }
}
