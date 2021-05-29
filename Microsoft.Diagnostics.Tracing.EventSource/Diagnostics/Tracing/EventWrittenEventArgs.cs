// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventWrittenEventArgs
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security;

namespace Microsoft.Diagnostics.Tracing
{
  public class EventWrittenEventArgs : EventArgs
  {
    private string m_message;
    private string m_eventName;
    private EventSource m_eventSource;
    private ReadOnlyCollection<string> m_payloadNames;
    internal EventTags m_tags;
    internal EventOpcode m_opcode;
    internal EventKeywords m_keywords;

    public string EventName
    {
      get => this.m_eventName != null || this.EventId < 0 ? this.m_eventName : this.m_eventSource.m_eventData[this.EventId].Name;
      internal set => this.m_eventName = value;
    }

    public int EventId { get; internal set; }

    public Guid ActivityId
    {
      [SecurityCritical] get => EventSource.CurrentThreadActivityId;
    }

    public Guid RelatedActivityId { [SecurityCritical] get; internal set; }

    public ReadOnlyCollection<object> Payload { get; internal set; }

    public ReadOnlyCollection<string> PayloadNames
    {
      get
      {
        if (this.m_payloadNames == null)
        {
          List<string> stringList = new List<string>();
          foreach (ParameterInfo parameter in this.m_eventSource.m_eventData[this.EventId].Parameters)
            stringList.Add(parameter.Name);
          this.m_payloadNames = new ReadOnlyCollection<string>((IList<string>) stringList);
        }
        return this.m_payloadNames;
      }
      internal set => this.m_payloadNames = value;
    }

    public EventSource EventSource => this.m_eventSource;

    public EventKeywords Keywords => this.EventId < 0 ? this.m_keywords : (EventKeywords) this.m_eventSource.m_eventData[this.EventId].Descriptor.Keywords;

    public EventOpcode Opcode => this.EventId < 0 ? this.m_opcode : (EventOpcode) this.m_eventSource.m_eventData[this.EventId].Descriptor.Opcode;

    public EventTask Task => this.EventId < 0 ? EventTask.None : (EventTask) this.m_eventSource.m_eventData[this.EventId].Descriptor.Task;

    public EventTags Tags => this.EventId < 0 ? this.m_tags : this.m_eventSource.m_eventData[this.EventId].Tags;

    public string Message
    {
      get => this.EventId < 0 ? this.m_message : this.m_eventSource.m_eventData[this.EventId].Message;
      internal set => this.m_message = value;
    }

    public EventChannel Channel => this.EventId < 0 ? EventChannel.None : (EventChannel) this.m_eventSource.m_eventData[this.EventId].Descriptor.Channel;

    public byte Version => this.EventId < 0 ? (byte) 0 : this.m_eventSource.m_eventData[this.EventId].Descriptor.Version;

    public EventLevel Level => (uint) this.EventId >= (uint) this.m_eventSource.m_eventData.Length ? EventLevel.LogAlways : (EventLevel) this.m_eventSource.m_eventData[this.EventId].Descriptor.Level;

    internal EventWrittenEventArgs(EventSource eventSource) => this.m_eventSource = eventSource;
  }
}
