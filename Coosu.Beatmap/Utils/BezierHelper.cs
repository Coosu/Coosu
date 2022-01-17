using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Shared;

namespace Coosu.Beatmap.Utils
{
    public static class BezierHelper
    {
        // Legendre-Gauss abscissae with n=24 (x_i values, defined at i=n as the roots of the nth order Legendre polynomial Pn(x))
        private static readonly float[] TValues =
        {
            -0.0640568928626056260850430826247450385909f,
             0.0640568928626056260850430826247450385909f,
            -0.1911188674736163091586398207570696318404f,
             0.1911188674736163091586398207570696318404f,
            -0.3150426796961633743867932913198102407864f,
             0.3150426796961633743867932913198102407864f,
            -0.4337935076260451384870842319133497124524f,
             0.4337935076260451384870842319133497124524f,
            -0.5454214713888395356583756172183723700107f,
             0.5454214713888395356583756172183723700107f,
            -0.6480936519369755692524957869107476266696f,
             0.6480936519369755692524957869107476266696f,
            -0.7401241915785543642438281030999784255232f,
             0.7401241915785543642438281030999784255232f,
            -0.8200019859739029219539498726697452080761f,
             0.8200019859739029219539498726697452080761f,
            -0.8864155270044010342131543419821967550873f,
             0.8864155270044010342131543419821967550873f,
            -0.9382745520027327585236490017087214496548f,
             0.9382745520027327585236490017087214496548f,
            -0.9747285559713094981983919930081690617411f,
             0.9747285559713094981983919930081690617411f,
            -0.9951872199970213601799974097007368118745f,
             0.9951872199970213601799974097007368118745f,
        };

        // Legendre-Gauss weights with n=24 (w_i values, defined by a function linked to in the Bezier primer article)
        private static readonly float[] CValues =
        {
            0.1279381953467521569740561652246953718517f,
            0.1279381953467521569740561652246953718517f,
            0.1258374563468282961213753825111836887264f,
            0.1258374563468282961213753825111836887264f,
            0.1216704729278033912044631534762624256070f,
            0.1216704729278033912044631534762624256070f,
            0.1155056680537256013533444839067835598622f,
            0.1155056680537256013533444839067835598622f,
            0.1074442701159656347825773424466062227946f,
            0.1074442701159656347825773424466062227946f,
            0.0976186521041138882698806644642471544279f,
            0.0976186521041138882698806644642471544279f,
            0.0861901615319532759171852029837426671850f,
            0.0861901615319532759171852029837426671850f,
            0.0733464814110803057340336152531165181193f,
            0.0733464814110803057340336152531165181193f,
            0.0592985849154367807463677585001085845412f,
            0.0592985849154367807463677585001085845412f,
            0.0442774388174198061686027482113382288593f,
            0.0442774388174198061686027482113382288593f,
            0.0285313886289336631813078159518782864491f,
            0.0285313886289336631813078159518782864491f,
            0.0123412297999871995468056670700372915759f,
            0.0123412297999871995468056670700372915759f,
        };

        public static float ArcFunction(IReadOnlyList<Vector2> points, float t)
        {
            var vec = Compute(points, t);
            var l = vec.X * vec.X + vec.Y * vec.Y;
#if NETCOREAPP3_1_OR_GREATER
            return MathF.Sqrt(l);
#else
            return (float)Math.Sqrt(l);
#endif
        }

        public static IList<Vector2> GetBezierTrail(IReadOnlyList<Vector2> points, int count)
        {
            var allPoints = new List<Vector2>();
            for (int s = 0; s <= count; s++)
            {
                var t = s / (float)count;
                var Vector2 = Compute(points, t);
                allPoints.Add(Vector2);
            }

            return allPoints;
        }

        public static Vector2 Compute(IReadOnlyList<Vector2> points, float t)
        {
            if (t == 0)
            {
                return points[0];
            }

            // constant?
            if (points.Count == 1)
            {
                return points[0];
            }

            if (Math.Abs(t - 1) < 0.0000001)
            {
                return points[points.Count - 1];
            }

            var nt = 1 - t;

            // linear?
            if (points.Count == 2)
            {
                var ret = new Vector2(
                     nt * points[0].X + t * points[1].X,
                     nt * points[0].Y + t * points[1].Y
                );
                return ret;
            }

            // quadratic/cubic curve?
            if (points.Count <= 4)
            {
                var nt2 = nt * nt;
                var t2 = t * t;
                if (points.Count == 3)
                {
                    var a = nt2;
                    var b = nt * t * 2;
                    var c = t2;

                    var ret = new Vector2(
                        a * points[0].X + b * points[1].X + c * points[2].X,
                        a * points[0].Y + b * points[1].Y + c * points[2].Y
                    );
                    return ret;
                }
                else
                {
                    var a = nt2 * nt;
                    var b = nt2 * t * 3;
                    var c = nt * t2 * 3;
                    var d = t * t2;

                    var ret = new Vector2(
                        a * points[0].X + b * points[1].X + c * points[2].X + d * points[3].X,
                        a * points[0].Y + b * points[1].Y + c * points[2].Y + d * points[3].Y
                    );
                    return ret;
                }
            }

            // higher order curves: use de Casteljau's computation
            unsafe
            {
                var span = stackalloc Vector2[points.Count];
                for (var i = 0; i < points.Count; i++)
                {
                    span[i] = points[i];
                }

                int j = points.Count;
                while (j > 1)
                {
                    for (int i = 0; i < j; i++)
                    {
                        if (i != j - 1)
                        {
                            span[i] = nt * span[i] + t * span[i + 1];
                        }
                        else
                        {
                            span[i] = nt * span[i];
                        }
                    }

                    j--;
                }

                return span[0];
            }
        }

        public static IEnumerable<IReadOnlyList<Vector2>> Derive(IReadOnlyList<Vector2> controlPoints)
        {
            for (int d = controlPoints.Count, c = d - 1; d > 1; d--, c--)
            {
                var list = new List<Vector2>();

                for (var j = 0; j < c; j++)
                {
                    var dpt = new Vector2(
                        c * (controlPoints[j + 1].X - controlPoints[j].X),
                        c * (controlPoints[j + 1].Y - controlPoints[j].Y)
                    );

                    list.Add(dpt);
                }

                yield return list;
                controlPoints = list;
            }

            yield return EmptyArray<Vector2>.Value;
        }

        public static float Length(IReadOnlyList<Vector2> controlPoints)
        {
            const float z = 0.5f;
            float sum = 0;

            var derivedVector2 = Derive(controlPoints).First();

            for (int i = 0; i < TValues.Length; i++)
            {
                var t = z * TValues[i] + z;
                var arc = ArcFunction(derivedVector2, t);
                sum += CValues[i] * arc;
            }

            return z * sum;
        }
    }

}
