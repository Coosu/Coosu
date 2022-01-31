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
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    [DebuggerDisplay("Expression = {DebuggerDisplay}")]
    public abstract class BasicEvent : IKeyEvent
    {
        public event Action? TimingChanged;
        internal List<float> _values;
        private float _startTime;
        private float _endTime;
        private string DebuggerDisplay => this.GetHeaderString();

        public abstract EventType EventType { get; }
        public EasingFunctionBase Easing { get; set; } = LinearEase.Instance;

        public float StartTime
        {
            get => _startTime;
            set
            {
                if (Precision.AlmostEquals(_startTime, value)) return;
                _startTime = value;
                TimingChanged?.Invoke();
            }
        }

        public float EndTime
        {
            get => _endTime;
            set
            {
                if (Precision.AlmostEquals(_startTime, value)) return;
                _endTime = value;
                TimingChanged?.Invoke();
            }
        }

        public virtual float DefaultValue => 0;
        public IReadOnlyList<float> Values
        {
            get => _values;
            internal set => _values = (List<float>)value;
        }

#if NET5_0_OR_GREATER
        public Span<float> GetStartsSpan()
        {
            Fill();
            var size = EventType.Size;
            var span = System.Runtime.InteropServices.CollectionsMarshal
                .AsSpan(_values)
                .Slice(0, size);
            return span;
        }

        public Span<float> GetEndsSpan()
        {
            Fill();
            var size = EventType.Size;
            var span = System.Runtime.InteropServices.CollectionsMarshal
                .AsSpan(_values)
                .Slice(size, size);
            return span;
        }
#endif
        public IEnumerable<float> GetStarts()
        {
            Fill();
            var size = EventType.Size;
            var span = _values.Take(size);
            return span;
        }

        public IEnumerable<float> GetEnds()
        {
            Fill();
            var size = EventType.Size;
            var span = _values.Skip(size).Take(size);
            return span;
        }

        public virtual bool IsHalfFilled => Values.Count == EventType.Size;
        public virtual bool IsFilled => Values.Count == EventType.Size * 2;

        public virtual float GetValue(int index)
        {
            if (index >= EventType.Size * 2)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");

            return GetValueImpl(index);
        }

        public virtual void SetValue(int index, float value)
        {
            if (index >= EventType.Size * 2)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Incorrect parameter index for {EventType.Flag}");

            SetValueImpl(index, value);
        }

        public void SetStarts(IEnumerable<float> startValues)
        {
            if (startValues is IReadOnlyList<float> l)
            {
                for (var i = 0; i < EventType.Size; i++)
                {
                    var f = l[i];
                    _values[i] = f;
                }
            }
            else
            {
                int i = 0;
                foreach (var startValue in startValues)
                {
                    _values[i] = startValue;
                    i++;
                    if (i >= EventType.Size) break;
                }
            }
        }

        public void SetEnds(IEnumerable<float> endValues)
        {
            if (endValues is IReadOnlyList<float> l)
            {
                for (var i = 0; i < EventType.Size; i++)
                {
                    var f = l[i];
                    _values[i + EventType.Size] = f;
                }
            }
            else
            {
                int i = EventType.Size;
                foreach (var startValue in endValues)
                {
                    _values[i] = startValue;
                    i++;
                    if (i >= EventType.Size * 2) break;
                }
            }
        }

        public virtual void Fill()
        {
            Fill(EventType.Size * 2);
        }

        public virtual bool IsStartsEqualsEnds()
        {
            if (IsHalfFilled) return true;
            Fill();
#if NET5_0_OR_GREATER
            return GetEndsSpan().SequenceEqual(GetStartsSpan());
#else
            var size = EventType.Size;
            for (var i = 0; i < Values.Count / 2; i++)
            {
                var d0 = Values[i];
                var d1 = Values[i + size];
                if (!Precision.AlmostEquals(d0, d1))
                    return false;
            }

            return true;
#endif
        }

        public void AdjustTiming(float offset)
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

        protected BasicEvent()
        {
            _values = new List<float>();
        }

        protected BasicEvent(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
        {
            Easing = easing;
            _startTime = startTime;
            _endTime = endTime;
            _values = values;
        }

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            var sequenceEqual = IsStartsEqualsEnds();
            if (sequenceEqual)
                await WriteStartAsync(textWriter);
            else
                await WriteFullAsync(textWriter);
        }

        protected void Fill(int count)
        {
            if (count <= _values.Count) return;
            var index = count - 1;
            var size = EventType.Size;
            if (IsHalfFilled && !IsFilled)
            {
                _values.Capacity = size * 2;
                for (int i = 0; i < size; i++)
                {
                    _values.Add(_values[i]);
                }
            }
            else if (index < size)
            {
                _values.Capacity = size;
                while (_values.Count < size)
                {
                    _values.Add(DefaultValue);
                }
            }
            else
            {
                while (index > _values.Count - 1)
                {
                    _values.Add(DefaultValue);
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

        private async Task WriteStartAsync(TextWriter textWriter)
        {
            for (int i = 0; i < EventType.Size; i++)
            {
                await textWriter.WriteAsync(Values[i].ToIcString());
                if (i != EventType.Size - 1) await textWriter.WriteAsync(',');
            }
        }

        private async Task WriteFullAsync(TextWriter textWriter)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                await textWriter.WriteAsync(Values[i].ToIcString());
                if (i != Values.Count - 1) await textWriter.WriteAsync(',');
            }
        }

        public static IKeyEvent Create(EventType e, EasingFunctionBase easing,
            float startTime, float endTime, Span<float> startValues, Span<float> endValues)
        {
            var list = new List<float>();
            foreach (var startValue in startValues) list.Add(startValue);
            foreach (var endValue in endValues) list.Add(endValue);

            return Create(e, easing, startTime, endTime, list);
        }

        public static IKeyEvent Create(EventType e, EasingFunctionBase easing,
            float startTime, float endTime, IEnumerable<float> startValues, IEnumerable<float> endValues)
        {
            var list = new List<float>();
            list.AddRange(startValues);
            list.AddRange(endValues);

            return Create(e, easing, startTime, endTime, list);
        }

        public static IKeyEvent Create(EventType e, EasingFunctionBase easing,
            float startTime, float endTime, List<float> values)
        {
            var size = e.Size;
            if (size != 0 && values.Count != size && values.Count != size * 2)
                throw new ArgumentException($"Incorrect parameter length for {e.Flag}: {values.Count}");

            IKeyEvent keyEvent;
            if (e.Flag == EventTypes.Fade.Flag)
                keyEvent = new Fade(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Move.Flag)
                keyEvent = new Move(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.MoveX.Flag)
                keyEvent = new MoveX(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.MoveY.Flag)
                keyEvent = new MoveY(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Scale.Flag)
                keyEvent = new Scale(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Vector.Flag)
                keyEvent = new Vector(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Rotate.Flag)
                keyEvent = new Rotate(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Color.Flag)
                keyEvent = new Color(easing, startTime, endTime, values);
            else if (e.Flag == EventTypes.Parameter.Flag)
                keyEvent = new Parameter(startTime, endTime, values);
            else
            {
                var result = HandlerRegister.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, values);
                keyEvent = result ?? throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            //keyEvent.Fill();
            return keyEvent;
        }

        public object Clone()
        {
            return BasicEvent.Create(EventType, Easing, StartTime, EndTime, Values.CloneAsList());
        }
    }
}
