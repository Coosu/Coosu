using System;
using System.Collections.Generic;

namespace OSharp.Beatmap
{
    public static class Bezier
    {
        /// <summary>
        /// 绘制n阶贝塞尔曲线路径
        /// </summary>
        /// <param name="points">输入点</param>
        /// <param name="step">步长,步长越小，轨迹点越密集</param>
        /// <returns></returns>
        public static Vector2<float>[] GetBezierTrail(IReadOnlyList<Vector2<float>> points, float step)
        {
            var curvePoints = new List<Vector2<float>>();
            float t = 0F;
            do
            {
                Vector2<float> result = CalcPoint(t, points);    // 计算插值点
                t += step;
                curvePoints.Add(result);
            }
            while (t <= 1 && points.Count > 1);    // 一个点的情况直接跳出.

            return curvePoints.ToArray();  // 曲线轨迹上的所有坐标点
        }

        /// <summary>
        /// n阶贝塞尔曲线插值计算函数
        /// 根据起点，n个控制点，终点 计算贝塞尔曲线插值
        /// </summary>
        /// <param name="ratio">当前插值位置0~1 ，0为起点，1为终点</param>
        /// <param name="points">起点，n-1个控制点，终点</param>
        /// <returns></returns>
        public static Vector2<float> CalcPoint(float ratio, IReadOnlyList<Vector2<float>> points)
        {
            float sumX = 0, sumY = 0;
            var count = points.Count;
            for (int i = 0; i < count; i++)
            {
                int order = count - 1; // 阶数
                var combination = CalcCombination(order, i);
                sumX += (float)(combination * points[i].X * Math.Pow(1 - ratio, order - i) * Math.Pow(ratio, i));
                sumY += (float)(combination * points[i].Y * Math.Pow(1 - ratio, order - i) * Math.Pow(ratio, i));
            }

            var vector2 = new Vector2<float>(sumX, sumY);
            return vector2;
        }

        /// <summary>
        /// 计算组合数公式
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static ulong CalcCombination(int n, int k)
        {
            ulong[] result = new ulong[n + 1];
            for (int i = 1; i <= n; i++)
            {
                result[i] = 1;
                for (int j = i - 1; j >= 1; j--)
                    result[j] += result[j - 1];
                result[0] = 1;
            }

            return result[k];
        }
    }

}
