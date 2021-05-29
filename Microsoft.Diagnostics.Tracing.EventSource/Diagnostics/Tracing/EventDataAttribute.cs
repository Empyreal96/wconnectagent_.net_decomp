// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventDataAttribute
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
  public class EventDataAttribute : Attribute
  {
    private EventLevel level = ~EventLevel.LogAlways;
    private EventOpcode opcode = ~EventOpcode.Info;

    public string Name { get; set; }

    internal EventLevel Level
    {
      get => this.level;
      set => this.level = value;
    }

    internal EventOpcode Opcode
    {
      get => this.opcode;
      set => this.opcode = value;
    }

    internal EventKeywords Keywords { get; set; }

    internal EventTags Tags { get; set; }
  }
}
