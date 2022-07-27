using System;
using System.Collections.Generic;
using System.Numerics;

namespace Coosu.Beatmap.Utils;

internal static class Interpolation
{
    /// <summary>
    /// Calculates the Barycentric weights for a Lagrange polynomial for a given set of coordinates. Can be used as a helper function to compute a Lagrange polynomial repeatedly.
    /// </summary>
    /// <param name="points">An array of coordinates. No two x should be the same.</param>
    public static double[] BarycentricWeights(IReadOnlyList<Vector2> points)
    {
        int n = points.Count;
        double[] w = new double[n];

        for (int i = 0; i < n; i++)
        {
            w[i] = 1;

            for (int j = 0; j < n; j++)
            {
                if (i != j)
                    w[i] *= points[i].X - points[j].X;
            }

            w[i] = 1.0 / w[i];
        }

        return w;
    }

    /// <summary>
    /// Calculates the Lagrange basis polynomial for a given set of x coordinates based on previously computed barycentric weights.
    /// </summary>
    /// <param name="points">An array of coordinates. No two x should be the same.</param>
    /// <param name="weights">An array of precomputed barycentric weights.</param>
    /// <param name="time">The x coordinate to calculate the basis polynomial for.</param>
    public static double BarycentricLagrange(IReadOnlyList<Vector2> points, double[] weights, double time)
    {
        if (points == null || points.Count == 0)
            throw new ArgumentException($"{nameof(points)} must contain at least one point");
        if (points.Count != weights.Length)
            throw new ArgumentException($"{nameof(points)} must contain exactly as many items as {nameof(weights)}");

        double numerator = 0;
        double denominator = 0;

        for (int i = 0; i < points.Count; i++)
        {
            // while this is not great with branch prediction, it prevents NaN at control point X coordinates
            if (time == points[i].X)
                return points[i].Y;

            double li = weights[i] / (time - points[i].X);
            numerator += li * points[i].Y;
            denominator += li;
        }

        return numerator / denominator;
    }
}