// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ScreenDensity
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
  public enum ScreenDensity : uint
  {
    Default = 0,
    Low = 120, // 0x00000078
    Medium = 160, // 0x000000A0
    TV = 213, // 0x000000D5
    High = 240, // 0x000000F0
    XHigh = 320, // 0x00000140
    XXHigh = 480, // 0x000001E0
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Following expected naming.", MessageId = "XXX")] XXXHigh = 640, // 0x00000280
    Any = 65534, // 0x0000FFFE
    None = 65535, // 0x0000FFFF
  }
}
