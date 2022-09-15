namespace Coosu.Animation;

public struct TransformAction
{
    public TransformAction(Easing easing, double startTime, double endTime, object startParam, object endParam)
    {
        Easing = easing;
        StartTime = startTime;
        EndTime = endTime;
        StartParam = startParam;
        EndParam = endParam;
    }

    public Easing Easing { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public object StartParam { get; set; }
    public object EndParam { get; set; }
}