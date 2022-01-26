using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Parameter : BasicEvent
    {
        public override EventType EventType => EventTypes.Parameter;

        public ParameterType Type
        {
            get => (ParameterType)(byte)GetValue(0);
            set => SetValue(0, (byte)value);
        }

        public override bool IsFilled => Values.Count == 1;
        public override bool IsHalfFilled => Values.Count == 1;

        public override float GetValue(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            return base.GetValueImpl(index);
        }

        public override void SetValue(int index, float value)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            base.SetValueImpl(index, value);
        }

        public override void Fill()
        {
            Fill(1);
        }

        public override bool IsStartsEqualsEnds() => true;

        protected override async Task WriteExtraScriptAsync(TextWriter writer)
        {
            await writer.WriteAsync(Type.ToShortString());
        }

        //public Parameter(double startTime, double endTime, ParameterType type) :
        //    base(EasingType.Linear.ToEasingFunction(), startTime, endTime,
        //        new double[] { (int)type }, new double[] { (int)type })
        //{
        //}

        public Parameter(float startTime, float endTime, List<float> values) :
            base(EasingType.Linear.ToEasingFunction(), startTime, endTime, values)
        {
        }

        public Parameter()
        {
        }
    }
}
