﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventFieldAttribute
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  [AttributeUsage(AttributeTargets.Property)]
  public class EventFieldAttribute : Attribute
  {
    public EventFieldTags Tags { get; set; }

    internal string Name { get; set; }

    public EventFieldFormat Format { get; set; }
  }
}
