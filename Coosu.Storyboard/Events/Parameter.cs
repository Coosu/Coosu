using System;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Parameter : BasicEvent
    {
        public override EventType EventType => EventTypes.Parameter;
        public override bool IsStatic => true;

        public ParameterType Type
        {
            get => (ParameterType)(int)Start[0];
            set
            {
                Start[0] = (int)value;
                End[0] = (int)value;
            }
        }

        protected override async Task WriteExtraScriptAsync(TextWriter writer)
        {
            await writer.WriteAsync(Type.ToShortString());
        }

        public Parameter(double startTime, double endTime, ParameterType type) :
            base(EasingType.Linear.ToEasingFunction(), startTime, endTime,
                new double[] { (int)type }, new double[] { (int)type })
        {
        }

        public Parameter(double startTime, double endTime, Span<double> value) :
            base(EasingType.Linear.ToEasingFunction(), startTime, endTime, value, default)
        {
        }

        public Parameter()
        {
        }
    }
}
