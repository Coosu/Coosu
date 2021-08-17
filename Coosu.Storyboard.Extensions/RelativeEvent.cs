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
        public double[] Start { get; set; }
        public double[] End
        {
            get => Start;
            set => Start = value;
        }

        public virtual int ParamLength => Start.Length;
        public virtual bool IsStatic => Start.SequenceEqual(End);

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
            Start = Array.Empty<double>();
            End = Array.Empty<double>();
        }

        public RelativeEvent(EventType eventType, IEasingFunction easing, double startTime, double endTime, double[] byValue)
        {
            EventType = eventType;
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            Start = byValue;
        }

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            await WriteStartAsync(textWriter);
        }

        private async Task WriteStartAsync(TextWriter textWriter)
        {
            for (int i = 0; i < ParamLength; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                if (i != ParamLength - 1) await textWriter.WriteAsync(',');
            }
        }
    }
}