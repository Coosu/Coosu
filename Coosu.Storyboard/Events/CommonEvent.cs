using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    [DebuggerDisplay("Expression = {DebuggerDisplay}")]
    public abstract class CommonEvent : ICommonEvent //,IComparable<CommonEvent>
    {
        private string DebuggerDisplay => this.GetHeaderString();

        public abstract EventType EventType { get; }
        public IEasingFunction Easing { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public double[] Start { get; set; }
        public double[] End { get; set; }

        public virtual int ParamLength => Start.Length;
        public virtual bool IsStatic => Start.SequenceEqual(End);

        //public int CompareTo(CommonEvent? other)
        //{
        //    if (other == null)
        //        return 1;
        //    if (StartTime > other.StartTime)
        //        return 1;
        //    if (StartTime.Equals(other.StartTime))
        //        return 0;
        //    if (StartTime < other.StartTime)
        //        return -1;
        //    throw new ArgumentOutOfRangeException(nameof(other));
        //}

        public void AdjustTiming(double offset)
        {
            StartTime += offset;
            EndTime += offset;
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync(EventType.Flag);
            await writer.WriteAsync(',');
            var typeStr = ((int?)Easing.TryGetEasingType())?.ToString() ?? "?";
            await writer.WriteAsync(typeStr);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Math.Round(StartTime));
            await writer.WriteAsync(',');
            if (!EndTime.Equals(StartTime))
                await writer.WriteAsync(Math.Round(EndTime));
            await writer.WriteAsync(',');
            await WriteExtraScriptAsync(writer);
        }

        public virtual async Task WriteScriptAsync(TextWriter writer) =>
            await WriteHeaderAsync(writer);

        protected CommonEvent()
        {
            Start = Array.Empty<double>();
            End = Array.Empty<double>();
        }

        protected CommonEvent(IEasingFunction easing, double startTime, double endTime, double[] start, double[] end)
        {
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            Start = start;
            End = end;
        }

        //protected CommonEvent(EasingType easing, double startTime, double endTime, double[] start, double[] end)
        //{
        //    Easing = easing.ToEasingFunction();
        //    StartTime = startTime;
        //    EndTime = endTime;
        //    Start = start;
        //    End = end;
        //}

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            bool sequenceEqual = Start.SequenceEqual(End);

            if (sequenceEqual)
                await WriteStartAsync(textWriter);
            else
                await WriteFullAsync(textWriter);
        }

        private async Task WriteStartAsync(TextWriter textWriter)
        {
            for (int i = 0; i < ParamLength; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                if (i != ParamLength - 1) await textWriter.WriteAsync(',');
            }
        }

        private async Task WriteFullAsync(TextWriter textWriter)
        {
            for (int i = 0; i < ParamLength; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                await textWriter.WriteAsync(',');
            }

            for (int i = 0; i < ParamLength; i++)
            {
                await textWriter.WriteAsync(End[i].ToIcString());
                if (i != ParamLength - 1) await textWriter.WriteAsync(',');
            }
        }

        public static ICommonEvent Create(EventType e, EasingType easing, double startTime, double endTime,
            params double[] parameters)
        {
            return Create(e, easing.ToEasingFunction(), startTime, endTime, parameters);
        }

        //todo: use Span<T>
        public static ICommonEvent Create(EventType e, IEasingFunction easing, double startTime, double endTime,
            params double[] parameters)
        {
            var size = e.Size;
            if (parameters.Length != size * 2) throw new ArgumentException();
            return Create(e, easing, startTime, endTime,
                parameters.Take(size).ToArray(),
                parameters.Skip(size).ToArray());
        }

        public static ICommonEvent Create(EventType e, EasingType easing,
            double startTime, double endTime, double[] start, double[]? end)
        {
            return Create(e, easing.ToEasingFunction(), startTime, endTime, start, end);
        }

        public static ICommonEvent Create(EventType e, IEasingFunction easing,
            double startTime, double endTime, double[] start, double[]? end)
        {
            ICommonEvent commonEvent;
            if (end == null || end.Length == 0)
                end = start;

            if (e == EventTypes.Fade)
            {
                commonEvent = new Fade(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Move)
            {
                commonEvent = new Move(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.MoveX)
            {
                commonEvent = new MoveX(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.MoveY)
            {
                commonEvent = new MoveY(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Scale)
            {
                commonEvent = new Scale(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Vector)
            {
                commonEvent = new Vector(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.Rotate)
            {
                commonEvent = new Rotate(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Color)
            {
                commonEvent = new Color(easing, startTime, endTime, start[0], start[1], start[2], end[0], end[1],
                    end[2]);
            }
            else if (e == EventTypes.Parameter)
            {
                commonEvent = new Parameter(startTime, endTime, (ParameterType)(int)start[0]);
            }
            else
            {
                var result = HandlerRegister.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, start, end);
                commonEvent = result ?? throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            return commonEvent;
        }
    }
}
