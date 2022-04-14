using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Database.Annotations;
using Coosu.Database.Converting;
using Coosu.Database.Internal;

namespace Coosu.Database;

public sealed class MappingHelper
{
    private readonly Dictionary<Type, IValueHandler> _sharedHandlers = new();
    private readonly HashSet<int> _lengthNodeIds = new();

    public MappingHelper(Type type)
    {
        Structure = GetClassStructure(type, null, type.Name, type.Name);
        NodeLengthFlags = new bool[LastId];
        foreach (var lengthNodeId in _lengthNodeIds)
        {
            NodeLengthFlags[lengthNodeId] = true;
        }
    }

    public IDbStructure Structure { get; }
    public int LastId { get; private set; }

    internal bool[] NodeLengthFlags { get; }

    private ClassStructure GetClassStructure(Type type, IDbStructure? baseStructure, string className, string classPath)
    {
        var propertyMapping = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(k => !k.Name.Equals("EqualityContract", StringComparison.Ordinal))
            .ToDictionary(k => k.Name, k => k);

        var classStructure = new ClassStructure(GetNextNodeId(), className, classPath, baseStructure);

        foreach (var kvp in propertyMapping)
        {
            var propertyInfo = kvp.Value;

            var attribute = propertyInfo.GetCustomAttribute<StructureIgnoreAttribute>();
            if (attribute != null) continue;
            var arrAttr = propertyInfo.GetCustomAttribute<StructureArrayAttribute>();

            var propertyName = propertyInfo.Name;
            var propertyPath = classPath + "." + propertyName;

            if (arrAttr == null)
            {
                var nodeId = GetNextNodeId();
                var forceStructure = propertyInfo.GetCustomAttribute<StructureTypeAttribute>();
                classStructure.Structures.Add(
                    new PropertyStructure(nodeId, propertyName, propertyPath, classStructure,
                        propertyInfo.PropertyType,
                        ConvertType(forceStructure, propertyInfo.PropertyType),
                        DefaultValueHandler.Instance)
                );
                classStructure.MemberNameIdMapping.Add(propertyName, nodeId);
                continue;
            }

            var lengthNodeId = classStructure.MemberNameIdMapping[arrAttr.LengthMemberName];
            _lengthNodeIds.Add(lengthNodeId);
            var arrayStructure = new ArrayStructure(
                GetNextNodeId(), propertyName + "[]", propertyPath + "[]", classStructure,
                arrAttr.ItemType, arrAttr.SubDataType, arrAttr.SubDataType is DataType.Object, lengthNodeId);

            if (arrayStructure.IsObjectArray)
            {
                arrayStructure.ObjectStructure = GetClassStructure(arrAttr.ItemType, arrayStructure,
                    propertyName, propertyPath);
            }
            else
            {
                DataType arrAttrSubDataType;
                if (arrAttr.SubDataType != DataType.Unknown)
                {
                    arrAttrSubDataType = arrAttr.SubDataType;
                }
                else
                {
                    arrAttrSubDataType = ConvertType(propertyInfo.GetCustomAttribute<StructureTypeAttribute>(),
                        arrAttr.ItemType);
                }

                arrayStructure.PropertyStructure =
                    new PropertyStructure(GetNextNodeId(), propertyName, propertyPath, classStructure,
                        arrAttr.ItemType,
                        arrAttrSubDataType,
                        GetSharedValueHandler(arrAttr.ValueHandler) ?? DefaultValueHandler.Instance
                    );
            }

            classStructure.Structures.Add(arrayStructure);
        }

        return classStructure;
    }

    private int GetNextNodeId()
    {
        return LastId++;
    }

    private static DataType ConvertType(StructureTypeAttribute? attribute, Type targetType)
    {
        if (attribute != null)
            return attribute.DataType;
        if (targetType == StaticTypes.Boolean)
            return DataType.Boolean;
        if (targetType == StaticTypes.Byte)
            return DataType.Byte;
        if (targetType == StaticTypes.Int16)
            return DataType.Int16;
        if (targetType == StaticTypes.Int32)
            return DataType.Int32;
        if (targetType == StaticTypes.Int64)
            return DataType.Int64;
        if (targetType == StaticTypes.Single)
            return DataType.Single;
        if (targetType == StaticTypes.Double)
            return DataType.Double;
        if (targetType == StaticTypes.String)
            return DataType.String;
        if (targetType == StaticTypes.DateTime)
            return DataType.DateTime;
        if (targetType == StaticTypes.IntDoublePair)
            return DataType.Double;
        if (targetType == StaticTypes.TimingPoint)
            return DataType.String;

        if (targetType.IsEnum)
        {
            var type = Enum.GetUnderlyingType(targetType);
            return ConvertType(null, type);
        }

        throw new NotSupportedException("Type supported: " + targetType);
    }

    private IValueHandler? GetSharedValueHandler(Type? type)
    {
        if (type == null) return null;
        if (_sharedHandlers.TryGetValue(type, out var value))
        {
            return value;
        }

        value = (IValueHandler)Activator.CreateInstance(type);
        _sharedHandlers.Add(type, value);
        return value;
    }
}