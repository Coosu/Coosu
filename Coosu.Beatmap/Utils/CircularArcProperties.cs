// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Numerics;

namespace osu.Framework.Utils;

public readonly struct CircularArcProperties
{
    public readonly bool IsValid;
    public readonly double ThetaStart;
    public readonly double ThetaRange;
    public readonly double Direction;
    public readonly float Radius;
    public readonly Vector3 Centre;

    public double ThetaEnd => ThetaStart + ThetaRange * Direction;

    public CircularArcProperties(double thetaStart, double thetaRange, double direction, float radius, Vector3 centre)
    {
        IsValid = true;
        ThetaStart = thetaStart;
        ThetaRange = thetaRange;
        Direction = direction;
        Radius = radius;
        Centre = centre;
    }

    /// <summary>
    /// Computes various properties that can be used to approximate the circular arc.
    /// </summary>
    /// <param name="controlPoints">Three distinct points on the arc.</param>
    public CircularArcProperties(ReadOnlySpan<Vector3> controlPoints)
    {
        Vector3 a = controlPoints[0];
        Vector3 b = controlPoints[1];
        Vector3 c = controlPoints[2];

        // If we have a degenerate triangle where a side-length is almost zero, then give up and fallback to a more numerically stable method.
        if (Precision.AlmostEquals(0, (b.Y - a.Y) * (c.X - a.X) - (b.X - a.X) * (c.Y - a.Y)))
        {
            IsValid = false;
            ThetaStart = default;
            ThetaRange = default;
            Direction = default;
            Radius = default;
            Centre = default;
            return;
        }

        // See: https://en.wikipedia.org/wiki/Circumscribed_circle#Cartesian_coordinates_2
        float d = 2 * (a.X * (b - c).Y + b.X * (c - a).Y + c.X * (a - b).Y);
        float aSq = a.LengthSquared();
        float bSq = b.LengthSquared();
        float cSq = c.LengthSquared();

        Centre = new Vector3(
            aSq * (b - c).Y + bSq * (c - a).Y + cSq * (a - b).Y,
            aSq * (c - b).X + bSq * (a - c).X + cSq * (b - a).X,
            0) / d;

        Vector3 dA = a - Centre;
        Vector3 dC = c - Centre;

        Radius = dA.Length();

        ThetaStart = Math.Atan2(dA.Y, dA.X);
        double thetaEnd = Math.Atan2(dC.Y, dC.X);

        while (thetaEnd < ThetaStart)
            thetaEnd += 2 * Math.PI;

        Direction = 1;
        ThetaRange = thetaEnd - ThetaStart;

        // Decide in which direction to draw the circle, depending on which side of
        // AC B lies.
        Vector3 orthoAtoC = c - a;
        orthoAtoC = new Vector3(orthoAtoC.Y, -orthoAtoC.X, 0);

        if (Vector3.Dot(orthoAtoC, b - a) < 0)
        {
            Direction = -Direction;
            ThetaRange = 2 * Math.PI - ThetaRange;
        }

        IsValid = true;
    }
}