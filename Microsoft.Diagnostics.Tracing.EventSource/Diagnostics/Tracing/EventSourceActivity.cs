// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventSourceActivity
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class EventSourceActivity : IDisposable
  {
    private readonly EventSource eventSource;
    private EventSourceOptions startStopOptions;
    internal Guid activityId;
    private EventSourceActivity.State state;
    private string eventName;
    internal static Guid s_empty;

    public EventSourceActivity(EventSource eventSource) => this.eventSource = eventSource != null ? eventSource : throw new ArgumentNullException(nameof (eventSource));

    public static implicit operator EventSourceActivity(EventSource eventSource) => new EventSourceActivity(eventSource);

    public EventSource EventSource => this.eventSource;

    public Guid Id => this.activityId;

    public EventSourceActivity Start<T>(
      string eventName,
      EventSourceOptions options,
      T data)
    {
      return this.Start<T>(eventName, ref options, ref data);
    }

    public EventSourceActivity Start(string eventName)
    {
      EventSourceOptions options = new EventSourceOptions();
      EmptyStruct data = new EmptyStruct();
      return this.Start<EmptyStruct>(eventName, ref options, ref data);
    }

    public EventSourceActivity Start(
      string eventName,
      EventSourceOptions options)
    {
      EmptyStruct data = new EmptyStruct();
      return this.Start<EmptyStruct>(eventName, ref options, ref data);
    }

    public EventSourceActivity Start<T>(string eventName, T data)
    {
      EventSourceOptions options = new EventSourceOptions();
      return this.Start<T>(eventName, ref options, ref data);
    }

    public void Stop<T>(T data) => this.Stop<T>((string) null, ref data);

    public void Stop<T>(string eventName)
    {
      EmptyStruct data = new EmptyStruct();
      this.Stop<EmptyStruct>(eventName, ref data);
    }

    public void Stop<T>(string eventName, T data) => this.Stop<T>(eventName, ref data);

    public void Write<T>(string eventName, EventSourceOptions options, T data) => this.Write<T>(this.eventSource, eventName, ref options, ref data);

    public void Write<T>(string eventName, T data)
    {
      EventSourceOptions options = new EventSourceOptions();
      this.Write<T>(this.eventSource, eventName, ref options, ref data);
    }

    public void Write(string eventName, EventSourceOptions options)
    {
      EmptyStruct data = new EmptyStruct();
      this.Write<EmptyStruct>(this.eventSource, eventName, ref options, ref data);
    }

    public void Write(string eventName)
    {
      EventSourceOptions options = new EventSourceOptions();
      EmptyStruct data = new EmptyStruct();
      this.Write<EmptyStruct>(this.eventSource, eventName, ref options, ref data);
    }

    public void Write<T>(EventSource source, string eventName, EventSourceOptions options, T data) => this.Write<T>(source, eventName, ref options, ref data);

    public void Dispose()
    {
      if (this.state != EventSourceActivity.State.Started)
        return;
      EmptyStruct data = new EmptyStruct();
      this.Stop<EmptyStruct>((string) null, ref data);
    }

    private EventSourceActivity Start<T>(
      string eventName,
      ref EventSourceOptions options,
      ref T data)
    {
      if (this.state != EventSourceActivity.State.Started)
        throw new InvalidOperationException();
      if (!this.eventSource.IsEnabled())
        return this;
      EventSourceActivity eventSourceActivity = new EventSourceActivity(this.eventSource);
      if (!this.eventSource.IsEnabled(options.Level, options.Keywords))
      {
        Guid id = this.Id;
        eventSourceActivity.activityId = Guid.NewGuid();
        eventSourceActivity.startStopOptions = options;
        eventSourceActivity.eventName = eventName;
        eventSourceActivity.startStopOptions.Opcode = EventOpcode.Start;
        this.eventSource.Write<T>(eventName, ref eventSourceActivity.startStopOptions, ref eventSourceActivity.activityId, ref id, ref data);
      }
      else
        eventSourceActivity.activityId = this.Id;
      return eventSourceActivity;
    }

    private void Write<T>(
      EventSource eventSource,
      string eventName,
      ref EventSourceOptions options,
      ref T data)
    {
      if (this.state != EventSourceActivity.State.Started)
        throw new InvalidOperationException();
      if (eventName == null)
        throw new ArgumentNullException();
      eventSource.Write<T>(eventName, ref options, ref this.activityId, ref EventSourceActivity.s_empty, ref data);
    }

    private void Stop<T>(string eventName, ref T data)
    {
      if (this.state != EventSourceActivity.State.Started)
        throw new InvalidOperationException();
      if (!this.StartEventWasFired)
        return;
      this.state = EventSourceActivity.State.Stopped;
      if (eventName == null)
      {
        eventName = this.eventName;
        if (eventName.EndsWith("Start"))
          eventName = eventName.Substring(0, eventName.Length - 5);
        eventName += nameof (Stop);
      }
      this.startStopOptions.Opcode = EventOpcode.Stop;
      this.eventSource.Write<T>(eventName, ref this.startStopOptions, ref this.activityId, ref EventSourceActivity.s_empty, ref data);
    }

    private bool StartEventWasFired => this.eventName != null;

    private enum State
    {
      Started,
      Stopped,
    }
  }
}
