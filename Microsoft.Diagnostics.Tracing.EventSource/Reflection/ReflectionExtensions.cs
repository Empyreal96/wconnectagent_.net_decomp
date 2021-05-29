// Decompiled with JetBrains decompiler
// Type: Microsoft.Reflection.ReflectionExtensions
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Reflection
{
  internal static class ReflectionExtensions
  {
    public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

    public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

    public static bool IsSealed(this Type type) => type.GetTypeInfo().IsSealed;

    public static Type BaseType(this Type type) => type.GetTypeInfo().BaseType;

    public static System.Reflection.Assembly Assembly(this Type type) => type.GetTypeInfo().Assembly;

    public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
    {
      Func<MethodInfo, bool> func1;
      switch (flags & (BindingFlags.Public | BindingFlags.NonPublic))
      {
        case (BindingFlags) 0:
          func1 = (Func<MethodInfo, bool>) (mi => false);
          break;
        case BindingFlags.Public:
          func1 = (Func<MethodInfo, bool>) (mi => mi.IsPublic);
          break;
        case BindingFlags.NonPublic:
          func1 = (Func<MethodInfo, bool>) (mi => !mi.IsPublic);
          break;
        default:
          func1 = (Func<MethodInfo, bool>) (mi => true);
          break;
      }
      Func<MethodInfo, bool> func2;
      switch (flags & (BindingFlags.Instance | BindingFlags.Static))
      {
        case (BindingFlags) 0:
          func2 = (Func<MethodInfo, bool>) (mi => false);
          break;
        case BindingFlags.Instance:
          func2 = (Func<MethodInfo, bool>) (mi => !mi.IsStatic);
          break;
        case BindingFlags.Static:
          func2 = (Func<MethodInfo, bool>) (mi => mi.IsStatic);
          break;
        default:
          func2 = (Func<MethodInfo, bool>) (mi => true);
          break;
      }
      List<MethodInfo> methodInfoList = new List<MethodInfo>();
      foreach (MethodInfo declaredMethod in type.GetTypeInfo().DeclaredMethods)
      {
        if (func1(declaredMethod) && func2(declaredMethod))
          methodInfoList.Add(declaredMethod);
      }
      return methodInfoList.ToArray();
    }

    public static FieldInfo[] GetFields(this Type type, BindingFlags flags)
    {
      Func<FieldInfo, bool> func1;
      switch (flags & (BindingFlags.Public | BindingFlags.NonPublic))
      {
        case (BindingFlags) 0:
          func1 = (Func<FieldInfo, bool>) (fi => false);
          break;
        case BindingFlags.Public:
          func1 = (Func<FieldInfo, bool>) (fi => fi.IsPublic);
          break;
        case BindingFlags.NonPublic:
          func1 = (Func<FieldInfo, bool>) (fi => !fi.IsPublic);
          break;
        default:
          func1 = (Func<FieldInfo, bool>) (fi => true);
          break;
      }
      Func<FieldInfo, bool> func2;
      switch (flags & (BindingFlags.Instance | BindingFlags.Static))
      {
        case (BindingFlags) 0:
          func2 = (Func<FieldInfo, bool>) (fi => false);
          break;
        case BindingFlags.Instance:
          func2 = (Func<FieldInfo, bool>) (fi => !fi.IsStatic);
          break;
        case BindingFlags.Static:
          func2 = (Func<FieldInfo, bool>) (fi => fi.IsStatic);
          break;
        default:
          func2 = (Func<FieldInfo, bool>) (fi => true);
          break;
      }
      List<FieldInfo> fieldInfoList = new List<FieldInfo>();
      foreach (FieldInfo declaredField in type.GetTypeInfo().DeclaredFields)
      {
        if (func1(declaredField) && func2(declaredField))
          fieldInfoList.Add(declaredField);
      }
      return fieldInfoList.ToArray();
    }

    public static Type GetNestedType(this Type type, string nestedTypeName)
    {
      TypeInfo typeInfo = (TypeInfo) null;
      foreach (TypeInfo declaredNestedType in type.GetTypeInfo().DeclaredNestedTypes)
      {
        if (declaredNestedType.Name == nestedTypeName)
        {
          typeInfo = declaredNestedType;
          break;
        }
      }
      return typeInfo?.AsType();
    }

    public static TypeCode GetTypeCode(this Type type)
    {
      if ((object) type == (object) typeof (bool))
        return TypeCode.Boolean;
      if ((object) type == (object) typeof (byte))
        return TypeCode.Byte;
      if ((object) type == (object) typeof (char))
        return TypeCode.Char;
      if ((object) type == (object) typeof (ushort))
        return TypeCode.UInt16;
      if ((object) type == (object) typeof (uint))
        return TypeCode.UInt32;
      if ((object) type == (object) typeof (ulong))
        return TypeCode.UInt64;
      if ((object) type == (object) typeof (sbyte))
        return TypeCode.SByte;
      if ((object) type == (object) typeof (short))
        return TypeCode.Int16;
      if ((object) type == (object) typeof (int))
        return TypeCode.Int32;
      if ((object) type == (object) typeof (long))
        return TypeCode.Int64;
      if ((object) type == (object) typeof (string))
        return TypeCode.String;
      if ((object) type == (object) typeof (float))
        return TypeCode.Single;
      if ((object) type == (object) typeof (double))
        return TypeCode.Double;
      if ((object) type == (object) typeof (DateTime))
        return TypeCode.DateTime;
      return (object) type == (object) typeof (Decimal) ? TypeCode.Decimal : TypeCode.Object;
    }

    public static object GetRawConstantValue(this FieldInfo fi) => fi.GetValue((object) null);

    public static bool ReflectionOnly(this System.Reflection.Assembly assm) => false;
  }
}
