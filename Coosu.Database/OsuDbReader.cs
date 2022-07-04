using System;
using System.IO;
using Coosu.Database.Annotations;
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
    private readonly Stream _stream;
    private readonly BinaryReader _binaryReader;

    private IDbStructure _currentDbStructure;

    private readonly StructureHelper _structureHelper;
    private readonly int[] _arrayCounts;
    private readonly int[] _arrayIndexes;
    private readonly int[] _memberIndexes;
    private bool _hasTargetPractice = false; // Temporary hardcoded for TargetPractice

    public OsuDbReader(string path, Type? type = null) : this(File.OpenRead(path), type)
    {
    }

    public OsuDbReader(Stream stream, Type? type = null)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(stream);
        type ??= StructureHelperPool.TypeOsuDb;
        _structureHelper = StructureHelperPool.GetHelperByType(type);
        _currentDbStructure = _structureHelper.RootStructure;

        _arrayCounts = new int[_structureHelper.LastId];
        _arrayIndexes = new int[_structureHelper.LastId];
        _memberIndexes = new int[_structureHelper.LastId];
        new Span<int>(_memberIndexes).Fill(-1);
    }

    public IntDoublePair GetIntDoublePair() => (IntDoublePair)Value!;
    public TimingPoint GetTimingPoint() => (TimingPoint)Value!;
    public DateTime GetDateTime() => (DateTime)Value!;

    public bool Read()
    {
        if (_currentDbStructure is ObjectStructure objectStructure)
        {
            var val = IsEndOfStream = !ReadObject(objectStructure);
            return !val;
        }
        else if (_currentDbStructure is ArrayStructure arrayStructure)
        {
            var val = IsEndOfStream = !ReadArray(arrayStructure);
            return !val;
        }

        return true;
    }

    private bool ReadObject(ObjectStructure objectStructure)
    {
        var memberIndex = _memberIndexes[objectStructure.NodeId];
        if (memberIndex == -1)
        {
            _memberIndexes[objectStructure.NodeId] = 0;
            NodeId = objectStructure.NodeId;
            Name = objectStructure.Name;
            Path = objectStructure.Path;
            NodeType = objectStructure.BaseStructure == null ? NodeType.FileStart : NodeType.ObjectStart;
            DataType = DataType.Object;
            TargetType = null;
            Value = null;
            return true;
        }

        if (memberIndex > objectStructure.Structures.Count - 1)
        {
            NodeId = objectStructure.NodeId;
            Name = objectStructure.Name;
            Path = objectStructure.Path;
            DataType = DataType.Object;
            TargetType = null;
            Value = null;
            _memberIndexes[objectStructure.NodeId] = -1;
            if (objectStructure.BaseStructure == null)
            {
                return false;
            }

            NodeType = NodeType.ObjectEnd;
            _currentDbStructure = objectStructure.BaseStructure;
            return true;
        }

        var subStructure = objectStructure.Structures[memberIndex];
        if (subStructure is PropertyStructure propertyStructure)
        {
            var ignoreWhen = propertyStructure.IgnoreWhenAttribute;
            if (ignoreWhen != null) // Temporary hardcoded for TargetPractice
            {
                var memberName = ignoreWhen.MemberName;
                var ignoreCondition = ignoreWhen.IgnoreCondition;
                var desiredValue = ignoreWhen.Value;
                if (memberName == nameof(Score.Mods) &&
                    ignoreCondition == StructureIgnoreWhenAttribute.Condition.Contains &&
                    (Mods)desiredValue == Mods.TargetPractice)
                {
                    if (!_hasTargetPractice)
                    {
                        _memberIndexes[objectStructure.NodeId] = memberIndex + 1;
                        return Read();
                    }
                }
            }

            NodeId = propertyStructure.NodeId;
            Name = propertyStructure.Name;
            Path = propertyStructure.Path;
            NodeType = NodeType.KeyValue;
            DataType = propertyStructure.TargetDataType;
            TargetType = propertyStructure.TargetType;
            Value = propertyStructure.ValueHandler.ReadValue(_binaryReader, propertyStructure.TargetDataType);
            if (_structureHelper.NodeLengthFlags[propertyStructure.NodeId])
            {
                _arrayCounts[propertyStructure.NodeId] = Convert.ToInt32(Value);
            }

            if (NodeId == 23) // Temporary hardcoded for TargetPractice
            {
                _hasTargetPractice = ((Mods)Value & Mods.TargetPractice) != 0;
            }
        }
        else if (subStructure is ArrayStructure arrayStructure)
        {
            NodeId = arrayStructure.NodeId;
            Name = arrayStructure.Name;
            Path = arrayStructure.Path;
            NodeType = NodeType.ArrayStart;
            DataType = DataType.Array;
            TargetType = null;
            Value = null;
            _currentDbStructure = arrayStructure;
        }

        _memberIndexes[objectStructure.NodeId] = memberIndex + 1;
        return true;
    }

    private bool ReadArray(ArrayStructure arrayStructure)
    {
        var itemIndex = _arrayIndexes[arrayStructure.NodeId];
        if (itemIndex > _arrayCounts[arrayStructure.LengthNodeId] - 1)
        {
            NodeId = arrayStructure.NodeId;
            Name = arrayStructure.Name;
            Path = arrayStructure.Path;
            NodeType = NodeType.ArrayEnd;
            DataType = DataType.Array;
            TargetType = null;
            Value = null;
            _currentDbStructure = arrayStructure.BaseStructure;
            _arrayIndexes[arrayStructure.NodeId] = 0;
            return true;
        }

        if (arrayStructure.IsObjectArray)
        {
            _currentDbStructure = arrayStructure.ObjectStructure!;
            if (!Read()) return false;

            _arrayIndexes[arrayStructure.NodeId] = itemIndex + 1;
            return true;
        }

        var propertyStructure = arrayStructure.PropertyStructure!;
        NodeId = propertyStructure.NodeId;
        Name = propertyStructure.Name;
        Path = propertyStructure.Path;
        NodeType = NodeType.KeyValue;
        DataType = propertyStructure.TargetDataType;
        TargetType = propertyStructure.TargetType;
        Value = propertyStructure.ValueHandler.ReadValue(_binaryReader, propertyStructure.TargetDataType);
        _arrayIndexes[arrayStructure.NodeId] = itemIndex + 1;
        return true;
    }

    public void Dispose()
    {
        _stream.Dispose();
        _binaryReader.Dispose();
    }
}