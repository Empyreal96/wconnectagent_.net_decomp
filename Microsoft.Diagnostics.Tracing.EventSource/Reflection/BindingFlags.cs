// Decompiled with JetBrains decompiler
// Type: Microsoft.Reflection.BindingFlags
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Reflection
{
  [Flags]
  public enum BindingFlags
  {
    DeclaredOnly = 2,
    Instance = 4,
    Static = 8,
    Public = 16, // 0x00000010
    NonPublic = 32, // 0x00000020
  }
}
