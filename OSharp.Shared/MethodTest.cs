using System;
using System.Diagnostics;

namespace OSharp.Shared
{
    public static class MethodTest
    {
        public static TimeSpan CalculateTime(Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            action.Invoke();
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
