using System;
using System.Collections.Generic;

namespace Coosu.Animation
{
    public class InternalTransformableObject<T> : ITransformable<T> where T : struct
    {
        internal readonly Dictionary<TransformType, List<TransformAction>> TransformDictionary =
            new Dictionary<TransformType, List<TransformAction>>();

        internal bool _supportLoop = true;

        internal List<(double startTime, int loopTimes, InternalTransformableObject<T> transformList)> loopList =
            new List<(double, int, InternalTransformableObject<T>)>();
        public void Fade(Easing easing, double startTime, double endTime, T startOpacity, T endOpacity)
        {
            const TransformType type = TransformType.Fade;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startOpacity, endOpacity));
        }

        public void Rotate(Easing easing, double startTime, double endTime, T startDeg, T endDeg)
        {
            const TransformType type = TransformType.Rotate;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startDeg, endDeg));
        }

        public void Move(Easing easing, double startTime, double endTime, Vector2<T> startPos, Vector2<T> endPos)
        {
            const TransformType type = TransformType.Move;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startPos, endPos));
        }

        public void MoveX(Easing easing, double startTime, double endTime, T startX, T endX)
        {
            const TransformType type = TransformType.MoveX;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startX, endX));
        }

        public void MoveY(Easing easing, double startTime, double endTime, T startY, T endY)
        {
            const TransformType type = TransformType.MoveY;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startY, endY));
        }

        public void ScaleVec(Easing easing, double startTime, double endTime, Vector2<T> startSize, Vector2<T> endSize)
        {
            const TransformType type = TransformType.ScaleVec;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startSize, endSize));
        }

        public void Color(Easing easing, double startTime, double endTime, Vector3<T> startColor, Vector3<T> endColor)
        {
            const TransformType type = TransformType.Color;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(easing, startTime, endTime, startColor, endColor));
        }

        public void Blend(double startTime, double endTime, BlendMode mode)
        {
            const TransformType type = TransformType.Blend;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(Easing.Linear, startTime, endTime, mode, mode));
        }

        public void Flip(double startTime, double endTime, FlipMode mode)
        {
            const TransformType type = TransformType.Flip;
            AddKey(type);
            TransformDictionary[type].Add(new TransformAction(Easing.Linear, startTime, endTime, mode, mode));
        }


        public void StartLoopGroup(double startTime, int loopTimes, Action<ITransformable<T>> func)
        {
            var loopGroup = new InternalTransformableObject<T>
            {
                _supportLoop = false
            };

            func?.Invoke(loopGroup);
            loopList.Add((startTime, loopTimes, loopGroup));
        }

        private void AddKey(TransformType type)
        {
            if (!TransformDictionary.ContainsKey(type))
            {
                TransformDictionary.Add(type, new List<TransformAction>());
            }
        }
    }
}