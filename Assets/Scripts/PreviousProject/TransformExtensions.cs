using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public static class TransformExtensions
{

    public static TransformMsg ToROSTransform(this Transform tfUnity)
    {
        return new TransformMsg(
            // Using vector/quaternion To<>() because Transform.To<>() doesn't use localPosition/localRotation
            tfUnity.localPosition.To<FLU>(),
            tfUnity.localRotation.To<FLU>());
    }

    public static TransformStampedMsg ToROSTransformStamped(this Transform tfUnity, double timeStamp,string prefix)
    {
        return new TransformStampedMsg(
            new HeaderMsg(1,new TimeStamp(timeStamp), prefix+"/"+tfUnity.parent.gameObject.name),
            prefix+"/"+tfUnity.gameObject.name,
            tfUnity.ToROSTransform());
    }
}
