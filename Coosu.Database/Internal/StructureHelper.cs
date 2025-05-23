﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Database.Annotations;
using Coosu.Database.Converting;

namespace Coosu.Database.Internal;

internal sealed class StructureHelper
{
    private readonly Dictionary<Type, IValueHandler> _sharedHandlers = new();
    private readonly HashSet<int> _lengthNodeIds = new();

    public StructureHelper(Type type)
    {
        RootStructure = GetClassStructure(type, null, type.Name, type.Name);
        NodeLengthFlags = new bool[LastId];
        foreach (var lengthNodeId in _lengthNodeIds)
        {
            NodeLengthFlags[lengthNodeId] = true;
        }
    }

    public IDbStructure RootStructure { get; }
    public int LastId { get; private set; }
    internal Dictionary<string, int> PreservableNodeIds { get; } = new();
    internal bool[] NodeLengthFlags { get; }

    private ObjectStructure GetClassStructure(Type type, IDbStructure? baseStructure, string className, string classPath)
    {
        var propertyMapping = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(k => !k.Name.Equals("EqualityContract", StringComparison.Ordinal))
            .ToDictionary(k => k.Name, k => k);

        var objectStructure = new ObjectStructure(GetNextNodeId(), className, classPath, baseStructure);
        var ignoreWhenList = new HashSet<string>();
        foreach (var kvp in propertyMapping)
        {
            var propertyInfo = kvp.Value;

            var ignoreAttr = propertyInfo.GetCustomAttribute<StructureIgnoreAttribute>();
            if (ignoreAttr != null) continue;
            var arrAttr = propertyInfo.GetCustomAttribute<StructureArrayAttribute>();
            var ignoreWhenAttr = propertyInfo.GetCustomAttribute<StructureIgnoreWhenAttribute>();

            var propertyName = propertyInfo.Name;
            var propertyPath = classPath + "." + propertyName;

            if (arrAttr == null)
            {
                var nodeId = GetNextNodeId();
                var forceStructure = propertyInfo.GetCustomAttribute<StructureTypeAttribute>();
                var propertyStructure = new PropertyStructure(nodeId, propertyName, propertyPath, objectStructure,
                    propertyInfo.PropertyType,
                    ConvertType(forceStructure, propertyInfo.PropertyType),
                    DefaultValueHandler.Instance,
                    ignoreWhenAttr);
                objectStructure.Structures.Add(propertyStructure);
                objectStructure.MemberNameIdMapping.Add(propertyName, nodeId);
                if (ignoreWhenAttr != null)
                {
                    ignoreWhenList.Add(ignoreWhenAttr.MemberName);
                }

                continue;
            }

            var lengthNodeId = objectStructure.MemberNameIdMapping[arrAttr.LengthMemberName];
            _lengthNodeIds.Add(lengthNodeId);
            var arrayStructure = new ArrayStructure(
                GetNextNodeId(), propertyName + "[]", propertyPath + "[]", objectStructure,
                arrAttr.ItemType, arrAttr.SubDataType, arrAttr.SubDataType is DataType.Object, lengthNodeId);

            if (arrayStructure.IsObjectArray)
            {
                arrayStructure.ObjectStructure = GetClassStructure(arrAttr.ItemType, arrayStructure,
                    propertyName + "[].Item", propertyPath + "[].Item");
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
                    arrayStructure.SubDataType = arrAttrSubDataType;
                }

                var nodeId = GetNextNodeId();
                arrayStructure.PropertyStructure =
                    new PropertyStructure(nodeId, propertyName + "[].Item",
                        propertyPath + "[].Item", objectStructure,
                        arrAttr.ItemType,
                        arrAttrSubDataType,
                        GetSharedValueHandler(arrAttr.ValueHandler) ?? DefaultValueHandler.Instance,
                        ignoreWhenAttr
                    );
                if (ignoreWhenAttr != null)
                {
                    ignoreWhenList.Add(ignoreWhenAttr.MemberName);
                }
            }

            objectStructure.Structures.Add(arrayStructure);
        }

        foreach (var structure in objectStructure.Structures)
        {
            if (ignoreWhenList.Contains(structure.Name))
            {
                PreservableNodeIds.Add(structure.Name, structure.NodeId);
            }
        }

        foreach (var structure in objectStructure.Structures)
        {
            if (structure is PropertyStructure { IgnoreWhenAttribute: { } } ps)
            {
                ps.IgnoreWhenAttribute.NodeId = PreservableNodeIds[ps.IgnoreWhenAttribute.MemberName];
            }
        }

        return objectStructure;
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
        if (targetType == StaticTypes.IntSinglePair)
            return DataType.Single;
        if (targetType == StaticTypes.IntDoublePair)
            return DataType.Double;
        if (targetType == StaticTypes.TimingPoint)
            return DataType.String;

        if (targetType.IsEnum)
        {
            var type = Enum.GetUnderlyingType(targetType);
            return ConvertType(null, type);
        }

        throw new NotSupportedException("Type not supported: " + targetType);
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