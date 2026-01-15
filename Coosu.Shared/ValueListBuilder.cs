using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Coosu.Shared;

public ref struct ValueListBuilder<T>
{
    private Span<T> _span;
    private T[]? _arrayFromPool;
    private int _pos;

    public ValueListBuilder(Span<T> initialSpan)
    {
        _span = initialSpan;
        _arrayFromPool = null;
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _span.Length);
            _pos = value;
        }
    }

    public ref T this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _span[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T item)
    {
        int pos = _pos;
        if (pos >= _span.Length)
            Grow();

        _span[pos] = item;
        _pos = pos + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlySpan<T> source)
    {
        int pos = _pos;
        if (pos + source.Length > _span.Length)
            Grow(pos + source.Length);

        source.CopyTo(_span.Slice(pos));
        _pos = pos + source.Length;
    }

    public ReadOnlySpan<T> AsSpan()
    {
        return _span.Slice(0, _pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn != null)
        {
            _arrayFromPool = null;
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    private void Grow(int minCapacity = 0)
    {
        int newCapacity = _span.Length * 2;
        if (newCapacity < minCapacity) newCapacity = minCapacity;

        T[] array = ArrayPool<T>.Shared.Rent(newCapacity);

        bool success = _span.TryCopyTo(array);
        Debug.Assert(success);

        T[]? toReturn = _arrayFromPool;
        _span = _arrayFromPool = array;
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }
}