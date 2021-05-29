// Decompiled with JetBrains decompiler
// Type: Microsoft.Reflection.TypeCode
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Reflection
{
  public enum TypeCode
  {
    Empty = 0,
    Object = 1,
    DBNull = 2,
    Boolean = 3,
    Char = 4,
    SByte = 5,
    Byte = 6,
    Int16 = 7,
    UInt16 = 8,
    Int32 = 9,
    UInt32 = 10, // 0x0000000A
    Int64 = 11, // 0x0000000B
    UInt64 = 12, // 0x0000000C
    Single = 13, // 0x0000000D
    Double = 14, // 0x0000000E
    Decimal = 15, // 0x0000000F
    DateTime = 16, // 0x00000010
    String = 18, // 0x00000012
  }
}
