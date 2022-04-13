using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;
using Coosu.Database.Utils;
using B = Coosu.Database.DataTypes.Beatmap;

namespace Coosu.Database;

/// <summary>
/// Only support version after 20191106
/// <seealso href="https://github.com/ppy/osu/wiki/Legacy-database-file-structure"/>
/// </summary>
public class OsuDbReader : ReaderBase, IDisposable
{
    private readonly Stream _stream;
    private readonly BinaryReader _binaryReader;

    private bool _documentStart;
    private bool _objectStart;

    private int _innerIndex = -1;
    private int _innerCount;

    private int _beatmapIndex;
    private int _beatmapCount;

    private int _generalItemIndex = -1;
    private int _beatmapItemIndex = -1;

    private readonly Stack<Action> _arrayHandlers = new();
    private readonly Stack<string> _arrays = new();

    public OsuDbReader(string path) : this(File.OpenRead(path))
    {
    }

    public OsuDbReader(Stream stream)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(stream);
    }

    public bool Read()
    {
        if (_generalItemIndex >= OsuDbReaderMapping.GeneralSequence.Length - 1)
        {
            if (NodeType == NodeType.FileEnd) return false;
            Name = null;
            Value = null;
            DataType = DataType.Object;
            NodeType = NodeType.FileEnd;
            return true;
        }

        if (!_documentStart && _generalItemIndex == -1)
        {
            NodeType = NodeType.FileStart;
            _documentStart = true;
            return true;
        }

        if (_arrayHandlers.Count == 0)
        {
            _generalItemIndex++;
            Name = OsuDbReaderMapping.GeneralSequence[_generalItemIndex];
            DataType = OsuDbReaderMapping.GeneralSequenceType[_generalItemIndex];
            if (DataType != DataType.Array)
            {
                NodeType = NodeType.KeyValue;
                Value = ReadValueByType(DataType);

                if (_generalItemIndex is 5) // BeatmapCount
                {
                    _beatmapCount = (int)Value;
                }
            }
            else
            {
                NodeType = NodeType.ArrayStart;
                Value = null;
                _arrayHandlers.Push(ReadBeatmap);
                _arrays.Push(Name);
            }
        }
        else
        {
            _arrayHandlers.Peek().Invoke();
        }

        return true;
    }

    public IntDoublePair GetIntDoublePair() => (IntDoublePair)Value!;
    public TimingPoint GetTimingPoint() => (TimingPoint)Value!;
    public DateTime GetDateTime() => (DateTime)Value!;

    public void Dispose()
    {
        _stream.Dispose();
        _binaryReader.Dispose();
    }

    private void ReadBeatmap()
    {
        if (_beatmapItemIndex >= OsuDbReaderMapping.BeatmapSequence.Length - 1)
        {
            if (_beatmapIndex == _beatmapCount - 1 && NodeType != NodeType.ObjectEnd)
            {
                EndObject();
                return;
            }

            if (ValidateEndArray(_beatmapCount, ref _beatmapIndex))
            {
                return;
            }

            _beatmapIndex++;
            _beatmapItemIndex = -1;
            EndObject();
            return;
        }

        if (!_objectStart && _beatmapItemIndex == -1)
        {
            NodeType = NodeType.ObjectStart;
            Name = _arrays.Peek();
            Value = null;
            DataType = DataType.Object;
            _objectStart = true;
            return;
        }

        _beatmapItemIndex++;

        Name = OsuDbReaderMapping.BeatmapSequence[_beatmapItemIndex];
        DataType = OsuDbReaderMapping.BeatmapSequenceType[_beatmapItemIndex];
        if (DataType != DataType.Array)
        {
            NodeType = NodeType.KeyValue;
            Value = ReadValueByType(DataType);
        }
        else
        {
            NodeType = NodeType.ArrayStart;
            if (_beatmapItemIndex is 20 or 22 or 24 or 26)
            {
                _innerCount = (int)Value!;

                _arrayHandlers.Push(ReadStarRatings);
                _arrays.Push(Name);
            }
            else if (_beatmapItemIndex is 31)
            {
                _innerCount = (int)Value!;

                _arrayHandlers.Push(ReadTimingPoints);
                _arrays.Push(Name);
            }

            Value = null;
        }
    }

    private void EndObject()
    {
        _objectStart = false;
        Name = _arrays.Peek();
        Value = null;
        DataType = DataType.Object;
        NodeType = NodeType.ObjectEnd;
    }

    private void ReadStarRatings()
    {
        if (ValidateEndArray(_innerCount, ref _innerIndex)) return;
        _innerIndex++;

        Name = _arrays.Peek();
        NodeType = NodeType.KeyValue;
        DataType = DataType.IntDoublePair;
        Value = ReadValueByType(DataType);
    }

    private void ReadTimingPoints()
    {
        if (ValidateEndArray(_innerCount, ref _innerIndex)) return;
        _innerIndex++;

        Name = _arrays.Peek();
        NodeType = NodeType.KeyValue;
        DataType = DataType.TimingPoint;
        Value = ReadValueByType(DataType);
    }

    private bool ValidateEndArray(int count, ref int index)
    {
        if (index < count - 1) return false;
        index = -1;
        _arrayHandlers.Pop();
        Name = _arrays.Pop();
        Value = null;
        DataType = DataType.Array;
        NodeType = NodeType.ArrayEnd;
        return true;
    }

    private object ReadValueByType(DataType dataType)
    {
        return dataType switch
        {
            DataType.Byte => _binaryReader.ReadByte(),
            DataType.Int16 => _binaryReader.ReadInt16(),
            DataType.Int32 => _binaryReader.ReadInt32(),
            DataType.Int64 => _binaryReader.ReadInt64(),
            DataType.ULEB128 => _stream.ReadLEB128Unsigned(),
            DataType.Single => _binaryReader.ReadSingle(),
            DataType.Double => _binaryReader.ReadDouble(),
            DataType.Boolean => _binaryReader.ReadBoolean(),
            DataType.String => _binaryReader.ReadStringA(),
            DataType.IntDoublePair => _binaryReader.ReadIntDoublePairA(),
            DataType.TimingPoint => _binaryReader.ReadTimingPointA(),
            DataType.DateTime => _binaryReader.ReadDateTimeA(),
            _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
        };
    }
}