using KinectConnect.SDK1x.Extraction;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.SDK1x.Models
{
    public class SensorModel
    {
        public KinectSensorChooser Chooser { get; set; }
        public KinectSensorManager Manager { get; set; }

        public SensorModel()
        {

        }
    }
}
