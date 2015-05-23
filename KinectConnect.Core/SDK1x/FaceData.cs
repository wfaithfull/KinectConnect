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
        public FaceData(List<Vec3> facePoints, 
            List<string> facePointDescriptors,
            List<PointF> projectedFacePoints,
            List<string> projectedFacePointDescriptors,
            List<float> animationUnits,
            List<string> animationUnitDescriptors,
            Vec3 rotation,
            Vec3 translation)
        {
            this.FacePoints =                       facePoints;
            this.FacePointDescriptors =             facePointDescriptors;
            this.ProjectedFacePoints =              projectedFacePoints;
            this.ProjectedFacePointDescriptors =    projectedFacePointDescriptors;
            this.AnimationUnits =                   animationUnits;
            this.AnimationUnitDescriptors =         animationUnitDescriptors;
            this.Rotation =                         rotation;
            this.Translation =                      translation;
        }

        public List<Vec3> FacePoints { get; set; }
        public List<string> FacePointDescriptors { get; set; }

        public List<PointF> ProjectedFacePoints { get; set; }
        public List<string> ProjectedFacePointDescriptors { get; set; }

        public List<float> AnimationUnits { get; set; }
        public List<string> AnimationUnitDescriptors { get; set; }

        public Vec3 Rotation { get; set; }
        public Vec3 Translation { get; set; }

    }

    [Serializable]
    public struct Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    [Serializable]
    public struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public static class SerialUtils
    {
        public static FaceData ToSerializableFaceData(this FaceTrackFrame frame)
        {
            var shape = frame.Get3DShape().ToSerializableTuple(x => { return x.ToSerializableVec3(); });
            var units = frame.GetAnimationUnitCoefficients().ToSerializableTuple(x => { return x; });
            var projected = frame.GetProjected3DShape().ToSerializableTuple(x => { return x.ToSerializablePointF(); });
            var rotation = frame.Rotation.ToSerializableVec3();
            var translation = frame.Translation.ToSerializableVec3();

            return new FaceData(
                shape.Item1, shape.Item2,
                projected.Item1, projected.Item2,
                units.Item1, units.Item2,
                rotation, translation
            );
        }

        public static Vec3 ToSerializableVec3(this Vector3DF vec)
        {
            return new Vec3()
            {
                X = vec.X,
                Y = vec.Y,
                Z = vec.Z
            };
        }

        public static PointF ToSerializablePointF(this Microsoft.Kinect.Toolkit.FaceTracking.PointF point)
        {
            return new PointF()
            {
                X = point.X,
                Y = point.Y
            };
        }

        /// <summary>
        /// Face melting extension conversion method.
        /// 
        /// Converts EnumIndexableCollection to List of TData and List of string descriptions.
        /// </summary>
        /// <typeparam name="TEnum">Type of enumeration the collection is indexed by</typeparam>
        /// <typeparam name="TFrom">Type in the collection to convert from</typeparam>
        /// <typeparam name="TTo">Type to convert to</typeparam>
        /// <param name="points">The collection</param>
        /// <param name="converter">Function converting from TFrom to TTo</param>
        /// <returns>A tuple with a list of the data points, and matching descriptors for each.</returns>
        public static Tuple<List<TTo>, List<string>> ToSerializableTuple<TEnum, TFrom, TTo>(
            this EnumIndexableCollection<TEnum, TFrom> points, 
            Func<TFrom, TTo> converter)
        {
            List<TTo> data = new List<TTo>();
            List<string> descriptors = new List<string>();

            foreach (var key in Enum.GetValues(typeof(TEnum)))
            {
                TFrom datapoint = points[(TEnum)key];
                TTo converted = converter(datapoint);
                data.Add(converted);
                descriptors.Add(key.ToString());
            }

            return new Tuple<List<TTo>, List<string>>(data,descriptors);
        }

    }
}
