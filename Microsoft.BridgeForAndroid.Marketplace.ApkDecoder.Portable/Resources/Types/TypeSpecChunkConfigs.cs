// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.TypeSpecChunkConfigs
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  [Flags]
  public enum TypeSpecChunkConfigs
  {
    Mcc = 1,
    Mnc = 2,
    Locale = 4,
    Touchscreen = 8,
    Keyboard = 16, // 0x00000010
    KeyboardHidden = 32, // 0x00000020
    Navigation = 64, // 0x00000040
    Orientation = 128, // 0x00000080
    Density = 256, // 0x00000100
    ScreenSize = 512, // 0x00000200
    Version = 1024, // 0x00000400
    ScreenLayout = 2048, // 0x00000800
    UiMode = 4096, // 0x00001000
    SmallestScreenSize = 8192, // 0x00002000
    LayoutDir = 16384, // 0x00004000
    NotYetUsed = 32768, // 0x00008000
    MncZero = NotYetUsed | LayoutDir | SmallestScreenSize | UiMode | ScreenLayout | Version | ScreenSize | Density | Orientation | Navigation | KeyboardHidden | Keyboard | Touchscreen | Locale | Mnc | Mcc, // 0x0000FFFF
  }
}
