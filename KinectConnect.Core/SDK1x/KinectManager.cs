using KinectConnect.Core.SDK1x;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    public class KinectManager
    {
        private List<KinectSensor> sensors = new List<KinectSensor>();

        public KinectSensor Kinect { get; set; }

        public KinectManager()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                Console.WriteLine(potentialSensor.UniqueKinectId);
                sensors.Add(potentialSensor);
            }

            Kinect = sensors.First(x => x.Status == KinectStatus.Connected);
        }
    }
}
