// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventChannelAttribute
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  [AttributeUsage(AttributeTargets.Field)]
  internal class EventChannelAttribute : Attribute
  {
    public bool Enabled { get; set; }

    public EventChannelType EventChannelType { get; set; }
  }
}
