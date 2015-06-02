using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KinectConnect.Core.Matlab;
using System.Threading;
using MathWorks.MATLAB.NET.Arrays;

namespace KinectConnect.Core.Test
{
    [TestClass]
    public class MatlabTests
    {
        Kinect kinect;
        [TestInitialize]
        public void Setup()
        {
            kinect = new Kinect(false);
            kinect.Start();
        }

        [TestMethod]
        public void TestMethod1()
        {
            MWNumericArray image = kinect.GetColorImage();
            Assert.IsNotNull(image);
        }
    }
}
