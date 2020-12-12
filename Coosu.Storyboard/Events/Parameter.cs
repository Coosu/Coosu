using System;
using System.Globalization;

namespace Coosu.Storyboard.Events
{
    public sealed class Parameter : CommonEvent
    {
        public override EventType EventType => EventTypes.Parameter;

        public override int ParamLength => 1;
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

        protected override string Script => Type.ToShortString();

        public Parameter(EasingType easing, float startTime, float endTime, ParameterType type) :
            base(easing, startTime, endTime, new float[] { (int)type }, new float[] { (int)type })
        {
            Easing = EasingType.Linear;
        }
    }
}
