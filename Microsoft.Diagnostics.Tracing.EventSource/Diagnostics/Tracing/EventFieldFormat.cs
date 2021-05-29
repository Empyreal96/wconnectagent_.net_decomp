// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventFieldFormat
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  public enum EventFieldFormat
  {
    Default = 0,
    String = 2,
    Boolean = 3,
    Hexadecimal = 4,
    Xml = 11, // 0x0000000B
    Json = 12, // 0x0000000C
    HResult = 15, // 0x0000000F
  }
}
