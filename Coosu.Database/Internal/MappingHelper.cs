using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Database.Mapping;
using Coosu.Database.Mapping.Converting;

namespace Coosu.Database.Internal;

public sealed class MappingHelper
{
    private readonly Dictionary<string, int> _arrayCountStorage = new();
    private readonly Dictionary<Type, IValueHandler> _sharedHandlers = new();

    public MappingHelper(Type type)
    {
        Mapping = GetClassMapping(type, null, type.Name, type.Name);
    }

    internal Dictionary<string, int> ArrayCountStorage => _arrayCountStorage;
    internal ClassMapping Mapping { get; }

    private ClassMapping GetClassMapping(Type type, IMapping? baseMapping, string className, string classPath)
    {
        var propertyMapping = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(k => !k.Name.Equals("EqualityContract", StringComparison.Ordinal))
            .ToDictionary(k => k.Name, k => k);
        var classMapping = new ClassMapping
        {
            BaseMapping = baseMapping,
            Name = className,
            Path = classPath
        };

        foreach (var kvp in propertyMapping)
        {
            var propertyInfo = kvp.Value;

            var attribute = propertyInfo.GetCustomAttribute<OsuDbIgnoreAttribute>();
            if (attribute != null) continue;
            var arrAttr = propertyInfo.GetCustomAttribute<OsuDbArrayAttribute>();

            var propertyName = propertyInfo.Name;
            var propertyPath = classPath + "." + propertyName;

            if (arrAttr == null)
            {
                classMapping.Mapping.Add(new PropertyMapping
                {
                    BaseMapping = classMapping,
                    TargetType = propertyInfo.PropertyType,
                    TargetDataType = ConvertType(propertyInfo.GetCustomAttribute<OsuDbTypeAttribute>(),
                        propertyInfo.PropertyType),
                    ValueHandler = DefaultValueHandler.Instance,
                    Name = propertyName,
                    Path = propertyPath
                });
            }
            else
            {
                var arrLenName = classPath + "." + propertyMapping[arrAttr.LengthDeclaration].Name;
                _arrayCountStorage.Add(arrLenName, -1);
                var arrayMapping = new ArrayMapping
                {
                    SubItemType = arrAttr.SubItemType,
                    SubDataType = arrAttr.SubDataType,
                    BaseMapping = classMapping,
                    LengthDeclarationMember = arrLenName,
                    IsObjectArray = arrAttr.SubDataType is DataType.Object,
                    Name = propertyName + "[]",
                    Path = propertyPath + "[]"
                };

                if (arrayMapping.IsObjectArray)
                {
                    arrayMapping.ClassMapping = GetClassMapping(arrAttr.SubItemType, arrayMapping,
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
                        arrAttrSubDataType = ConvertType(propertyInfo.GetCustomAttribute<OsuDbTypeAttribute>(),
                            arrAttr.SubItemType);
                    }

                    arrayMapping.PropertyMapping = new PropertyMapping
                    {
                        BaseMapping = classMapping,
                        TargetType = arrAttr.SubItemType,
                        TargetDataType = arrAttrSubDataType,
                        ValueHandler = GetSharedValueHandler(arrAttr.ValueHandler) ?? DefaultValueHandler.Instance,
                        Name = propertyName,
                        Path = propertyPath
                    };
                }

                classMapping.Mapping.Add(arrayMapping);
            }
        }

        return classMapping;
    }

    private static DataType ConvertType(OsuDbTypeAttribute? attribute, Type targetType)
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