// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.ResourceValueTypes
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  [Flags]
  public enum ResourceValueTypes
  {
    None = 0,
    Reference = 1,
    Attribute = 2,
    String = Attribute | Reference, // 0x00000003
    Float = 4,
    Dimension = Float | Reference, // 0x00000005
    Fraction = Float | Attribute, // 0x00000006
    FirstInt = 16, // 0x00000010
    IntDec = FirstInt, // 0x00000010
    IntHex = IntDec | Reference, // 0x00000011
    IntBoolean = IntDec | Attribute, // 0x00000012
    FirstColorInt = 28, // 0x0000001C
    IntColorArgb8 = FirstColorInt, // 0x0000001C
    IntColorRgb8 = IntColorArgb8 | Reference, // 0x0000001D
    IntColorArgb4 = IntColorArgb8 | Attribute, // 0x0000001E
    IntColorRgb4 = IntColorArgb4 | Reference, // 0x0000001F
    LastColorInt = IntColorRgb4, // 0x0000001F
    LastInt = LastColorInt, // 0x0000001F
  }
}
