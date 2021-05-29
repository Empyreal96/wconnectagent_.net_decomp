// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.TraceLoggingDataType
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal enum TraceLoggingDataType
  {
    Nil = 0,
    Utf16String = 1,
    MbcsString = 2,
    Int8 = 3,
    UInt8 = 4,
    Int16 = 5,
    UInt16 = 6,
    Int32 = 7,
    UInt32 = 8,
    Int64 = 9,
    UInt64 = 10, // 0x0000000A
    Float = 11, // 0x0000000B
    Double = 12, // 0x0000000C
    Boolean32 = 13, // 0x0000000D
    Binary = 14, // 0x0000000E
    Guid = 15, // 0x0000000F
    FileTime = 17, // 0x00000011
    SystemTime = 18, // 0x00000012
    HexInt32 = 20, // 0x00000014
    HexInt64 = 21, // 0x00000015
    CountedUtf16String = 22, // 0x00000016
    CountedMbcsString = 23, // 0x00000017
    Struct = 24, // 0x00000018
    Char8 = 516, // 0x00000204
    Char16 = 518, // 0x00000206
    Boolean8 = 772, // 0x00000304
    HexInt8 = 1028, // 0x00000404
    HexInt16 = 1030, // 0x00000406
    Utf16Xml = 2817, // 0x00000B01
    MbcsXml = 2818, // 0x00000B02
    CountedUtf16Xml = 2838, // 0x00000B16
    CountedMbcsXml = 2839, // 0x00000B17
    Utf16Json = 3073, // 0x00000C01
    MbcsJson = 3074, // 0x00000C02
    CountedUtf16Json = 3094, // 0x00000C16
    CountedMbcsJson = 3095, // 0x00000C17
    HResult = 3847, // 0x00000F07
  }
}
