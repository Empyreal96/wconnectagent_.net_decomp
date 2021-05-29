// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventDescriptor
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Diagnostics.Tracing
{
  [HostProtection(MayLeakOnAbort = true)]
  [StructLayout(LayoutKind.Explicit, Size = 16)]
  internal struct EventDescriptor
  {
    [FieldOffset(0)]
    private int m_traceloggingId;
    [FieldOffset(0)]
    private ushort m_id;
    [FieldOffset(2)]
    private byte m_version;
    [FieldOffset(3)]
    private byte m_channel;
    [FieldOffset(4)]
    private byte m_level;
    [FieldOffset(5)]
    private byte m_opcode;
    [FieldOffset(6)]
    private ushort m_task;
    [FieldOffset(8)]
    private long m_keywords;

    public EventDescriptor(int traceloggingId, byte level, byte opcode, long keywords)
    {
      this.m_id = (ushort) 0;
      this.m_version = (byte) 0;
      this.m_channel = (byte) 0;
      this.m_traceloggingId = traceloggingId;
      this.m_level = level;
      this.m_opcode = opcode;
      this.m_task = (ushort) 0;
      this.m_keywords = keywords;
    }

    public EventDescriptor(
      int id,
      byte version,
      byte channel,
      byte level,
      byte opcode,
      int task,
      long keywords)
    {
      if (id < 0)
        throw new ArgumentOutOfRangeException(nameof (id), Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
      if (id > (int) ushort.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (id), Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("ArgumentOutOfRange_NeedValidId", (object) 1, (object) ushort.MaxValue));
      this.m_traceloggingId = 0;
      this.m_id = (ushort) id;
      this.m_version = version;
      this.m_channel = channel;
      this.m_level = level;
      this.m_opcode = opcode;
      this.m_keywords = keywords;
      if (task < 0)
        throw new ArgumentOutOfRangeException(nameof (task), Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
      this.m_task = task <= (int) ushort.MaxValue ? (ushort) task : throw new ArgumentOutOfRangeException(nameof (task), Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("ArgumentOutOfRange_NeedValidId", (object) 1, (object) ushort.MaxValue));
    }

    public int EventId => (int) this.m_id;

    public byte Version => this.m_version;

    public byte Channel => this.m_channel;

    public byte Level => this.m_level;

    public byte Opcode => this.m_opcode;

    public int Task => (int) this.m_task;

    public long Keywords => this.m_keywords;

    public override bool Equals(object obj) => obj is EventDescriptor other && this.Equals(other);

    public override int GetHashCode() => (int) this.m_id ^ (int) this.m_version ^ (int) this.m_channel ^ (int) this.m_level ^ (int) this.m_opcode ^ (int) this.m_task ^ (int) this.m_keywords;

    public bool Equals(EventDescriptor other) => (int) this.m_id == (int) other.m_id && (int) this.m_version == (int) other.m_version && ((int) this.m_channel == (int) other.m_channel && (int) this.m_level == (int) other.m_level) && ((int) this.m_opcode == (int) other.m_opcode && (int) this.m_task == (int) other.m_task && this.m_keywords == other.m_keywords);

    public static bool operator ==(EventDescriptor event1, EventDescriptor event2) => event1.Equals(event2);

    public static bool operator !=(EventDescriptor event1, EventDescriptor event2) => !event1.Equals(event2);
  }
}
