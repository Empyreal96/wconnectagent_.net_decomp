// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventDispatcher
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal class EventDispatcher
  {
    internal readonly EventListener m_Listener;
    internal bool[] m_EventEnabled;
    internal EventDispatcher m_Next;

    internal EventDispatcher(EventDispatcher next, bool[] eventEnabled, EventListener listener)
    {
      this.m_Next = next;
      this.m_EventEnabled = eventEnabled;
      this.m_Listener = listener;
    }
  }
}
