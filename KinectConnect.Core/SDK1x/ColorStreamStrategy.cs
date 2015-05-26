using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public class ColorStreamStrategy : IExtractorStrategy
    {
        public Capabilities RequiredCapabilities()
        {
            return Capabilities.ColorStream;
        }

        public void Initialise(Microsoft.Kinect.KinectSensor sensor)
        {
            // Not needed
        }

        public void Extract(Microsoft.Kinect.AllFramesReadyEventArgs args)
        {
            using (var cif = args.OpenColorImageFrame())
            {

            }
        }
    }
}
