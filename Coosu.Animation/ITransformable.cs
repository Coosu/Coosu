using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Animation
{
    public interface ITransformable<T> where T : struct
    {
        void Fade(Easing easing, double startTime, double endTime, T startOpacity, T endOpacity);
        void Rotate(Easing easing, double startTime, double endTime, T startDeg, T endDeg);

        void Move(Easing easing, double startTime, double endTime, Vector2<T> startPos, Vector2<T> endPos);
        void MoveX(Easing easing, double startTime, double endTime, T startX, T endX);
        void MoveY(Easing easing, double startTime, double endTime, T startY, T endY);
        void ScaleVec(Easing easing, double startTime, double endTime, Vector2<T> startSize, Vector2<T> endSize);
        void Color(Easing easing, double startTime, double endTime, Vector3<T> startColor, Vector3<T> endColor);

        void Blend(double startTime, double endTime, BlendMode mode);

        void Flip(double startTime, double endTime, FlipMode mode);
    }
}
