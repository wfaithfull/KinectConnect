using KinectConnect.Core.SDK1x;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.CLI
{
    /// <summary>
    /// Visual Studio has something against x64 tests, specifically,
    /// they don't appear in the test explorer. Since most of the tests
    /// we want to do are integration tests, this is an easier solution.
    /// </summary>
    class Tester
    {
        static void Main(string[] args)
        {
            KinectManager manager = new KinectManager();
            Extractor extractor = new Extractor();

            ByteArrayStreamStrategy colorStrategy = new ByteArrayStreamStrategy();
            colorStrategy.DataExtracted += bytes =>
            {
                Console.WriteLine(bytes.Length + " bytes in " + manager.Kinect.ColorStream.Format.ToString() + "@" + manager.Kinect.ColorStream.FrameBytesPerPixel + " bpp");
                Console.WriteLine(BitConverter.ToUInt32(bytes.Take(4).ToArray(), 0));
                
            };
            extractor.RegisterStrategy(colorStrategy);
            extractor.Initialise(manager.Kinect);
            Console.ReadKey();
        }
    }
}
