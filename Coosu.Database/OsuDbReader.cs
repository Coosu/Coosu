using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;
using Coosu.Database.Mapping;
using Coosu.Database.Serialization;
using Coosu.Database.Utils;

namespace Coosu.Database;

/// <summary>
/// Only support version after 20191106
/// <seealso href="https://github.com/ppy/osu/wiki/Legacy-database-file-structure"/>
/// </summary>
public class OsuDbReader : ReaderBase, IDisposable
{
    private readonly Stream _stream;
    private readonly BinaryReader _binaryReader;
    private readonly MappingHelper _mappingHelper;

    private IMapping _currentMapping;

    public OsuDbReader(string path, MappingHelper? mappingHelper = null) : this(File.OpenRead(path), mappingHelper)
    {
    }

    public OsuDbReader(Stream stream, MappingHelper? mappingHelper = null)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(stream);
        _mappingHelper = mappingHelper ?? new MappingHelper(typeof(OsuDb));
        _currentMapping = _mappingHelper.Mapping;
    }

    public bool Read()
    {
        try
        {
            var c = _currentMapping;
            if (_currentMapping is ClassMapping classMapping)
            {
                var memberIndex = classMapping.CurrentMemberIndex;
                if (memberIndex == -1)
                {
                    classMapping.CurrentMemberIndex++;
                    Name = classMapping.Name;
                    Path = classMapping.Path;
                    NodeType = classMapping.BaseMapping == null ? NodeType.FileStart : NodeType.ObjectStart;
                    DataType = DataType.Object;
                    TargetType = null;
                    Value = null;
                    return true;
                }

                if (memberIndex > classMapping.Mapping.Count - 1)
                {
                    Name = classMapping.Name;
                    Path = classMapping.Path;
                    DataType = DataType.Object;
                    TargetType = null;
                    Value = null;
                    classMapping.CurrentMemberIndex = -1;
                    if (classMapping.BaseMapping != null)
                    {
                        NodeType = NodeType.ObjectEnd;
                        _currentMapping = classMapping.BaseMapping;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                var subMapping = classMapping.Mapping[memberIndex];
                if (subMapping is PropertyMapping propertyMapping)
                {
                    Name = propertyMapping.Name;
                    Path = propertyMapping.Path;
                    NodeType = NodeType.KeyValue;
                    DataType = propertyMapping.TargetDataType;
                    TargetType = propertyMapping.TargetType;
                    Value = propertyMapping.ValueHandler.ReadValue(_binaryReader, propertyMapping.TargetDataType);
                    if (_mappingHelper.ArrayCountStorage.ContainsKey(propertyMapping.Path))
                    {
                        _mappingHelper.ArrayCountStorage[propertyMapping.Path] = Convert.ToInt32(Value);
                    }
                }
                else if (subMapping is ArrayMapping arrayMapping)
                {
                    arrayMapping.Length = _mappingHelper.ArrayCountStorage[arrayMapping.LengthDeclarationMember];
                    Name = arrayMapping.Name;
                    Path = arrayMapping.Path;
                    NodeType = NodeType.ArrayStart;
                    DataType = DataType.Array;
                    TargetType = null;
                    Value = null;
                    _currentMapping = arrayMapping;
                }

                classMapping.CurrentMemberIndex++;
            }
            else if (_currentMapping is ArrayMapping arrayMapping)
            {
                var itemIndex = arrayMapping.CurrentItemIndex;
                if (itemIndex > arrayMapping.Length - 1)
                {
                    Name = arrayMapping.Name;
                    Path = arrayMapping.Path;
                    NodeType = NodeType.ArrayEnd;
                    DataType = DataType.Array;
                    TargetType = null;
                    Value = null;
                    _currentMapping = arrayMapping.BaseMapping;
                    arrayMapping.CurrentItemIndex = 0;
                    return true;
                }

                if (arrayMapping.IsObjectArray)
                {
                    _currentMapping = arrayMapping.ClassMapping!;
                    var result = Read();
                    arrayMapping.CurrentItemIndex++;
                    return result;
                }

                var propertyMapping = arrayMapping.PropertyMapping!;
                Name = propertyMapping.Name;
                Path = propertyMapping.Path;
                NodeType = NodeType.KeyValue;
                DataType = propertyMapping.TargetDataType;
                TargetType = propertyMapping.TargetType;
                Value = propertyMapping.ValueHandler.ReadValue(_binaryReader, propertyMapping.TargetDataType);
                arrayMapping.CurrentItemIndex++;
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

    public IntDoublePair GetIntDoublePair() => (IntDoublePair)Value!;
    public TimingPoint GetTimingPoint() => (TimingPoint)Value!;
    public DateTime GetDateTime() => (DateTime)Value!;

    public void Dispose()
    {
        _stream.Dispose();
        _binaryReader.Dispose();
    }

    private static string GetString(object? obj)
    {
        if (obj == null) return "NULL";
        return obj.ToString();
    }
}