using System;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;
using Coosu.Database.Serialization;

namespace Coosu.Database;

/// <summary>
/// Only support version after 20191106
/// <seealso href="https://github.com/ppy/osu/wiki/Legacy-database-file-structure"/>
/// </summary>
public class OsuDbReader : ReaderBase, IDisposable
{
    private static readonly MappingHelper MappingHelper;

    static OsuDbReader()
    {
        MappingHelper = new MappingHelper(typeof(OsuDb));
    }

    private readonly Stream _stream;
    private readonly BinaryReader _binaryReader;

    private IDbStructure _currentDbStructure;

    private readonly int[] _arrayCounts;
    private readonly int[] _arrayIndexes;
    private readonly int[] _memberIndexes;

    public OsuDbReader(string path) : this(File.OpenRead(path))
    {
    }

    public OsuDbReader(Stream stream)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(stream);
        _currentDbStructure = MappingHelper.Structure;

        _arrayCounts = new int[MappingHelper.LastId];
        _arrayIndexes = new int[MappingHelper.LastId];
        _memberIndexes = new int[MappingHelper.LastId];
        new Span<int>(_memberIndexes).Fill(-1);
    }

    public bool Read()
    {
        try
        {
            if (_currentDbStructure is ClassStructure classStructure)
            {
                return ReadObject(classStructure);
            }
            else if (_currentDbStructure is ArrayStructure arrayStructure)
            {
                return ReadArray(arrayStructure);
            }

            return true;
        }
        finally
        {
            //Console.WriteLine("Name = " + GetString(Name) + "; " +
            //                  "Path = " + GetString(Path) + "; " +
            //                  "Value = " + GetString(Value) + "; " +
            //                  "NodeType = " + GetString(NodeType) + "; " +
            //                  "DataType = " + GetString(DataType) + "; " +
            //                  "TargetType = " + GetString(TargetType));
        }
    }

    private bool ReadObject(ClassStructure classStructure)
    {
        var memberIndex = _memberIndexes[classStructure.NodeId];
        if (memberIndex == -1)
        {
            _memberIndexes[classStructure.NodeId] = 0;
            Name = classStructure.Name;
            Path = classStructure.Path;
            NodeType = classStructure.BaseStructure == null ? NodeType.FileStart : NodeType.ObjectStart;
            DataType = DataType.Object;
            TargetType = null;
            Value = null;
            {
                return true;
            }
        }

        if (memberIndex > classStructure.Structures.Count - 1)
        {
            Name = classStructure.Name;
            Path = classStructure.Path;
            DataType = DataType.Object;
            TargetType = null;
            Value = null;
            _memberIndexes[classStructure.NodeId] = -1;
            if (classStructure.BaseStructure == null)
            {
                return false;
            }

            NodeType = NodeType.ObjectEnd;
            _currentDbStructure = classStructure.BaseStructure;
            {
                return true;
            }
        }

        var subStructure = classStructure.Structures[memberIndex];
        if (subStructure is PropertyStructure propertyStructure)
        {
            Name = propertyStructure.Name;
            Path = propertyStructure.Path;
            NodeType = NodeType.KeyValue;
            DataType = propertyStructure.TargetDataType;
            TargetType = propertyStructure.TargetType;
            Value = propertyStructure.ValueHandler.ReadValue(_binaryReader, propertyStructure.TargetDataType);
            if (MappingHelper.NodeLengthFlags[propertyStructure.NodeId])
            {
                _arrayCounts[propertyStructure.NodeId] = Convert.ToInt32(Value);
            }
        }
        else if (subStructure is ArrayStructure arrayStructure)
        {
            Name = arrayStructure.Name;
            Path = arrayStructure.Path;
            NodeType = NodeType.ArrayStart;
            DataType = DataType.Array;
            TargetType = null;
            Value = null;
            _currentDbStructure = arrayStructure;
        }

        _memberIndexes[classStructure.NodeId] = memberIndex + 1;
        return true;
    }

    private bool ReadArray(ArrayStructure arrayStructure)
    {
        var itemIndex = _arrayIndexes[arrayStructure.NodeId];
        if (itemIndex > _arrayCounts[arrayStructure.LengthNodeId] - 1)
        {
            Name = arrayStructure.Name;
            Path = arrayStructure.Path;
            NodeType = NodeType.ArrayEnd;
            DataType = DataType.Array;
            TargetType = null;
            Value = null;
            _currentDbStructure = arrayStructure.BaseStructure;
            _arrayIndexes[arrayStructure.NodeId] = 0;
            {
                return true;
            }
        }

        if (arrayStructure.IsObjectArray)
        {
            _currentDbStructure = arrayStructure.ObjectStructure!;
            var result = Read();
            _arrayIndexes[arrayStructure.NodeId] = itemIndex + 1;
            {
                return true;
            }
        }

        var propertyStructure = arrayStructure.PropertyStructure!;
        Name = propertyStructure.Name;
        Path = propertyStructure.Path;
        NodeType = NodeType.KeyValue;
        DataType = propertyStructure.TargetDataType;
        TargetType = propertyStructure.TargetType;
        Value = propertyStructure.ValueHandler.ReadValue(_binaryReader, propertyStructure.TargetDataType);
        _arrayIndexes[arrayStructure.NodeId] = itemIndex + 1;
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

    //private static string GetString(object? obj)
    //{
    //    if (obj == null) return "NULL";
    //    return obj.ToString();
    //}
}