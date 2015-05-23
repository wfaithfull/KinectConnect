using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using Microsoft.Samples.Kinect.WpfViewers;
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
        private const DepthImageFormat DEFAULT_DEPTH_FORMAT = DepthImageFormat.Resolution320x240Fps30;
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
            //PrepareKinectSensor(sensor);

            tracker = new FaceTracker(sensor);

            colorFormat = sensor.ColorStream.Format;
            depthFormat = sensor.DepthStream.Format;

            colorData = new byte[sensor.ColorStream.FramePixelDataLength];
            depthData = new short[sensor.DepthStream.FramePixelDataLength];

            sensor.AllFramesReady += sensor_AllFramesReady;
            Debug.WriteLine("Hello");
        }

        public static void PrepareKinectSensor(KinectSensorManager sensor)
        {
            sensor.Dispatcher.InvokeAsync(() =>
            {
                sensor.ColorStreamEnabled = true;
                sensor.DepthStreamEnabled = true;
                sensor.SkeletonEnableTrackingInNearMode = true;
                sensor.SkeletonTrackingMode = SkeletonTrackingMode.Seated;
                sensor.SkeletonStreamEnabled = true;
            });

            /*
            Debug.WriteLine("Check connected..");
            if (sensor.Status != KinectStatus.Connected)
                throw new ArgumentException("Kinect sensor must be connected and ready.");

            try
            {
                sensor.ColorStream.Enable(DEFAULT_COLOR_FORMAT);
                sensor.DepthStream.Enable(DEFAULT_DEPTH_FORMAT);
                try
                {
                    // This will throw on non Kinect For Windows devices.
                    sensor.DepthStream.Range = DepthRange.Near;
                    sensor.SkeletonStream.EnableTrackingInNearRange = true;
                }
                catch (InvalidOperationException)
                {
                    sensor.DepthStream.Range = DepthRange.Default;
                    sensor.SkeletonStream.EnableTrackingInNearRange = false;
                }

                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                Debug.WriteLine("Enabling skeleton stream");
                sensor.SkeletonStream.Enable(DEFAULT_SKELETON_TRANSFORM_PARAMS);

                Debug.WriteLine("Done");
            }
            catch (InvalidOperationException)
            {
                // This exception can be thrown when we are trying to
                // enable streams on a device that has gone away.  This
                // can occur in app shutdown scenarios when the sensor
                // goes away between the time it changed status and the
                // time we get the sensor changed notification.
                //
                // Behavior here is to just eat the exception and assume
                // another notification will come along if a sensor
                // comes back.
            }

            Debug.WriteLine("Check Running");
            if (!sensor.IsRunning)
                sensor.Start();
             */
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
