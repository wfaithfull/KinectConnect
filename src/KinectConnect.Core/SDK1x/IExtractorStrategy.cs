using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.Core.SDK1x
{
    /// <summary>
    /// Represents a strategy for Kinect data extraction 
    /// as per the strategy pattern.
    /// </summary>
    public interface IExtractorStrategy
    {
        /// <summary>
        /// The Capabilities required of the kinect to extract
        /// the data for this strategy.
        /// </summary>
        /// <returns>Required capabilities for this strategy</returns>
        Capabilities RequiredCapabilities();

        /// <summary>
        /// Initialises the <paramref name="sensor"/> provided with the
        /// capabilities required for this strategy.
        /// </summary>
        /// <param name="sensor">The kinect sensor</param>
        void Initialise(KinectSensor sensor);

        /// <summary>
        /// Called by the Extractor to extract the data.
        /// </summary>
        /// <param name="args">A kinect toolkit allframereadyeventargs</param>
        void Extract(AllFramesReadyEventArgs args);
    }

    /// <summary>
    /// Sub interface to abstract type parameter requirements to
    /// the event only.
    /// </summary>
    /// <typeparam name="T">The type of data to be extracted</typeparam>
    public interface IEventedExtractorStrategy<T> : IExtractorStrategy
    {
        /// <summary>
        /// Event that fires when data has been extracted from the kinect.
        /// </summary>
        event Action<T> DataExtracted;
    }

}
