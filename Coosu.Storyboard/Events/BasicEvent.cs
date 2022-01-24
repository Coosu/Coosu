using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    [DebuggerDisplay("Expression = {DebuggerDisplay}")]
    public abstract class BasicEvent : IKeyEvent
    {
        public virtual double DefaultValue { get; } = 0;

        private double[]? _end;
        internal List<double> ListValue;
        private string DebuggerDisplay => this.GetHeaderString();

        public abstract EventType EventType { get; }
        public EasingFunctionBase Easing { get; set; } = LinearEase.Instance;
        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public IReadOnlyList<double> Value
        {
            get => ListValue;
            internal set => ListValue = (List<double>)value;
        }

        protected void Fill(int count)
        {
            var index = count - 1;
            if (index > ListValue.Count - 1)
            {
                while (index > ListValue.Count - 1)
                {
                    ListValue.Add(DefaultValue);
                }
            }

        }

        // todo: 大于size时自动匹配为start
        protected double GetValue(int index)
        {
            if (index > ListValue.Count - 1)
            {
                while (index > ListValue.Count - 1)
                {
                    ListValue.Add(DefaultValue);
                }

                return DefaultValue;
            }

            return ListValue[index];
        }

        protected void SetValue(int index, double value)
        {
            if (index > ListValue.Count - 1)
                while (index > ListValue.Count - 1)
                {
                    if (index == ListValue.Count)
                        ListValue.Add(value);
                    else
                        ListValue.Add(DefaultValue);
                }
            else
                ListValue[index] = value;
        }

        //public double[] Start { get; set; }

        //public double[] End
        //{
        //    get => EventType.Size == 0 ? Start : _end!;
        //    set
        //    {
        //        if (EventType.Size == 0) Start = value;
        //        else _end = value;
        //    }
        //}

        public virtual bool IsStatic => Start.SequenceEqual(End);

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

        //protected BasicEvent()
        //{
        //    Value = EmptyArray<double>.Value;
        //}

        protected BasicEvent(EasingFunctionBase easing, double startTime, double endTime, List<double> value)
        {
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            Start = start.ToArray();
            End = end == default ? Start : end.ToArray();
        }

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
            for (int i = 0; i < EventType.Size; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                if (i != EventType.Size - 1) await textWriter.WriteAsync(',');
            }
        }

        private async Task WriteFullAsync(TextWriter textWriter)
        {
            for (int i = 0; i < EventType.Size; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                await textWriter.WriteAsync(',');
            }

            for (int i = 0; i < EventType.Size; i++)
            {
                await textWriter.WriteAsync(End[i].ToIcString());
                if (i != EventType.Size - 1) await textWriter.WriteAsync(',');
            }
        }

        public static IKeyEvent Create(EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            List<double> value)
        {
            var size = e.Size;
            if (size != 0 && value.Count != size && value.Count != size * 2)
                throw new ArgumentException();
            if (size == 0)
                return Create(e, easing, startTime, endTime, value.Slice(0, 1), default);
            return Create(e, easing, startTime, endTime,
                value.Slice(0, size),
                value.Length == size ? default : value.Slice(size, size));
        }

        public static IKeyEvent Create(EventType e, EasingFunctionBase easing,
            double startTime, double endTime, Span<double> start, Span<double> end)
        {
            IKeyEvent keyEvent;
            if (e.Size != 0 && end.Length == 0)
                end = start.ToArray();

            if (e.Size != 0 && start.Length != e.Size) throw new ArgumentException();

            if (e == EventTypes.Fade)
                keyEvent = new Fade(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Move)
                keyEvent = new Move(easing, startTime, endTime, start, end);
            else if (e == EventTypes.MoveX)
                keyEvent = new MoveX(easing, startTime, endTime, start, end);
            else if (e == EventTypes.MoveY)
                keyEvent = new MoveY(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Scale)
                keyEvent = new Scale(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Vector)
                keyEvent = new Vector(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Rotate)
                keyEvent = new Rotate(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Color)
                keyEvent = new Color(easing, startTime, endTime, start, end);
            else if (e == EventTypes.Parameter)
                keyEvent = new Parameter(startTime, endTime, start.Slice(0, 1));
            else
            {
                var result = HandlerRegister.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, start, end);
                keyEvent = result ?? throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            return keyEvent;
        }

        public object Clone()
        {
            return BasicEvent.Create(EventType, Easing, StartTime, EndTime, Start.ToArray(), End.ToArray());
        }
    }
}
