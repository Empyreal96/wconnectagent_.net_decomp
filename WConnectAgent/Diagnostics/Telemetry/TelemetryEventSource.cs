// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Telemetry.TelemetryEventSource
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Diagnostics.Tracing;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Diagnostics.Telemetry
{
  internal class TelemetryEventSource : EventSource
  {
    public const EventKeywords Reserved44Keyword = (EventKeywords) 17592186044416;
    public const EventKeywords TelemetryKeyword = (EventKeywords) 35184372088832;
    public const EventKeywords MeasuresKeyword = (EventKeywords) 70368744177664;
    public const EventKeywords CriticalDataKeyword = (EventKeywords) 140737488355328;
    public const EventTags CoreData = (EventTags) 524288;
    public const EventTags InjectXToken = (EventTags) 1048576;
    public const EventTags RealtimeLatency = (EventTags) 2097152;
    public const EventTags NormalLatency = (EventTags) 4194304;
    public const EventTags CriticalPersistence = (EventTags) 8388608;
    public const EventTags NormalPersistence = (EventTags) 16777216;
    public const EventTags DropPii = (EventTags) 33554432;
    public const EventTags HashPii = (EventTags) 67108864;
    public const EventTags MarkPii = (EventTags) 134217728;
    public const EventFieldTags DropPiiField = (EventFieldTags) 67108864;
    public const EventFieldTags HashPiiField = (EventFieldTags) 134217728;
    private static readonly string[] telemetryTraits = new string[2]
    {
      "ETW_GROUP",
      "{4f50731a-89cf-4782-b3e0-dce8c90476ba}"
    };

    [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
    public TelemetryEventSource(string eventSourceName)
      : base(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
    protected TelemetryEventSource()
      : base(EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
    public static EventSourceOptions TelemetryOptions() => new EventSourceOptions()
    {
      Keywords = (EventKeywords) 35184372088832
    };

    [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
    public static EventSourceOptions MeasuresOptions() => new EventSourceOptions()
    {
      Keywords = (EventKeywords) 70368744177664
    };

    [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
    public static EventSourceOptions CriticalDataOptions() => new EventSourceOptions()
    {
      Keywords = (EventKeywords) 140737488355328
    };
  }
}
