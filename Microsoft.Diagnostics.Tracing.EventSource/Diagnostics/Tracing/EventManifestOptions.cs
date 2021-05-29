// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventManifestOptions
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  [Flags]
  public enum EventManifestOptions
  {
    None = 0,
    Strict = 1,
    AllCultures = 2,
    OnlyIfNeededForRegistration = 4,
    AllowEventSourceOverride = 8,
  }
}
