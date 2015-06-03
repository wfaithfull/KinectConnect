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
    [Flags()]
    public enum Capabilities
    {
        ColorStream = 1,
        DepthStream = 2,
        SkeletonStream = 4,
        NearMode = 8,
        Seated = 16,
        All = ~0
    }

    public class Extractor
    {

        private const ColorImageFormat DEFAULT_COLOR_FORMAT = ColorImageFormat.RgbResolution640x480Fps30;
        private const DepthImageFormat DEFAULT_DEPTH_FORMAT = DepthImageFormat.Resolution320x240Fps30;
        private static TransformSmoothParameters DEFAULT_SKELETON_TRANSFORM_PARAMS = new TransformSmoothParameters()
        {
            Correction = 0.5f,
            JitterRadius = 0.05f,
            MaxDeviationRadius = 0.05f,
            Prediction = 0.5f,
            Smoothing = 0.5f
        };

        private FaceTracker tracker;

        private ColorImageFormat colorFormat;
        private DepthImageFormat depthFormat;

        private byte[] colorData;
        private short[] depthData;

        private List<IExtractorStrategy> strategies = new List<IExtractorStrategy>();

        private Capabilities capabilities = Capabilities.ColorStream;

        public Capabilities Capabilities
        {
            get { return capabilities; }
            private set { capabilities = value; }
        }

        public void RegisterStrategy(IExtractorStrategy strategy)
        {
            strategies.Add(strategy);
            AggregateCapabilities();
        }

        public void Initialise(KinectSensor sensor)
        {
            PrepareKinect(capabilities, sensor);

            tracker = new FaceTracker(sensor);
            colorFormat = sensor.ColorStream.Format;
            depthFormat = sensor.DepthStream.Format;

            colorData = new byte[sensor.ColorStream.FramePixelDataLength];
            depthData = new short[sensor.DepthStream.FramePixelDataLength];

            sensor.AllFramesReady += AllFramesReadyHandler;
        }

        private void AggregateCapabilities()
        {
            foreach(IExtractorStrategy strategy in strategies)
                capabilities = capabilities | strategy.RequiredCapabilities();
        }

        private void PrepareKinect(Capabilities capabilities, KinectSensor sensor)
        {
            if (capabilities.HasFlag(Capabilities.ColorStream) && !sensor.ColorStream.IsEnabled)
                sensor.ColorStream.Enable(DEFAULT_COLOR_FORMAT);

            if (capabilities.HasFlag(Capabilities.DepthStream) && !sensor.DepthStream.IsEnabled)
                sensor.DepthStream.Enable(DEFAULT_DEPTH_FORMAT);
            
            if(capabilities.HasFlag(Capabilities.NearMode)) {
                sensor.DepthStream.Range = DepthRange.Near;
                sensor.SkeletonStream.EnableTrackingInNearRange = true;
            }

            if(capabilities.HasFlag(Capabilities.Seated))
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

            if (capabilities.HasFlag(Capabilities.SkeletonStream) && !sensor.SkeletonStream.IsEnabled)
                sensor.SkeletonStream.Enable(DEFAULT_SKELETON_TRANSFORM_PARAMS);

            if (!sensor.IsRunning)
                sensor.Start();

            foreach (var strategy in strategies)
                strategy.Initialise(sensor);
        }

        private void AllFramesReadyHandler(object sender, AllFramesReadyEventArgs args)
        {
            foreach (var strategy in strategies)
            {
                strategy.Extract(args);
            }
        }
    }
}
