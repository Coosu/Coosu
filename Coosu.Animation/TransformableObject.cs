using System;
using System.Collections.Generic;
using System.Linq;

namespace Coosu.Animation;

public abstract class TransformableObject<T> where T : struct
{
    protected abstract void FadeAction(List<TransformAction> actions);
    protected abstract void RotateAction(List<TransformAction> actions);
    protected abstract void MoveAction(List<TransformAction> actions);
    protected abstract void Move1DAction(List<TransformAction> actions, bool isHorizon);
    protected abstract void ScaleVecAction(List<TransformAction> actions);
    protected abstract void ColorAction(List<TransformAction> actions);
    protected abstract void BlendAction(List<TransformAction> actions);
    protected abstract void FlipAction(List<TransformAction> actions);

    protected abstract void HandleLoopGroup((double startTime, int loopTimes, ITransformable<T> transformList) tuple);

    protected abstract void StartAnimation();
    protected abstract void EndAnimation();
    public void ApplyAnimation(Action<ITransformable<T>> func)
    {
        StartAnimation();
        var internalObj = new InternalTransformableObject<T>();
        func?.Invoke(internalObj);
        var time1 = internalObj.TransformDictionary.Count > 0
            ? internalObj.TransformDictionary.Min(k => k.Value.Min(o => o.StartTime))
            : int.MaxValue;
        var time2 = internalObj.loopList.Count > 0
            ? internalObj.loopList.Min(k => k.startTime)
            : int.MaxValue;
            
        var time3 = internalObj.TransformDictionary.Count > 0
            ? internalObj.TransformDictionary.Max(k => k.Value.Max(o => o.EndTime))
            : int.MinValue;
        var time4 = internalObj.loopList.Count > 0
            ? internalObj.loopList.Max(k =>
                k.transformList.TransformDictionary.Max(o => o.Value.Max(s => s.EndTime)))
            : int.MinValue;
        MinTime = Math.Min(time1, time2);
        MaxTime = Math.Max(time3, time4);

        foreach (var transform in internalObj.TransformDictionary)
        {
            switch (transform.Key)
            {
                case TransformType.Fade:
                    FadeAction(transform.Value);
                    break;
                case TransformType.Rotate:
                    RotateAction(transform.Value);
                    break;
                case TransformType.Move:
                    MoveAction(transform.Value);
                    break;
                case TransformType.MoveX:
                    Move1DAction(transform.Value, true);
                    break;
                case TransformType.MoveY:
                    Move1DAction(transform.Value, false);
                    break;
                case TransformType.ScaleVec:
                    ScaleVecAction(transform.Value);
                    break;
                case TransformType.Color:
                    ColorAction(transform.Value);
                    break;
                case TransformType.Blend:
                    BlendAction(transform.Value);
                    break;
                case TransformType.Flip:
                    FlipAction(transform.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var tuple in internalObj.loopList)
        {
            HandleLoopGroup(tuple);
        }

        EndAnimation();
    }

    public double MaxTime { get; set; }

    public double MinTime { get; set; }
}