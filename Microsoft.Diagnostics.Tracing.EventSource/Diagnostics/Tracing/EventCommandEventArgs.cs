// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventCommandEventArgs
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  public class EventCommandEventArgs : EventArgs
  {
    internal EventSource eventSource;
    internal EventDispatcher dispatcher;
    internal EventListener listener;
    internal int perEventSourceSessionId;
    internal int etwSessionId;
    internal bool enable;
    internal EventLevel level;
    internal EventKeywords matchAnyKeyword;
    internal EventCommandEventArgs nextCommand;

    public EventCommand Command { get; internal set; }

    public IDictionary<string, string> Arguments { get; internal set; }

    public bool EnableEvent(int eventId)
    {
      if (this.Command != EventCommand.Enable && this.Command != EventCommand.Disable)
        throw new InvalidOperationException();
      return this.eventSource.EnableEventForDispatcher(this.dispatcher, eventId, true);
    }

    public bool DisableEvent(int eventId)
    {
      if (this.Command != EventCommand.Enable && this.Command != EventCommand.Disable)
        throw new InvalidOperationException();
      return this.eventSource.EnableEventForDispatcher(this.dispatcher, eventId, false);
    }

    internal EventCommandEventArgs(
      EventCommand command,
      IDictionary<string, string> arguments,
      EventSource eventSource,
      EventListener listener,
      int perEventSourceSessionId,
      int etwSessionId,
      bool enable,
      EventLevel level,
      EventKeywords matchAnyKeyword)
    {
      this.Command = command;
      this.Arguments = arguments;
      this.eventSource = eventSource;
      this.listener = listener;
      this.perEventSourceSessionId = perEventSourceSessionId;
      this.etwSessionId = etwSessionId;
      this.enable = enable;
      this.level = level;
      this.matchAnyKeyword = matchAnyKeyword;
    }
  }
}
