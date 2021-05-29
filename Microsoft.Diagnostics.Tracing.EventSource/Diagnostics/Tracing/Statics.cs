// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.Statics
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Diagnostics.Tracing
{
  internal static class Statics
  {
    public const byte DefaultLevel = 5;
    public const byte TraceLoggingChannel = 11;
    public const byte InTypeMask = 31;
    public const byte InTypeFixedCountFlag = 32;
    public const byte InTypeVariableCountFlag = 64;
    public const byte InTypeCustomCountFlag = 96;
    public const byte InTypeCountMask = 96;
    public const byte InTypeChainFlag = 128;
    public const byte OutTypeMask = 127;
    public const byte OutTypeChainFlag = 128;
    public const EventTags EventTagsMask = (EventTags) 268435455;
    public static readonly TraceLoggingDataType IntPtrType = IntPtr.Size == 8 ? TraceLoggingDataType.Int64 : TraceLoggingDataType.Int32;
    public static readonly TraceLoggingDataType UIntPtrType = IntPtr.Size == 8 ? TraceLoggingDataType.UInt64 : TraceLoggingDataType.UInt32;
    public static readonly TraceLoggingDataType HexIntPtrType = IntPtr.Size == 8 ? TraceLoggingDataType.HexInt64 : TraceLoggingDataType.HexInt32;

    public static byte[] MetadataForString(
      string name,
      int prefixSize,
      int suffixSize,
      int additionalSize)
    {
      Statics.CheckName(name);
      int length = Encoding.UTF8.GetByteCount(name) + 3 + prefixSize + suffixSize;
      byte[] bytes = new byte[length];
      ushort num = checked ((ushort) (length + additionalSize));
      bytes[0] = (byte) num;
      bytes[1] = (byte) ((uint) num >> 8);
      Encoding.UTF8.GetBytes(name, 0, name.Length, bytes, 2 + prefixSize);
      return bytes;
    }

    public static void EncodeTags(int tags, ref int pos, byte[] metadata)
    {
      int num1 = tags & 268435455;
      bool flag;
      do
      {
        byte num2 = (byte) (num1 >> 21 & (int) sbyte.MaxValue);
        flag = (num1 & 2097151) != 0;
        byte num3 = (byte) ((int) num2 | (flag ? 128 : 0));
        num1 <<= 7;
        if (metadata != null)
          metadata[pos] = num3;
        ++pos;
      }
      while (flag);
    }

    public static byte Combine(int settingValue, byte defaultValue) => (int) (byte) settingValue != settingValue ? defaultValue : (byte) settingValue;

    public static byte Combine(int settingValue1, int settingValue2, byte defaultValue)
    {
      if ((int) (byte) settingValue1 == settingValue1)
        return (byte) settingValue1;
      return (int) (byte) settingValue2 != settingValue2 ? defaultValue : (byte) settingValue2;
    }

    public static int Combine(int settingValue1, int settingValue2) => (int) (byte) settingValue1 != settingValue1 ? settingValue2 : settingValue1;

    public static void CheckName(string name)
    {
      if (name != null && 0 <= name.IndexOf(char.MinValue))
        throw new ArgumentOutOfRangeException(nameof (name));
    }

    public static bool ShouldOverrideFieldName(string fieldName) => fieldName.Length <= 2 && fieldName[0] == '_';

    public static TraceLoggingDataType MakeDataType(
      TraceLoggingDataType baseType,
      EventFieldFormat format)
    {
      return baseType & (TraceLoggingDataType) 31 | (TraceLoggingDataType) ((int) format << 8);
    }

    public static TraceLoggingDataType Format8(
      EventFieldFormat format,
      TraceLoggingDataType native)
    {
      switch (format)
      {
        case EventFieldFormat.Default:
          return native;
        case EventFieldFormat.String:
          return TraceLoggingDataType.Char8;
        case EventFieldFormat.Boolean:
          return TraceLoggingDataType.Boolean8;
        case EventFieldFormat.Hexadecimal:
          return TraceLoggingDataType.HexInt8;
        default:
          return Statics.MakeDataType(native, format);
      }
    }

    public static TraceLoggingDataType Format16(
      EventFieldFormat format,
      TraceLoggingDataType native)
    {
      switch (format)
      {
        case EventFieldFormat.Default:
          return native;
        case EventFieldFormat.String:
          return TraceLoggingDataType.Char16;
        case EventFieldFormat.Hexadecimal:
          return TraceLoggingDataType.HexInt16;
        default:
          return Statics.MakeDataType(native, format);
      }
    }

    public static TraceLoggingDataType Format32(
      EventFieldFormat format,
      TraceLoggingDataType native)
    {
      switch (format)
      {
        case EventFieldFormat.Default:
          return native;
        case EventFieldFormat.Boolean:
          return TraceLoggingDataType.Boolean32;
        case EventFieldFormat.Hexadecimal:
          return TraceLoggingDataType.HexInt32;
        case EventFieldFormat.HResult:
          return TraceLoggingDataType.HResult;
        default:
          return Statics.MakeDataType(native, format);
      }
    }

    public static TraceLoggingDataType Format64(
      EventFieldFormat format,
      TraceLoggingDataType native)
    {
      switch (format)
      {
        case EventFieldFormat.Default:
          return native;
        case EventFieldFormat.Hexadecimal:
          return TraceLoggingDataType.HexInt64;
        default:
          return Statics.MakeDataType(native, format);
      }
    }

    public static TraceLoggingDataType FormatPtr(
      EventFieldFormat format,
      TraceLoggingDataType native)
    {
      switch (format)
      {
        case EventFieldFormat.Default:
          return native;
        case EventFieldFormat.Hexadecimal:
          return Statics.HexIntPtrType;
        default:
          return Statics.MakeDataType(native, format);
      }
    }

    public static object CreateInstance(Type type, params object[] parameters) => Activator.CreateInstance(type, parameters);

    public static bool IsValueType(Type type) => type.GetTypeInfo().IsValueType;

    public static bool IsEnum(Type type) => type.GetTypeInfo().IsEnum;

    public static IEnumerable<PropertyInfo> GetProperties(Type type) => type.GetRuntimeProperties();

    public static MethodInfo GetGetMethod(PropertyInfo propInfo) => propInfo.GetMethod;

    public static MethodInfo GetDeclaredStaticMethod(Type declaringType, string name) => declaringType.GetTypeInfo().GetDeclaredMethod(name);

    public static bool HasCustomAttribute(PropertyInfo propInfo, Type attributeType) => propInfo.IsDefined(attributeType);

    public static AttributeType GetCustomAttribute<AttributeType>(PropertyInfo propInfo) where AttributeType : Attribute
    {
      AttributeType attributeType = default (AttributeType);
      using (IEnumerator<AttributeType> enumerator = propInfo.GetCustomAttributes<AttributeType>(false).GetEnumerator())
      {
        if (enumerator.MoveNext())
          attributeType = enumerator.Current;
      }
      return attributeType;
    }

    public static AttributeType GetCustomAttribute<AttributeType>(Type type) where AttributeType : Attribute
    {
      AttributeType attributeType = default (AttributeType);
      using (IEnumerator<AttributeType> enumerator = type.GetTypeInfo().GetCustomAttributes<AttributeType>(false).GetEnumerator())
      {
        if (enumerator.MoveNext())
          attributeType = enumerator.Current;
      }
      return attributeType;
    }

    public static Type[] GetGenericArguments(Type type) => type.GenericTypeArguments;

    public static Type FindEnumerableElementType(Type type)
    {
      Type type1 = (Type) null;
      if (Statics.IsGenericMatch(type, (object) typeof (IEnumerable<>)))
      {
        type1 = Statics.GetGenericArguments(type)[0];
      }
      else
      {
        foreach (Type implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
        {
          if (Statics.IsGenericMatch(implementedInterface, (object) typeof (IEnumerable<>)))
          {
            if ((object) type1 != null)
            {
              type1 = (Type) null;
              break;
            }
            type1 = Statics.GetGenericArguments(implementedInterface)[0];
          }
        }
      }
      return type1;
    }

    public static bool IsGenericMatch(Type type, object openType) => type.IsConstructedGenericType && (object) type.GetGenericTypeDefinition() == (object) (Type) openType;

    public static Delegate CreateDelegate(Type delegateType, MethodInfo methodInfo) => methodInfo.CreateDelegate(delegateType);

    public static TraceLoggingTypeInfo GetTypeInfoInstance(
      Type dataType,
      List<Type> recursionCheck)
    {
      TraceLoggingTypeInfo traceLoggingTypeInfo;
      if ((object) dataType == (object) typeof (int))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) TraceLoggingTypeInfo<int>.Instance;
      else if ((object) dataType == (object) typeof (long))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) TraceLoggingTypeInfo<long>.Instance;
      else if ((object) dataType == (object) typeof (string))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) TraceLoggingTypeInfo<string>.Instance;
      else
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) Statics.GetDeclaredStaticMethod(typeof (TraceLoggingTypeInfo<>).MakeGenericType(dataType), "GetInstance").Invoke((object) null, new object[1]
        {
          (object) recursionCheck
        });
      return traceLoggingTypeInfo;
    }

    public static TraceLoggingTypeInfo<DataType> CreateDefaultTypeInfo<DataType>(
      List<Type> recursionCheck)
    {
      Type type = typeof (DataType);
      if (recursionCheck.Contains(type))
        throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_RecursiveTypeDefinition"));
      recursionCheck.Add(type);
      EventDataAttribute customAttribute = Statics.GetCustomAttribute<EventDataAttribute>(type);
      TraceLoggingTypeInfo traceLoggingTypeInfo;
      if (customAttribute != null || Statics.GetCustomAttribute<CompilerGeneratedAttribute>(type) != null)
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new InvokeTypeInfo<DataType>(new TypeAnalysis(type, customAttribute, recursionCheck));
      else if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        if ((object) elementType == (object) typeof (bool))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new BooleanArrayTypeInfo();
        else if ((object) elementType == (object) typeof (byte))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new ByteArrayTypeInfo();
        else if ((object) elementType == (object) typeof (sbyte))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new SByteArrayTypeInfo();
        else if ((object) elementType == (object) typeof (short))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int16ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (ushort))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt16ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (int))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int32ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (uint))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt32ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (long))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int64ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (ulong))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt64ArrayTypeInfo();
        else if ((object) elementType == (object) typeof (char))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new CharArrayTypeInfo();
        else if ((object) elementType == (object) typeof (double))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new DoubleArrayTypeInfo();
        else if ((object) elementType == (object) typeof (float))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new SingleArrayTypeInfo();
        else if ((object) elementType == (object) typeof (IntPtr))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new IntPtrArrayTypeInfo();
        else if ((object) elementType == (object) typeof (UIntPtr))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UIntPtrArrayTypeInfo();
        else if ((object) elementType == (object) typeof (Guid))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new GuidArrayTypeInfo();
        else
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) Statics.CreateInstance(typeof (ArrayTypeInfo<>).MakeGenericType(elementType), (object) Statics.GetTypeInfoInstance(elementType, recursionCheck));
      }
      else if (Statics.IsEnum(type))
      {
        Type underlyingType = Enum.GetUnderlyingType(type);
        if ((object) underlyingType == (object) typeof (int))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumInt32TypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (uint))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumUInt32TypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (byte))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumByteTypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (sbyte))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumSByteTypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (short))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumInt16TypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (ushort))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumUInt16TypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (long))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumInt64TypeInfo<DataType>();
        else if ((object) underlyingType == (object) typeof (ulong))
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) new EnumUInt64TypeInfo<DataType>();
        else
          throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedEnumType", (object) type.Name, (object) underlyingType.Name));
      }
      else if ((object) type == (object) typeof (string))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new StringTypeInfo();
      else if ((object) type == (object) typeof (bool))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new BooleanTypeInfo();
      else if ((object) type == (object) typeof (byte))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new ByteTypeInfo();
      else if ((object) type == (object) typeof (sbyte))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new SByteTypeInfo();
      else if ((object) type == (object) typeof (short))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int16TypeInfo();
      else if ((object) type == (object) typeof (ushort))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt16TypeInfo();
      else if ((object) type == (object) typeof (int))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int32TypeInfo();
      else if ((object) type == (object) typeof (uint))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt32TypeInfo();
      else if ((object) type == (object) typeof (long))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new Int64TypeInfo();
      else if ((object) type == (object) typeof (ulong))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UInt64TypeInfo();
      else if ((object) type == (object) typeof (char))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new CharTypeInfo();
      else if ((object) type == (object) typeof (double))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new DoubleTypeInfo();
      else if ((object) type == (object) typeof (float))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new SingleTypeInfo();
      else if ((object) type == (object) typeof (DateTime))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new DateTimeTypeInfo();
      else if ((object) type == (object) typeof (Decimal))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new DecimalTypeInfo();
      else if ((object) type == (object) typeof (IntPtr))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new IntPtrTypeInfo();
      else if ((object) type == (object) typeof (UIntPtr))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new UIntPtrTypeInfo();
      else if ((object) type == (object) typeof (Guid))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new GuidTypeInfo();
      else if ((object) type == (object) typeof (TimeSpan))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new TimeSpanTypeInfo();
      else if ((object) type == (object) typeof (DateTimeOffset))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new DateTimeOffsetTypeInfo();
      else if ((object) type == (object) typeof (EmptyStruct))
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) new NullTypeInfo<EmptyStruct>();
      else if (Statics.IsGenericMatch(type, (object) typeof (KeyValuePair<,>)))
      {
        Type[] genericArguments = Statics.GetGenericArguments(type);
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) Statics.CreateInstance(typeof (KeyValuePairTypeInfo<,>).MakeGenericType(genericArguments[0], genericArguments[1]), (object) recursionCheck);
      }
      else if (Statics.IsGenericMatch(type, (object) typeof (Nullable<>)))
      {
        traceLoggingTypeInfo = (TraceLoggingTypeInfo) Statics.CreateInstance(typeof (NullableTypeInfo<>).MakeGenericType(Statics.GetGenericArguments(type)[0]), (object) recursionCheck);
      }
      else
      {
        Type enumerableElementType = Statics.FindEnumerableElementType(type);
        if ((object) enumerableElementType != null)
          traceLoggingTypeInfo = (TraceLoggingTypeInfo) Statics.CreateInstance(typeof (EnumerableTypeInfo<,>).MakeGenericType(type, enumerableElementType), (object) Statics.GetTypeInfoInstance(enumerableElementType, recursionCheck));
        else
          throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NonCompliantTypeError", (object) type.Name));
      }
      return (TraceLoggingTypeInfo<DataType>) traceLoggingTypeInfo;
    }
  }
}
