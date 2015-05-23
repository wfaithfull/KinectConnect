using KinectConnect.Core.SDK1x;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.CLI
{
    class Program
    {
        private static IEnumerable<string> Splash()
        {
            int width = 20;
            yield return PadTo("/", '-', width - 1) + "\\";
            yield return PadTo("| KinectConnect", ' ', width - 1) + "|";
            yield return PadTo("\\", '-', width - 1) + "/";
        }

        static string PadTo(string str, char pad, int count)
        {
            if (str.Length > count)
            {
                char[] chars = str.ToCharArray();
                str = new string(chars.Take(count).ToArray());
            }

            if (str.Length == count)
                return str;

            return str + new string(pad, count - str.Length);
        }

        static void Main(string[] args)
        {
            foreach(string line in Splash())
                Console.WriteLine(line);
            KinectManager manager = new KinectManager();

            Console.WriteLine("Making strategy");
            FaceDataStrategy faceExtractor = new FaceDataStrategy();
            faceExtractor.FaceFrameReady += (s, e) =>
            {
                Console.WriteLine(e.FacePoints[0]);
            };
            Console.WriteLine("Making extractor");
            Extractor extractor = new Extractor();
            Console.WriteLine("Registering strategy");
            extractor.RegisterStrategy(faceExtractor);

            var availableCaps = Enum.GetValues(typeof(Capabilities)).Cast<Enum>();
            foreach (Capabilities cap in availableCaps.Where(extractor.Capabilities.HasFlag))
            {
                Console.WriteLine("Capabilities." + cap.ToString());
            }
            Console.WriteLine("Starting extractor");
            extractor.Start(manager.Kinect);
            
            Console.ReadKey();
        }
    }
}
