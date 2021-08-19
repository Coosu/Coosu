using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions
{
    [DebuggerDisplay("Expression = {DebuggerDisplay}")]
    public class RelativeEvent : ICommonEvent
    {
        private string DebuggerDisplay => this.GetHeaderString();
        public EventType EventType { get; }

        public IEasingFunction Easing { get; set; } = LinearEase.Instance;

        public double StartTime { get; set; }
        public double EndTime { get; set; }

        /// <summary>
        /// For relative event this is a tag.
        /// </summary>
        public double[] Start { get; set; }

        /// <summary>
        /// This is actual relative value.
        /// </summary>
        public double[] End { get; set; }
        public virtual bool IsStatic => End.All(k => k == 0);

        public void AdjustTiming(double offset)
        {
            StartTime += offset;
            EndTime += offset;
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            //await writer.WriteAsync("(Relative) " + EventType.Flag.TrimEnd('c'));
            await writer.WriteAsync(EventType.Flag);
            await writer.WriteAsync(',');
            var typeStr = ((int?)Easing.TryGetEasingType())?.ToString() ?? "?";
            await writer.WriteAsync(typeStr);
            await writer.WriteAsync(" [");
            await writer.WriteAsync(Math.Round(StartTime));
            await writer.WriteAsync(",");
            await writer.WriteAsync(Math.Round(EndTime));
            await writer.WriteAsync("] by [");
            await WriteExtraScriptAsync(writer);
            await writer.WriteAsync(']');
        }

        public virtual async Task WriteScriptAsync(TextWriter writer) =>
            await WriteHeaderAsync(writer);

        public RelativeEvent(EventType eventType)
        {
            EventType = eventType;
            Start = EmptyArray<double>.Value;
            End = EmptyArray<double>.Value;
        }

        public RelativeEvent(EventType eventType, IEasingFunction easing, double startTime, double endTime, double[] byValue)
        {
            EventType = eventType;
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            End = byValue;
            Start = new double[eventType.Size];
        }

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            await WriteStartAsync(textWriter);
        }

        private async Task WriteStartAsync(TextWriter textWriter)
        {
            for (int i = 0; i < EventType.Size; i++)
            {
                if (Start[i] != 0)
                {
                    await textWriter.WriteAsync(Start[i].ToIcString());
                    await textWriter.WriteAsync('~');
                }

                await textWriter.WriteAsync(End[i].ToIcString());
                if (i != EventType.Size - 1) await textWriter.WriteAsync(',');
            }
        }

        public object Clone()
        {
            return new RelativeEvent(EventType, Easing, StartTime, EndTime, End)
            {
                Start = Start
            };
        }
    }
}