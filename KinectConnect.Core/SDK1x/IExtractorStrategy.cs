using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public interface IExtractorStrategy
    {
        Capabilities RequiredCapabilities();

        void Initialise(KinectSensor sensor);

        void Extract(AllFramesReadyEventArgs args);
    }

}
