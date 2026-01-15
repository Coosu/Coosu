using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using osu.Framework.Utils;

namespace Coosu.Storyboard.Extensions;

[DebuggerDisplay("Expression = {DebuggerDisplay}")]
public class RelativeEvent : IKeyEvent
{
    public event Action? TimingChanged;

    private List<double> _values;
    private double _startTime;
    private double _endTime;
    private string DebuggerDisplay => this.GetHeaderString();
    public EventType EventType { get; }

    public EasingFunctionBase Easing { get; set; } = LinearEase.Instance;

    public double StartTime
    {
        get => _startTime;
        set
        {
            if (Precision.AlmostEquals(_startTime, value)) return;
            _startTime = value;
            TimingChanged?.Invoke();
        }
    }

    public double EndTime
    {
        get => _endTime;
        set
        {
            if (Precision.AlmostEquals(_startTime, value)) return;
            _endTime = value;
            TimingChanged?.Invoke();
        }
    }

    public virtual double DefaultValue => 0;

    internal List<double> TagValues { get; set; }

    public IReadOnlyList<double> Values
    {
        get => _values;
        internal set => _values = (List<double>)value;
    }


#if NET5_0_OR_GREATER
        public virtual Span<double> GetStartsSpan()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }

        public Span<double> GetEndsSpan()
        {
            throw new NotSupportedException("The relative events have no starts or ends.");
        }
#endif
    public IEnumerable<double> GetStarts()
    {
        return TagValues;
        throw new NotSupportedException("The relative events have no starts or ends.");
    }

    public IEnumerable<double> GetEnds()
    {
        return Values;
        throw new NotSupportedException("The relative events have no starts or ends.");
    }

    public void SetStarts(IEnumerable<double> startValues)
    {
        TagValues = startValues is List<double> startValueList ? startValueList : startValues.ToList();
        if (TagValues.Count != EventType.Size) throw new ArgumentException();
    }

    public void SetEnds(IEnumerable<double> endValues)
    {
        Values = endValues is List<double> endValuesList ? endValuesList : endValues.ToList();
        if (Values.Count != EventType.Size) throw new ArgumentException();
    }

    public virtual bool IsHalfFilled => false;
    public virtual bool IsFilled => Values.Count == EventType.Size;

    public virtual double GetValue(int index)
    {
        if (index >= EventType.Size)
            throw new ArgumentOutOfRangeException(nameof(index), index,
                $"Incorrect parameter index for {EventType.Flag}");
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index,
                $"Incorrect parameter index for {EventType.Flag}");

        return GetValueImpl(index);
    }

    public virtual void SetValue(int index, double value)
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
        await writer.WriteStandardizedNumberAsync(Math.Round(StartTime));
        await writer.WriteAsync(",");
        await writer.WriteStandardizedNumberAsync(Math.Round(EndTime));
        await writer.WriteAsync("] by [");
        await WriteExtraScriptAsync(writer);
        await writer.WriteAsync(']');
    }

    public virtual async Task WriteScriptAsync(TextWriter writer) =>
        await WriteHeaderAsync(writer);

    public RelativeEvent()
    {
        _values = new List<double>();
    }

    public RelativeEvent(EventType eventType)
    {
        EventType = eventType;
        _values = new List<double>();
        TagValues = new List<double>();
    }

    public RelativeEvent(EventType eventType, EasingFunctionBase easing, double startTime, double endTime, List<double> byValues)
    {
        EventType = eventType;
        Easing = easing;
        StartTime = startTime;
        EndTime = endTime;
        _values = byValues;
        TagValues = new List<double>(_values.Count);
        for (int i = 0; i < _values.Count; i++)
        {
            //TagValues.Add(double.NaN);
            TagValues.Add(0);
        }
    }

    protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
    {
        Fill();
        for (int i = 0; i < Values.Count; i++)
        {
            if (TagValues.Count > 0 && !double.IsNaN(TagValues[i]))
            {
                await textWriter.WriteStandardizedNumberAsync(TagValues[i]);
                await textWriter.WriteAsync('~');
            }

            await textWriter.WriteStandardizedNumberAsync(Values[i]);
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
                _values.Add(double.NaN);
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
                _values.Add(double.NaN);
            }
        }
    }

    protected double GetValueImpl(int index)
    {
        Fill(index + 1);
        return _values[index];
    }

    protected void SetValueImpl(int index, double value)
    {
        Fill(index + 1);
        _values[index] = value;
    }

    public object Clone()
    {
        return new RelativeEvent(EventType, Easing, StartTime, EndTime, Values.CloneAsList());
    }
}