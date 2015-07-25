using Microsoft.Kinect.Toolkit.FaceTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectConnect.Core.SDK1x
{
    [Serializable]
    public class FaceData
    {
        public FaceData(Dictionary<string, Vec3> facePoints, 
            Dictionary<string, Vec2> projectedFacePoints,
            Dictionary<string, float> animationUnits,
            Vec3 rotation,
            Vec3 translation)
        {
            this.FacePoints =                       facePoints;
            this.ProjectedFacePoints =              projectedFacePoints;
            this.AnimationUnits =                   animationUnits;
            this.Rotation =                         rotation;
            this.Translation =                      translation;
        }

        public Dictionary<string, Vec3> FacePoints { get; set; }

        public Dictionary<string, Vec2> ProjectedFacePoints { get; set; }

        public Dictionary<string, float> AnimationUnits { get; set; }

        public Vec3 Rotation { get; set; }
        public Vec3 Translation { get; set; }

        public override string ToString()
        {
            return string.Format("{0} FacePoints, {1} ProjectedFacePoints, {2} AnimationUnits", FacePoints.Count, ProjectedFacePoints.Count, AnimationUnits.Count);
        }

    }

    [Serializable]
    public struct Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    [Serializable]
    public struct Vec2
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public static class SerialUtils
    {
        /// <summary>
        /// Converts a Kinect Toolkit FaceTrackFrame to a KinectConnect
        /// FaceData.
        /// </summary>
        /// <param name="frame">Kinect Toolkit FaceTrackFrame</param>
        /// <returns>KinectConnect FaceData</returns>
        public static FaceData ToSerializableFaceData(this FaceTrackFrame frame)
        {
            var shape = frame.Get3DShape().ToDictionary(x => { return x.ToSerializableVec3(); });
            var projected = frame.GetProjected3DShape().ToDictionary(x => { return x.ToSerializablePointF(); });
            var units = frame.GetAnimationUnitCoefficients().ToDictionary(x => { return x; });
            var rotation = frame.Rotation.ToSerializableVec3();
            var translation = frame.Translation.ToSerializableVec3();

            return new FaceData(
                shape, 
                projected, 
                units,
                rotation, 
                translation
            );
        }

        /// <summary>
        /// Converts a Kinect Toolkit Vector3DF to KinectConnect Vec3.
        /// </summary>
        /// <param name="point">Kinect Toolkit Vector3DF</param>
        /// <returns>KinectConnect Vec2</returns>
        public static Vec3 ToSerializableVec3(this Vector3DF vec)
        {
            return new Vec3()
            {
                X = vec.X,
                Y = vec.Y,
                Z = vec.Z
            };
        }

        /// <summary>
        /// Converts a Kinect Toolkit PointF to KinectConnect Vec2.
        /// </summary>
        /// <param name="point">Kinect Toolkit PointF</param>
        /// <returns>KinectConnect Vec2</returns>
        public static Vec2 ToSerializablePointF(this Microsoft.Kinect.Toolkit.FaceTracking.PointF point)
        {
            return new Vec2()
            {
                X = point.X,
                Y = point.Y
            };
        }

        /// <summary>
        /// Converts EnumIndexableCollection to List of TData and List of string descriptions.
        /// 
        /// ** BEFORE YOU JUDGE ME **
        /// I know this is hard to stomach. Anything with more than two type parameters
        /// gives me indigestion, however, MS devs saw fit to create a real monstrosity with
        /// the Kinect SDK v1. The EnumIndexableCollection. This may be a semi-useful format
        /// at compile time, but we are serving data dynamically at runtime to other applications, 
        /// so we have to find an easy way to convert to a SANE data type, like a dictionary.
        /// 
        /// The best bit is, ALL the type parameters are inferred. Yes, ALL. Example:
        /// 
        /// var units = frame.GetAnimationUnitCoefficients().ToDictionary(x => { return x; });
        /// 
        /// See, how awesome is that?
        /// 
        /// </summary>
        /// <typeparam name="TEnum">Type of enumeration the collection is indexed by</typeparam>
        /// <typeparam name="TFrom">Type in the collection to convert from</typeparam>
        /// <typeparam name="TTo">Type to convert to</typeparam>
        /// <param name="points">The collection</param>
        /// <param name="converter">Function converting from TFrom to TTo</param>
        /// <returns>A tuple with a list of the data points, and matching descriptors for each.</returns>
        public static Dictionary<string, TTo> ToDictionary<TEnum, TFrom, TTo>(
            this EnumIndexableCollection<TEnum, TFrom> points, 
            Func<TFrom, TTo> converter)
        {
            Dictionary<string, TTo> data = new Dictionary<string, TTo>();

            foreach (var key in Enum.GetValues(typeof(TEnum)))
            {
                TFrom datapoint = points[(TEnum)key];
                TTo converted = converter(datapoint);
                data.Add(key.ToString(), converted);
            }

            return data;
        }

    }
}
