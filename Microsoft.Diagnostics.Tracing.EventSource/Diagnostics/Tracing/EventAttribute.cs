// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventAttribute
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class EventAttribute : Attribute
  {
    private EventOpcode m_opcode;
    private bool m_opcodeSet;

    public EventAttribute(int eventId)
    {
      this.EventId = eventId;
      this.Level = EventLevel.Informational;
      this.m_opcodeSet = false;
    }

    public int EventId { get; private set; }

    public EventLevel Level { get; set; }

    public EventKeywords Keywords { get; set; }

    public EventOpcode Opcode
    {
      get => this.m_opcode;
      set
      {
        this.m_opcode = value;
        this.m_opcodeSet = true;
      }
    }

    internal bool IsOpcodeSet => this.m_opcodeSet;

    public EventTask Task { get; set; }

    public EventChannel Channel { get; set; }

    public byte Version { get; set; }

    public string Message { get; set; }

    public EventTags Tags { get; set; }

    public EventActivityOptions ActivityOptions { get; set; }
  }
}
