using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions
{
    [DebuggerDisplay("Expression = {DebuggerDisplay}")]
    public class RelativeEvent : IKeyEvent
    {
        private List<float> _values;
        private string DebuggerDisplay => this.GetHeaderString();
        public EventType EventType { get; }

        public EasingFunctionBase Easing { get; set; } = LinearEase.Instance;

        public float StartTime { get; set; }
        public float EndTime { get; set; }

        public virtual float DefaultValue => 0;

        internal List<float> TagValues { get; set; }

        public IReadOnlyList<float> Values
        {
            get => _values;
            internal set => _values = (List<float>)value;
        }


#if NET5_0_OR_GREATER
        public virtual Span<float> GetStartsSpan()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }

        public Span<float> GetEndsSpan()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }
#endif
        public IEnumerable<float> GetStarts()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }

        public IEnumerable<float> GetEnds()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }

        public void SetStarts(IEnumerable<float> startValues)
        {
            TagValues = startValues is List<float> startValueList ? startValueList : startValues.ToList();
            if (TagValues.Count != EventType.Size) throw new ArgumentException();
        }

        public void SetEnds(IEnumerable<float> endValues)
        {
            Values = endValues is List<float> endValuesList ? endValuesList : endValues.ToList();
            if (Values.Count != EventType.Size) throw new ArgumentException();
        }

        public virtual bool IsHalfFilled => false;
        public virtual bool IsFilled => Values.Count == EventType.Size;

        public virtual float GetValue(int index)
        {
            if (index >= EventType.Size)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");

            return GetValueImpl(index);
        }

        public virtual void SetValue(int index, float value)
        {
            if (index >= EventType.Size)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");

            SetValueImpl(index, value);
        }

        public virtual void Fill()
        {
            Fill(EventType.Size);
        }

        public virtual bool IsStartsEqualsEnds()
        {
            Fill();

            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values[i];
                if (value != 0) return false;
            }

            return true;
        }

        public void AdjustTiming(float offset)
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

        public RelativeEvent()
        {
            _values = new List<float>();
        }

        public RelativeEvent(EventType eventType)
        {
            EventType = eventType;
            _values = new List<float>();
            TagValues = new List<float>();
        }

        public RelativeEvent(EventType eventType, EasingFunctionBase easing, float startTime, float endTime, List<float> byValues)
        {
            EventType = eventType;
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            _values = byValues;
            TagValues = new List<float>(_values.Count);
        }

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            Fill();
            for (int i = 0; i < Values.Count; i++)
            {
                if (!float.IsNaN(TagValues[i]))
                {
                    await textWriter.WriteAsync(TagValues[i].ToIcString());
                    await textWriter.WriteAsync('~');
                }

                await textWriter.WriteAsync(Values[i].ToIcString());
                if (i != EventType.Size - 1) await textWriter.WriteAsync(',');
            }
        }

        protected void Fill(int count)
        {
            if (count <= _values.Count) return;
            var index = count - 1;
            var size = EventType.Size;
            if (index < size)
            {
                _values.Capacity = size;
                TagValues.Capacity = size;
                while (_values.Count < size)
                {
                    _values.Add(DefaultValue);
                }

                while (TagValues.Count < size)
                {
                    _values.Add(float.NaN);
                }
            }
            else
            {
                while (index > _values.Count - 1)
                {
                    _values.Add(DefaultValue);
                }

                while (index > TagValues.Count - 1)
                {
                    _values.Add(float.NaN);
                }
            }
        }

        protected float GetValueImpl(int index)
        {
            Fill(index + 1);
            return _values[index];
        }

        protected void SetValueImpl(int index, float value)
        {
            Fill(index + 1);
            _values[index] = value;
        }

        public object Clone()
        {
            return new RelativeEvent(EventType, Easing, StartTime, EndTime, Values.CloneAsList());
        }
    }
}