// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventSource
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using Microsoft.Reflection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.Diagnostics.Tracing
{
  public class EventSource : IDisposable
  {
    internal const string s_ActivityStartSuffix = "Start";
    internal const string s_ActivityStopSuffix = "Stop";
    private string m_name;
    internal int m_id;
    private Guid m_guid;
    internal volatile EventSource.EventMetadata[] m_eventData;
    private volatile byte[] m_rawManifest;
    private EventSourceSettings m_config;
    private bool m_eventSourceEnabled;
    internal EventLevel m_level;
    internal EventKeywords m_matchAnyKeyword;
    internal volatile EventDispatcher m_Dispatchers;
    private volatile EventSource.OverideEventProvider m_provider;
    private bool m_completelyInited;
    private Exception m_constructionException;
    private byte m_outOfBandMessageCount;
    private EventCommandEventArgs m_deferredCommands;
    private string[] m_traits;
    internal static uint s_currentPid;
    [ThreadStatic]
    private static byte m_EventSourceExceptionRecurenceCount = 0;
    internal volatile ulong[] m_channelData;
    private ActivityTracker m_activityTracker;
    private static readonly byte[] namespaceBytes = new byte[16]
    {
      (byte) 72,
      (byte) 44,
      (byte) 45,
      (byte) 178,
      (byte) 195,
      (byte) 144,
      (byte) 71,
      (byte) 200,
      (byte) 135,
      (byte) 248,
      (byte) 26,
      (byte) 21,
      (byte) 191,
      (byte) 193,
      (byte) 48,
      (byte) 251
    };
    private byte[] providerMetadata;

    public string Name => this.m_name;

    public Guid Guid => this.m_guid;

    public bool IsEnabled() => this.m_eventSourceEnabled;

    public bool IsEnabled(EventLevel level, EventKeywords keywords) => this.IsEnabled(level, keywords, EventChannel.None);

    public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel) => this.m_eventSourceEnabled && this.IsEnabledCommon(this.m_eventSourceEnabled, this.m_level, this.m_matchAnyKeyword, level, keywords, channel);

    public EventSourceSettings Settings => this.m_config;

    public static Guid GetGuid(Type eventSourceType)
    {
      EventSourceAttribute eventSourceAttribute = (object) eventSourceType != null ? (EventSourceAttribute) EventSource.GetCustomAttributeHelper(eventSourceType, typeof (EventSourceAttribute), EventManifestOptions.None) : throw new ArgumentNullException(nameof (eventSourceType));
      string name = eventSourceType.Name;
      if (eventSourceAttribute != null)
      {
        if (eventSourceAttribute.Guid != null)
        {
          Guid result = Guid.Empty;
          if (Guid.TryParse(eventSourceAttribute.Guid, out result))
            return result;
        }
        if (eventSourceAttribute.Name != null)
          name = eventSourceAttribute.Name;
      }
      return name != null ? EventSource.GenerateGuidFromName(name.ToUpperInvariant()) : throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("Argument_InvalidTypeName"), nameof (eventSourceType));
    }

    public static string GetName(Type eventSourceType) => EventSource.GetName(eventSourceType, EventManifestOptions.None);

    public static string GenerateManifest(
      Type eventSourceType,
      string assemblyPathToIncludeInManifest)
    {
      return EventSource.GenerateManifest(eventSourceType, assemblyPathToIncludeInManifest, EventManifestOptions.None);
    }

    public static string GenerateManifest(
      Type eventSourceType,
      string assemblyPathToIncludeInManifest,
      EventManifestOptions flags)
    {
      if ((object) eventSourceType == null)
        throw new ArgumentNullException(nameof (eventSourceType));
      byte[] manifestAndDescriptors = EventSource.CreateManifestAndDescriptors(eventSourceType, assemblyPathToIncludeInManifest, (EventSource) null, flags);
      return manifestAndDescriptors != null ? Encoding.UTF8.GetString(manifestAndDescriptors, 0, manifestAndDescriptors.Length) : (string) null;
    }

    public static IEnumerable<EventSource> GetSources()
    {
      List<EventSource> eventSourceList = new List<EventSource>();
      lock (EventListener.EventListenersLock)
      {
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          if (eventSource.Target is EventSource target2 && !target2.IsDisposed)
            eventSourceList.Add(target2);
        }
      }
      return (IEnumerable<EventSource>) eventSourceList;
    }

    public static void SendCommand(
      EventSource eventSource,
      EventCommand command,
      IDictionary<string, string> commandArguments)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      if (command <= EventCommand.Update && command != EventCommand.SendManifest)
        throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_InvalidCommand"), nameof (command));
      eventSource.SendCommand((EventListener) null, 0, 0, command, true, EventLevel.LogAlways, EventKeywords.None, commandArguments);
    }

    [SecuritySafeCritical]
    public static void SetCurrentThreadActivityId(Guid activityId)
    {
      UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_SET_ID, ref activityId);
      if (TplEtwProvider.Log == null)
        return;
      TplEtwProvider.Log.SetActivityId(activityId);
    }

    [SecuritySafeCritical]
    public static void SetCurrentThreadActivityId(
      Guid activityId,
      out Guid oldActivityThatWillContinue)
    {
      oldActivityThatWillContinue = activityId;
      UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_SET_ID, ref oldActivityThatWillContinue);
      if (TplEtwProvider.Log == null)
        return;
      TplEtwProvider.Log.SetActivityId(activityId);
    }

    public static Guid CurrentThreadActivityId
    {
      [SecuritySafeCritical] get
      {
        Guid ActivityId = new Guid();
        UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_ID, ref ActivityId);
        return ActivityId;
      }
    }

    public Exception ConstructionException => this.m_constructionException;

    public string GetTrait(string key)
    {
      if (this.m_traits != null)
      {
        for (int index = 0; index < this.m_traits.Length - 1; index += 2)
        {
          if (this.m_traits[index] == key)
            return this.m_traits[index + 1];
        }
      }
      return (string) null;
    }

    public override string ToString() => Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_ToString", (object) this.Name, (object) this.Guid);

    protected EventSource()
      : this(EventSourceSettings.EtwManifestEventFormat)
    {
    }

    protected EventSource(bool throwOnEventWriteErrors)
      : this((EventSourceSettings) (4 | (throwOnEventWriteErrors ? 1 : 0)))
    {
    }

    protected EventSource(EventSourceSettings settings)
      : this(settings, (string[]) null)
    {
    }

    protected EventSource(EventSourceSettings settings, params string[] traits)
    {
      this.m_config = this.ValidateSettings(settings);
      Type type = this.GetType();
      this.Initialize(EventSource.GetGuid(type), EventSource.GetName(type), traits);
    }

    protected virtual void OnEventCommand(EventCommandEventArgs command)
    {
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId) => this.WriteEventCore(eventId, 0, (EventSource.EventData*) null);

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, int arg1)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[1];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 4;
      this.WriteEventCore(eventId, 1, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, int arg1, int arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[2];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 4;
      data[1].DataPointer = (IntPtr) (void*) &arg2;
      data[1].Size = 4;
      this.WriteEventCore(eventId, 2, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, int arg1, int arg2, int arg3)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 4;
      data[1].DataPointer = (IntPtr) (void*) &arg2;
      data[1].Size = 4;
      data[2].DataPointer = (IntPtr) (void*) &arg3;
      data[2].Size = 4;
      this.WriteEventCore(eventId, 3, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, long arg1)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[1];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 8;
      this.WriteEventCore(eventId, 1, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, long arg1, long arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[2];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 8;
      data[1].DataPointer = (IntPtr) (void*) &arg2;
      data[1].Size = 8;
      this.WriteEventCore(eventId, 2, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, long arg1, long arg2, long arg3)
    {
      if (!this.m_eventSourceEnabled)
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->DataPointer = (IntPtr) (void*) &arg1;
      data->Size = 8;
      data[1].DataPointer = (IntPtr) (void*) &arg2;
      data[1].Size = 8;
      data[2].DataPointer = (IntPtr) (void*) &arg3;
      data[2].Size = 8;
      this.WriteEventCore(eventId, 3, data);
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      fixed (char* chPtr = arg1)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[1];
        data->DataPointer = (IntPtr) (void*) chPtr;
        data->Size = (arg1.Length + 1) * 2;
        this.WriteEventCore(eventId, 1, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1, string arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      if (arg2 == null)
        arg2 = "";
      fixed (char* chPtr1 = arg1)
        fixed (char* chPtr2 = arg2)
        {
          EventSource.EventData* data = stackalloc EventSource.EventData[2];
          data->DataPointer = (IntPtr) (void*) chPtr1;
          data->Size = (arg1.Length + 1) * 2;
          data[1].DataPointer = (IntPtr) (void*) chPtr2;
          data[1].Size = (arg2.Length + 1) * 2;
          this.WriteEventCore(eventId, 2, data);
        }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      if (arg2 == null)
        arg2 = "";
      if (arg3 == null)
        arg3 = "";
      fixed (char* chPtr1 = arg1)
        fixed (char* chPtr2 = arg2)
          fixed (char* chPtr3 = arg3)
          {
            EventSource.EventData* data = stackalloc EventSource.EventData[3];
            data->DataPointer = (IntPtr) (void*) chPtr1;
            data->Size = (arg1.Length + 1) * 2;
            data[1].DataPointer = (IntPtr) (void*) chPtr2;
            data[1].Size = (arg2.Length + 1) * 2;
            data[2].DataPointer = (IntPtr) (void*) chPtr3;
            data[2].Size = (arg3.Length + 1) * 2;
            this.WriteEventCore(eventId, 3, data);
          }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1, int arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      fixed (char* chPtr = arg1)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[2];
        data->DataPointer = (IntPtr) (void*) chPtr;
        data->Size = (arg1.Length + 1) * 2;
        data[1].DataPointer = (IntPtr) (void*) &arg2;
        data[1].Size = 4;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1, int arg2, int arg3)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      fixed (char* chPtr = arg1)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[3];
        data->DataPointer = (IntPtr) (void*) chPtr;
        data->Size = (arg1.Length + 1) * 2;
        data[1].DataPointer = (IntPtr) (void*) &arg2;
        data[1].Size = 4;
        data[2].DataPointer = (IntPtr) (void*) &arg3;
        data[2].Size = 4;
        this.WriteEventCore(eventId, 3, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = "";
      fixed (char* chPtr = arg1)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[2];
        data->DataPointer = (IntPtr) (void*) chPtr;
        data->Size = (arg1.Length + 1) * 2;
        data[1].DataPointer = (IntPtr) (void*) &arg2;
        data[1].Size = 8;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, long arg1, string arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg2 == null)
        arg2 = "";
      fixed (char* chPtr = arg2)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[2];
        data->DataPointer = (IntPtr) (void*) &arg1;
        data->Size = 8;
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        data[1].Size = (arg2.Length + 1) * 2;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, int arg1, string arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg2 == null)
        arg2 = "";
      fixed (char* chPtr = arg2)
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[2];
        data->DataPointer = (IntPtr) (void*) &arg1;
        data->Size = 4;
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        data[1].Size = (arg2.Length + 1) * 2;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, byte[] arg1)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg1 == null)
        arg1 = new byte[0];
      int length = arg1.Length;
      fixed (byte* numPtr = &arg1[0])
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[2];
        data->DataPointer = (IntPtr) (void*) &length;
        data->Size = 4;
        data[1].DataPointer = (IntPtr) (void*) numPtr;
        data[1].Size = length;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, long arg1, byte[] arg2)
    {
      if (!this.m_eventSourceEnabled)
        return;
      if (arg2 == null)
        arg2 = new byte[0];
      int length = arg2.Length;
      fixed (byte* numPtr = &arg2[0])
      {
        EventSource.EventData* data = stackalloc EventSource.EventData[3];
        data->DataPointer = (IntPtr) (void*) &arg1;
        data->Size = 8;
        data[1].DataPointer = (IntPtr) (void*) &length;
        data[1].Size = 4;
        data[2].DataPointer = (IntPtr) (void*) numPtr;
        data[2].Size = length;
        this.WriteEventCore(eventId, 3, data);
      }
    }

    [SecurityCritical]
    [CLSCompliant(false)]
    protected unsafe void WriteEventCore(
      int eventId,
      int eventDataCount,
      EventSource.EventData* data)
    {
      this.WriteEventWithRelatedActivityIdCore(eventId, (Guid*) null, eventDataCount, data);
    }

    [CLSCompliant(false)]
    [SecurityCritical]
    protected unsafe void WriteEventWithRelatedActivityIdCore(
      int eventId,
      Guid* relatedActivityId,
      int eventDataCount,
      EventSource.EventData* data)
    {
      if (!this.m_eventSourceEnabled)
        return;
      try
      {
        if ((IntPtr) relatedActivityId != IntPtr.Zero)
          this.ValidateEventOpcodeForTransfer(ref this.m_eventData[eventId]);
        if (this.m_eventData[eventId].EnabledForETW)
        {
          EventOpcode opcode = (EventOpcode) this.m_eventData[eventId].Descriptor.Opcode;
          EventActivityOptions activityOptions = this.m_eventData[eventId].ActivityOptions;
          Guid* activityID = (Guid*) null;
          Guid empty1 = Guid.Empty;
          Guid empty2 = Guid.Empty;
          if (opcode != EventOpcode.Info && (IntPtr) relatedActivityId == IntPtr.Zero && (activityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
          {
            switch (opcode)
            {
              case EventOpcode.Start:
                this.m_activityTracker.OnStart(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty1, ref empty2, this.m_eventData[eventId].ActivityOptions);
                break;
              case EventOpcode.Stop:
                this.m_activityTracker.OnStop(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty1);
                break;
            }
            if (empty1 != Guid.Empty)
              activityID = &empty1;
            if (empty2 != Guid.Empty)
              relatedActivityId = &empty2;
          }
          if (!this.SelfDescribingEvents)
          {
            if (!this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, activityID, relatedActivityId, eventDataCount, (IntPtr) (void*) data))
              this.ThrowEventSourceException();
          }
          else
          {
            TraceLoggingEventTypes eventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
            if (eventTypes == null)
            {
              eventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, this.m_eventData[eventId].Tags, this.m_eventData[eventId].Parameters);
              Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, eventTypes, (TraceLoggingEventTypes) null);
            }
            EventSourceOptions options = new EventSourceOptions()
            {
              Keywords = (EventKeywords) this.m_eventData[eventId].Descriptor.Keywords,
              Level = (EventLevel) this.m_eventData[eventId].Descriptor.Level,
              Opcode = (EventOpcode) this.m_eventData[eventId].Descriptor.Opcode
            };
            this.WriteMultiMerge(this.m_eventData[eventId].Name, ref options, eventTypes, activityID, relatedActivityId, data);
          }
        }
        if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
          return;
        this.WriteToAllListeners(eventId, relatedActivityId, eventDataCount, data);
      }
      catch (Exception ex)
      {
        if (ex is EventSourceException)
          throw;
        else
          this.ThrowEventSourceException(ex);
      }
    }

    [SecuritySafeCritical]
    protected unsafe void WriteEvent(int eventId, params object[] args) => this.WriteEventVarargs(eventId, (Guid*) null, args);

    [SecuritySafeCritical]
    protected unsafe void WriteEventWithRelatedActivityId(
      int eventId,
      Guid relatedActivityId,
      params object[] args)
    {
      this.WriteEventVarargs(eventId, &relatedActivityId, args);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.m_eventSourceEnabled)
        {
          try
          {
            this.SendManifest(this.m_rawManifest);
          }
          catch (Exception ex)
          {
          }
          this.m_eventSourceEnabled = false;
        }
        if (this.m_provider != null)
        {
          this.m_provider.Dispose();
          this.m_provider = (EventSource.OverideEventProvider) null;
        }
      }
      this.m_eventSourceEnabled = false;
    }

    ~EventSource() => this.Dispose(false);

    [SecurityCritical]
    private unsafe void WriteEventRaw(
      ref EventDescriptor eventDescriptor,
      Guid* activityID,
      Guid* relatedActivityID,
      int dataCount,
      IntPtr data)
    {
      if (this.m_provider == null)
      {
        this.ThrowEventSourceException();
      }
      else
      {
        if (this.m_provider.WriteEventRaw(ref eventDescriptor, activityID, relatedActivityID, dataCount, data))
          return;
        this.ThrowEventSourceException();
      }
    }

    internal EventSource(Guid eventSourceGuid, string eventSourceName)
      : this(eventSourceGuid, eventSourceName, EventSourceSettings.EtwManifestEventFormat)
    {
    }

    internal EventSource(
      Guid eventSourceGuid,
      string eventSourceName,
      EventSourceSettings settings,
      string[] traits = null)
    {
      this.m_config = this.ValidateSettings(settings);
      this.Initialize(eventSourceGuid, eventSourceName, traits);
    }

    [SecuritySafeCritical]
    private unsafe void Initialize(Guid eventSourceGuid, string eventSourceName, string[] traits)
    {
      try
      {
        this.m_traits = traits;
        if (this.m_traits != null && this.m_traits.Length % 2 != 0)
          throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("TraitEven"), nameof (traits));
        if (eventSourceGuid == Guid.Empty)
          throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NeedGuid"));
        this.m_name = eventSourceName != null ? eventSourceName : throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NeedName"));
        this.m_guid = eventSourceGuid;
        this.m_activityTracker = ActivityTracker.Instance;
        this.InitializeProviderMetadata();
        EventSource.OverideEventProvider overideEventProvider = new EventSource.OverideEventProvider(this);
        overideEventProvider.Register(eventSourceGuid);
        EventListener.AddEventSource(this);
        this.m_provider = overideEventProvider;
        fixed (byte* numPtr = this.providerMetadata)
          this.m_provider.SetInformation(UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS.SetTraits, (void*) numPtr, this.providerMetadata.Length);
        this.m_completelyInited = true;
      }
      catch (Exception ex)
      {
        if (this.m_constructionException == null)
          this.m_constructionException = ex;
        this.ReportOutOfBandMessage("ERROR: Exception during construction of EventSource " + this.Name + ": " + ex.Message, true);
      }
      lock (EventListener.EventListenersLock)
      {
        for (; this.m_deferredCommands != null; this.m_deferredCommands = this.m_deferredCommands.nextCommand)
          this.DoCommand(this.m_deferredCommands);
      }
    }

    private static string GetName(Type eventSourceType, EventManifestOptions flags)
    {
      EventSourceAttribute eventSourceAttribute = (object) eventSourceType != null ? (EventSourceAttribute) EventSource.GetCustomAttributeHelper(eventSourceType, typeof (EventSourceAttribute), flags) : throw new ArgumentNullException(nameof (eventSourceType));
      return eventSourceAttribute != null && eventSourceAttribute.Name != null ? eventSourceAttribute.Name : eventSourceType.Name;
    }

    private static Guid GenerateGuidFromName(string name)
    {
      byte[] bytes = Encoding.BigEndianUnicode.GetBytes(name);
      EventSource.Sha1ForNonSecretPurposes nonSecretPurposes = new EventSource.Sha1ForNonSecretPurposes();
      nonSecretPurposes.Start();
      nonSecretPurposes.Append(EventSource.namespaceBytes);
      nonSecretPurposes.Append(bytes);
      Array.Resize<byte>(ref bytes, 16);
      nonSecretPurposes.Finish(bytes);
      bytes[7] = (byte) ((int) bytes[7] & 15 | 80);
      return new Guid(bytes);
    }

    [SecurityCritical]
    private unsafe object DecodeObject(
      int eventId,
      int parameterId,
      ref EventSource.EventData* data)
    {
      IntPtr dataPointer1 = data->DataPointer;
      ++data;
      for (Type type = this.m_eventData[eventId].Parameters[parameterId].ParameterType; (object) type != (object) typeof (IntPtr); type = Enum.GetUnderlyingType(type))
      {
        if ((object) type == (object) typeof (int))
          return (object) *(int*) (void*) dataPointer1;
        if ((object) type == (object) typeof (uint))
          return (object) *(uint*) (void*) dataPointer1;
        if ((object) type == (object) typeof (long))
          return (object) *(long*) (void*) dataPointer1;
        if ((object) type == (object) typeof (ulong))
          return (object) (ulong) *(long*) (void*) dataPointer1;
        if ((object) type == (object) typeof (byte))
          return (object) *(byte*) (void*) dataPointer1;
        if ((object) type == (object) typeof (sbyte))
          return (object) *(sbyte*) (void*) dataPointer1;
        if ((object) type == (object) typeof (short))
          return (object) *(short*) (void*) dataPointer1;
        if ((object) type == (object) typeof (ushort))
          return (object) *(ushort*) (void*) dataPointer1;
        if ((object) type == (object) typeof (float))
          return (object) *(float*) (void*) dataPointer1;
        if ((object) type == (object) typeof (double))
          return (object) *(double*) (void*) dataPointer1;
        if ((object) type == (object) typeof (Decimal))
          return (object) *(Decimal*) (void*) dataPointer1;
        if ((object) type == (object) typeof (bool))
          return *(int*) (void*) dataPointer1 == 1 ? (object) true : (object) false;
        if ((object) type == (object) typeof (Guid))
          return (object) *(Guid*) (void*) dataPointer1;
        if ((object) type == (object) typeof (char))
          return (object) (char) *(ushort*) (void*) dataPointer1;
        if ((object) type == (object) typeof (DateTime))
          return (object) DateTime.FromFileTimeUtc(*(long*) (void*) dataPointer1);
        if ((object) type == (object) typeof (byte[]))
        {
          int length = *(int*) (void*) dataPointer1;
          byte[] numArray = new byte[length];
          IntPtr dataPointer2 = data->DataPointer;
          ++data;
          for (int index = 0; index < length; ++index)
            numArray[index] = *(byte*) (void*) dataPointer2;
          return (object) numArray;
        }
        if ((object) type == (object) typeof (byte*))
          return (object) null;
        if (!ReflectionExtensions.IsEnum(type))
          return (object) Marshal.PtrToStringUni(dataPointer1);
      }
      return (object) *(IntPtr*) (void*) dataPointer1;
    }

    private EventDispatcher GetDispatcher(EventListener listener)
    {
      EventDispatcher eventDispatcher = this.m_Dispatchers;
      while (eventDispatcher != null && eventDispatcher.m_Listener != listener)
        eventDispatcher = eventDispatcher.m_Next;
      return eventDispatcher;
    }

    [SecurityCritical]
    private unsafe void WriteEventVarargs(int eventId, Guid* childActivityID, object[] args)
    {
      if (!this.m_eventSourceEnabled)
        return;
      try
      {
        if ((IntPtr) childActivityID != IntPtr.Zero)
          this.ValidateEventOpcodeForTransfer(ref this.m_eventData[eventId]);
        if (this.m_eventData[eventId].EnabledForETW)
        {
          Guid* activityID = (Guid*) null;
          Guid empty1 = Guid.Empty;
          Guid empty2 = Guid.Empty;
          EventOpcode opcode = (EventOpcode) this.m_eventData[eventId].Descriptor.Opcode;
          EventActivityOptions activityOptions = this.m_eventData[eventId].ActivityOptions;
          if ((IntPtr) childActivityID == IntPtr.Zero && (activityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
          {
            switch (opcode)
            {
              case EventOpcode.Start:
                this.m_activityTracker.OnStart(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty1, ref empty2, this.m_eventData[eventId].ActivityOptions);
                break;
              case EventOpcode.Stop:
                this.m_activityTracker.OnStop(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty1);
                break;
            }
            if (empty1 != Guid.Empty)
              activityID = &empty1;
            if (empty2 != Guid.Empty)
              childActivityID = &empty2;
          }
          if (!this.SelfDescribingEvents)
          {
            if (!this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, activityID, childActivityID, args))
              this.ThrowEventSourceException();
          }
          else
          {
            TraceLoggingEventTypes eventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
            if (eventTypes == null)
            {
              eventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, EventTags.None, this.m_eventData[eventId].Parameters);
              Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, eventTypes, (TraceLoggingEventTypes) null);
            }
            EventSourceOptions options = new EventSourceOptions()
            {
              Keywords = (EventKeywords) this.m_eventData[eventId].Descriptor.Keywords,
              Level = (EventLevel) this.m_eventData[eventId].Descriptor.Level,
              Opcode = (EventOpcode) this.m_eventData[eventId].Descriptor.Opcode
            };
            this.WriteMultiMerge(this.m_eventData[eventId].Name, ref options, eventTypes, activityID, childActivityID, args);
          }
        }
        if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
          return;
        object[] objArray = this.SerializeEventArgs(eventId, args);
        this.WriteToAllListeners(eventId, childActivityID, objArray);
      }
      catch (Exception ex)
      {
        if (ex is EventSourceException)
          throw;
        else
          this.ThrowEventSourceException(ex);
      }
    }

    [SecurityCritical]
    private object[] SerializeEventArgs(int eventId, object[] args)
    {
      TraceLoggingEventTypes loggingEventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
      if (loggingEventTypes == null)
      {
        loggingEventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, EventTags.None, this.m_eventData[eventId].Parameters);
        Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, loggingEventTypes, (TraceLoggingEventTypes) null);
      }
      object[] objArray = new object[loggingEventTypes.typeInfos.Length];
      for (int index = 0; index < loggingEventTypes.typeInfos.Length; ++index)
        objArray[index] = loggingEventTypes.typeInfos[index].GetData(args[index]);
      return objArray;
    }

    [SecurityCritical]
    private unsafe void WriteToAllListeners(
      int eventId,
      Guid* childActivityID,
      int eventDataCount,
      EventSource.EventData* data)
    {
      int val1 = this.m_eventData[eventId].Parameters.Length;
      if (eventDataCount != val1)
      {
        this.ReportOutOfBandMessage(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventParametersMismatch", (object) eventId, (object) eventDataCount, (object) val1), true);
        val1 = Math.Min(val1, eventDataCount);
      }
      object[] objArray = new object[val1];
      EventSource.EventData* data1 = data;
      for (int parameterId = 0; parameterId < val1; ++parameterId)
        objArray[parameterId] = this.DecodeObject(eventId, parameterId, ref data1);
      this.WriteToAllListeners(eventId, childActivityID, objArray);
    }

    [SecurityCritical]
    private unsafe void WriteToAllListeners(
      int eventId,
      Guid* childActivityID,
      params object[] args)
    {
      EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
      eventCallbackArgs.EventId = eventId;
      if ((IntPtr) childActivityID != IntPtr.Zero)
        eventCallbackArgs.RelatedActivityId = *childActivityID;
      eventCallbackArgs.EventName = this.m_eventData[eventId].Name;
      eventCallbackArgs.Message = this.m_eventData[eventId].Message;
      eventCallbackArgs.Payload = new ReadOnlyCollection<object>((IList<object>) args);
      this.DisptachToAllListeners(eventId, childActivityID, eventCallbackArgs);
    }

    [SecurityCritical]
    private unsafe void DisptachToAllListeners(
      int eventId,
      Guid* childActivityID,
      EventWrittenEventArgs eventCallbackArgs)
    {
      Exception innerException = (Exception) null;
      for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
      {
        if (eventId != -1)
        {
          if (!eventDispatcher.m_EventEnabled[eventId])
            continue;
        }
        try
        {
          eventDispatcher.m_Listener.OnEventWritten(eventCallbackArgs);
        }
        catch (Exception ex)
        {
          this.ReportOutOfBandMessage("ERROR: Exception during EventSource.OnEventWritten: " + ex.Message, false);
          innerException = ex;
        }
      }
      if (innerException != null)
        throw new EventSourceException(innerException);
    }

    [SecuritySafeCritical]
    private unsafe void WriteEventString(EventLevel level, long keywords, string msgString)
    {
      if (this.m_provider == null)
        return;
      string str = "EventSourceMessage";
      if (this.SelfDescribingEvents)
      {
        EventSourceOptions options = new EventSourceOptions()
        {
          Keywords = (EventKeywords) keywords,
          Level = level
        };
        var data = new{ message = msgString };
        TraceLoggingEventTypes eventTypes = new TraceLoggingEventTypes(str, EventTags.None, new Type[1]
        {
          data.GetType()
        });
        this.WriteMultiMergeInner(str, ref options, eventTypes, (Guid*) IntPtr.Zero, (Guid*) IntPtr.Zero, (object) data);
      }
      else
      {
        if (this.m_rawManifest == null && this.m_outOfBandMessageCount == (byte) 1)
        {
          ManifestBuilder manifestBuilder = new ManifestBuilder(this.Name, this.Guid, this.Name, (ResourceManager) null, EventManifestOptions.None);
          manifestBuilder.StartEvent(str, new EventAttribute(0)
          {
            Level = EventLevel.LogAlways,
            Task = (EventTask) 65534
          });
          manifestBuilder.AddEventParameter(typeof (string), "message");
          manifestBuilder.EndEvent();
          this.SendManifest(manifestBuilder.CreateManifest());
        }
        fixed (char* chPtr = msgString)
        {
          EventDescriptor eventDescriptor = new EventDescriptor(0, (byte) 0, (byte) 0, (byte) level, (byte) 0, 0, keywords);
          this.m_provider.WriteEvent(ref eventDescriptor, (Guid*) null, (Guid*) null, 1, (IntPtr) (void*) &new EventProvider.EventData()
          {
            Ptr = (ulong) chPtr,
            Size = (uint) (2 * (msgString.Length + 1)),
            Reserved = 0U
          });
        }
      }
    }

    private void WriteStringToAllListeners(string eventName, string msg)
    {
      EventWrittenEventArgs eventData = new EventWrittenEventArgs(this);
      eventData.EventId = 0;
      eventData.Message = msg;
      eventData.Payload = new ReadOnlyCollection<object>((IList<object>) new List<object>()
      {
        (object) msg
      });
      eventData.PayloadNames = new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "message"
      });
      eventData.EventName = eventName;
      for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
      {
        bool flag = false;
        if (eventDispatcher.m_EventEnabled == null)
        {
          flag = true;
        }
        else
        {
          for (int index = 0; index < eventDispatcher.m_EventEnabled.Length; ++index)
          {
            if (eventDispatcher.m_EventEnabled[index])
            {
              flag = true;
              break;
            }
          }
        }
        try
        {
          if (flag)
            eventDispatcher.m_Listener.OnEventWritten(eventData);
        }
        catch
        {
        }
      }
    }

    private bool IsEnabledByDefault(
      int eventNum,
      bool enable,
      EventLevel currentLevel,
      EventKeywords currentMatchAnyKeyword)
    {
      if (!enable)
        return false;
      EventLevel level = (EventLevel) this.m_eventData[eventNum].Descriptor.Level;
      EventKeywords eventKeywords = (EventKeywords) (this.m_eventData[eventNum].Descriptor.Keywords & ~(long) SessionMask.All.ToEventKeywords());
      EventChannel channel = (EventChannel) this.m_eventData[eventNum].Descriptor.Channel;
      return this.IsEnabledCommon(enable, currentLevel, currentMatchAnyKeyword, level, eventKeywords, channel);
    }

    private bool IsEnabledCommon(
      bool enabled,
      EventLevel currentLevel,
      EventKeywords currentMatchAnyKeyword,
      EventLevel eventLevel,
      EventKeywords eventKeywords,
      EventChannel eventChannel)
    {
      if (!enabled || currentLevel != EventLevel.LogAlways && currentLevel < eventLevel)
        return false;
      if (currentMatchAnyKeyword != EventKeywords.None && eventKeywords != EventKeywords.None)
      {
        if (eventChannel != EventChannel.None && this.m_channelData != null && (EventChannel) this.m_channelData.Length > eventChannel)
        {
          EventKeywords eventKeywords1 = (EventKeywords) this.m_channelData[(int) eventChannel] | eventKeywords;
          if (eventKeywords1 != EventKeywords.None && (eventKeywords1 & currentMatchAnyKeyword) == EventKeywords.None)
            return false;
        }
        else if ((eventKeywords & currentMatchAnyKeyword) == EventKeywords.None)
          return false;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowEventSourceException(Exception innerEx = null)
    {
      if (EventSource.m_EventSourceExceptionRecurenceCount > (byte) 0)
        return;
      try
      {
        ++EventSource.m_EventSourceExceptionRecurenceCount;
        switch (EventProvider.GetLastWriteEventError())
        {
          case EventProvider.WriteEventErrorCode.NoFreeBuffers:
            this.ReportOutOfBandMessage("EventSourceException: " + Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NoFreeBuffers"), true);
            if (!this.ThrowOnEventWriteErrors)
              break;
            throw new EventSourceException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NoFreeBuffers"), innerEx);
          case EventProvider.WriteEventErrorCode.EventTooBig:
            this.ReportOutOfBandMessage("EventSourceException: " + Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventTooBig"), true);
            if (!this.ThrowOnEventWriteErrors)
              break;
            throw new EventSourceException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventTooBig"), innerEx);
          case EventProvider.WriteEventErrorCode.NullInput:
            this.ReportOutOfBandMessage("EventSourceException: " + Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NullInput"), true);
            if (!this.ThrowOnEventWriteErrors)
              break;
            throw new EventSourceException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NullInput"), innerEx);
          case EventProvider.WriteEventErrorCode.TooManyArgs:
            this.ReportOutOfBandMessage("EventSourceException: " + Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TooManyArgs"), true);
            if (!this.ThrowOnEventWriteErrors)
              break;
            throw new EventSourceException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TooManyArgs"), innerEx);
          default:
            if (innerEx != null)
              this.ReportOutOfBandMessage("EventSourceException: " + (object) ((object) innerEx).GetType() + ":" + innerEx.Message, true);
            else
              this.ReportOutOfBandMessage("EventSourceException", true);
            if (!this.ThrowOnEventWriteErrors)
              break;
            throw new EventSourceException(innerEx);
        }
      }
      finally
      {
        --EventSource.m_EventSourceExceptionRecurenceCount;
      }
    }

    private void ValidateEventOpcodeForTransfer(ref EventSource.EventMetadata eventData)
    {
      if (eventData.Descriptor.Opcode == (byte) 9 || eventData.Descriptor.Opcode == (byte) 240)
        return;
      this.ThrowEventSourceException();
    }

    internal static EventOpcode GetOpcodeWithDefault(
      EventOpcode opcode,
      string eventName)
    {
      if (opcode == EventOpcode.Info && eventName != null)
      {
        if (eventName.EndsWith("Start"))
          return EventOpcode.Start;
        if (eventName.EndsWith("Stop"))
          return EventOpcode.Stop;
      }
      return opcode;
    }

    internal void SendCommand(
      EventListener listener,
      int perEventSourceSessionId,
      int etwSessionId,
      EventCommand command,
      bool enable,
      EventLevel level,
      EventKeywords matchAnyKeyword,
      IDictionary<string, string> commandArguments)
    {
      EventCommandEventArgs commandArgs = new EventCommandEventArgs(command, commandArguments, this, listener, perEventSourceSessionId, etwSessionId, enable, level, matchAnyKeyword);
      lock (EventListener.EventListenersLock)
      {
        if (this.m_completelyInited)
        {
          this.DoCommand(commandArgs);
        }
        else
        {
          commandArgs.nextCommand = this.m_deferredCommands;
          this.m_deferredCommands = commandArgs;
        }
      }
    }

    internal void DoCommand(EventCommandEventArgs commandArgs)
    {
      if (this.m_provider == null)
        return;
      this.m_outOfBandMessageCount = (byte) 0;
      if (commandArgs.perEventSourceSessionId > 0)
      {
        int eventSourceSessionId = commandArgs.perEventSourceSessionId;
      }
      try
      {
        this.EnsureDescriptorsInitialized();
        commandArgs.dispatcher = this.GetDispatcher(commandArgs.listener);
        if (commandArgs.dispatcher == null && commandArgs.listener != null)
          throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_ListenerNotFound"));
        if (commandArgs.Arguments == null)
          commandArgs.Arguments = (IDictionary<string, string>) new Dictionary<string, string>();
        if (commandArgs.Command == EventCommand.Update)
        {
          for (int index = 0; index < this.m_eventData.Length; ++index)
            this.EnableEventForDispatcher(commandArgs.dispatcher, index, this.IsEnabledByDefault(index, commandArgs.enable, commandArgs.level, commandArgs.matchAnyKeyword));
          if (commandArgs.enable)
          {
            if (!this.m_eventSourceEnabled)
            {
              this.m_level = commandArgs.level;
              this.m_matchAnyKeyword = commandArgs.matchAnyKeyword;
            }
            else
            {
              if (commandArgs.level > this.m_level)
                this.m_level = commandArgs.level;
              if (commandArgs.matchAnyKeyword == EventKeywords.None)
                this.m_matchAnyKeyword = EventKeywords.None;
              else if (this.m_matchAnyKeyword != EventKeywords.None)
                this.m_matchAnyKeyword |= commandArgs.matchAnyKeyword;
            }
          }
          bool flag1 = commandArgs.perEventSourceSessionId >= 0;
          if (commandArgs.perEventSourceSessionId == 0 && !commandArgs.enable)
            flag1 = false;
          if (commandArgs.listener == null)
          {
            if (!flag1)
              commandArgs.perEventSourceSessionId = -commandArgs.perEventSourceSessionId;
            --commandArgs.perEventSourceSessionId;
          }
          commandArgs.Command = flag1 ? EventCommand.Enable : EventCommand.Disable;
          if (flag1 && commandArgs.dispatcher == null && !this.SelfDescribingEvents)
            this.SendManifest(this.m_rawManifest);
          if (commandArgs.enable)
            this.m_eventSourceEnabled = true;
          this.OnEventCommand(commandArgs);
          if (commandArgs.enable)
            return;
          for (int index = 0; index < this.m_eventData.Length; ++index)
          {
            bool flag2 = false;
            for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
            {
              if (eventDispatcher.m_EventEnabled[index])
              {
                flag2 = true;
                break;
              }
            }
            this.m_eventData[index].EnabledForAnyListener = flag2;
          }
          if (this.AnyEventEnabled())
            return;
          this.m_level = EventLevel.LogAlways;
          this.m_matchAnyKeyword = EventKeywords.None;
          this.m_eventSourceEnabled = false;
        }
        else
        {
          if (commandArgs.Command == EventCommand.SendManifest && this.m_rawManifest != null)
            this.SendManifest(this.m_rawManifest);
          this.OnEventCommand(commandArgs);
        }
      }
      catch (Exception ex)
      {
        this.ReportOutOfBandMessage("ERROR: Exception in Command Processing for EventSource " + this.Name + ": " + ex.Message, true);
      }
    }

    internal bool EnableEventForDispatcher(EventDispatcher dispatcher, int eventId, bool value)
    {
      if (dispatcher == null)
      {
        if (eventId >= this.m_eventData.Length)
          return false;
        if (this.m_provider != null)
          this.m_eventData[eventId].EnabledForETW = value;
      }
      else
      {
        if (eventId >= dispatcher.m_EventEnabled.Length)
          return false;
        dispatcher.m_EventEnabled[eventId] = value;
        if (value)
          this.m_eventData[eventId].EnabledForAnyListener = true;
      }
      return true;
    }

    private bool AnyEventEnabled()
    {
      for (int index = 0; index < this.m_eventData.Length; ++index)
      {
        if (this.m_eventData[index].EnabledForETW || this.m_eventData[index].EnabledForAnyListener)
          return true;
      }
      return false;
    }

    private bool IsDisposed => this.m_provider == null || this.m_provider.m_disposed;

    [SecuritySafeCritical]
    private void EnsureDescriptorsInitialized()
    {
      if (this.m_eventData == null)
      {
        this.m_rawManifest = EventSource.CreateManifestAndDescriptors(this.GetType(), this.Name, this);
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          if (eventSource.Target is EventSource target3 && target3.Guid == this.m_guid && (!target3.IsDisposed && target3 != this))
            throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventSourceGuidInUse", (object) this.m_guid));
        }
        for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
        {
          if (eventDispatcher.m_EventEnabled == null)
            eventDispatcher.m_EventEnabled = new bool[this.m_eventData.Length];
        }
      }
      if (EventSource.s_currentPid != 0U)
        return;
      EventSource.s_currentPid = Win32Native.GetCurrentProcessId();
    }

    [SecuritySafeCritical]
    private unsafe bool SendManifest(byte[] rawManifest)
    {
      bool flag = true;
      if (rawManifest == null)
        return false;
      fixed (byte* numPtr = rawManifest)
      {
        EventDescriptor eventDescriptor = new EventDescriptor(65534, (byte) 1, (byte) 0, (byte) 0, (byte) 254, 65534, 72057594037927935L);
        ManifestEnvelope manifestEnvelope = new ManifestEnvelope();
        manifestEnvelope.Format = ManifestEnvelope.ManifestFormats.SimpleXmlFormat;
        manifestEnvelope.MajorVersion = (byte) 1;
        manifestEnvelope.MinorVersion = (byte) 0;
        manifestEnvelope.Magic = (byte) 91;
        int length = rawManifest.Length;
        manifestEnvelope.ChunkNumber = (ushort) 0;
        EventProvider.EventData* eventDataPtr = stackalloc EventProvider.EventData[2];
        eventDataPtr->Ptr = (ulong) &manifestEnvelope;
        eventDataPtr->Size = (uint) sizeof (ManifestEnvelope);
        eventDataPtr->Reserved = 0U;
        eventDataPtr[1].Ptr = (ulong) numPtr;
        eventDataPtr[1].Reserved = 0U;
        int val2 = 65280;
label_3:
        manifestEnvelope.TotalChunks = (ushort) ((length + (val2 - 1)) / val2);
        while (length > 0)
        {
          eventDataPtr[1].Size = (uint) Math.Min(length, val2);
          if (this.m_provider != null && !this.m_provider.WriteEvent(ref eventDescriptor, (Guid*) null, (Guid*) null, 2, (IntPtr) (void*) eventDataPtr))
          {
            if (EventProvider.GetLastWriteEventError() == EventProvider.WriteEventErrorCode.EventTooBig && manifestEnvelope.ChunkNumber == (ushort) 0 && val2 > 256)
            {
              val2 /= 2;
              goto label_3;
            }
            else
            {
              flag = false;
              if (this.ThrowOnEventWriteErrors)
              {
                this.ThrowEventSourceException();
                break;
              }
              break;
            }
          }
          else
          {
            length -= val2;
            eventDataPtr[1].Ptr += (ulong) (uint) val2;
            ++manifestEnvelope.ChunkNumber;
          }
        }
      }
      return flag;
    }

    internal static Attribute GetCustomAttributeHelper(
      Type type,
      Type attributeType,
      EventManifestOptions flags = EventManifestOptions.None)
    {
      return EventSource.GetCustomAttributeHelper((MemberInfo) type.GetTypeInfo(), attributeType, flags);
    }

    internal static Attribute GetCustomAttributeHelper(
      MemberInfo member,
      Type attributeType,
      EventManifestOptions flags = EventManifestOptions.None)
    {
      if (!ReflectionExtensions.ReflectionOnly(member.Module.Assembly) && (flags & EventManifestOptions.AllowEventSourceOverride) == EventManifestOptions.None)
      {
        Attribute attribute = (Attribute) null;
        using (IEnumerator<Attribute> enumerator = CustomAttributeExtensions.GetCustomAttributes(member, attributeType, false).GetEnumerator())
        {
          if (enumerator.MoveNext())
            attribute = enumerator.Current;
        }
        return attribute;
      }
      throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString(nameof (EventSource), (object) "EventSource_PCLPlatformNotSupportedReflection"));
    }

    private static bool AttributeTypeNamesMatch(Type attributeType, Type reflectedAttributeType)
    {
      if ((object) attributeType == (object) reflectedAttributeType || string.Equals(attributeType.FullName, reflectedAttributeType.FullName, StringComparison.Ordinal))
        return true;
      return string.Equals(attributeType.Name, reflectedAttributeType.Name, StringComparison.Ordinal) && attributeType.Namespace.EndsWith("Diagnostics.Tracing") && reflectedAttributeType.Namespace.EndsWith("Diagnostics.Tracing");
    }

    private static Type GetEventSourceBaseType(
      Type eventSourceType,
      bool allowEventSourceOverride,
      bool reflectionOnly)
    {
      if ((object) ReflectionExtensions.BaseType(eventSourceType) == null)
        return (Type) null;
      do
      {
        eventSourceType = ReflectionExtensions.BaseType(eventSourceType);
      }
      while ((object) eventSourceType != null && ReflectionExtensions.IsAbstract(eventSourceType));
      if ((object) eventSourceType != null)
      {
        if (!allowEventSourceOverride)
        {
          if (reflectionOnly && eventSourceType.FullName != typeof (EventSource).FullName || !reflectionOnly && (object) eventSourceType != (object) typeof (EventSource))
            return (Type) null;
        }
        else if (eventSourceType.Name != nameof (EventSource))
          return (Type) null;
      }
      return eventSourceType;
    }

    private static byte[] CreateManifestAndDescriptors(
      Type eventSourceType,
      string eventSourceDllName,
      EventSource source,
      EventManifestOptions flags = EventManifestOptions.None)
    {
      ManifestBuilder manifest = (ManifestBuilder) null;
      bool flag1 = source == null || !source.SelfDescribingEvents;
      Exception innerException = (Exception) null;
      byte[] numArray = (byte[]) null;
      if (ReflectionExtensions.IsAbstract(eventSourceType))
      {
        if ((flags & EventManifestOptions.Strict) == EventManifestOptions.None)
          return (byte[]) null;
      }
      try
      {
        MethodInfo[] methods = eventSourceType.GetMethods(Microsoft.Reflection.BindingFlags.DeclaredOnly | Microsoft.Reflection.BindingFlags.Instance | Microsoft.Reflection.BindingFlags.Public | Microsoft.Reflection.BindingFlags.NonPublic);
        int eventId = 1;
        EventSource.EventMetadata[] eventData = (EventSource.EventMetadata[]) null;
        Dictionary<string, string> eventsByName = (Dictionary<string, string>) null;
        if (source != null || (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
        {
          eventData = new EventSource.EventMetadata[methods.Length + 1];
          eventData[0].Name = "";
        }
        ResourceManager resources = (ResourceManager) null;
        EventSourceAttribute customAttributeHelper = (EventSourceAttribute) EventSource.GetCustomAttributeHelper(eventSourceType, typeof (EventSourceAttribute), flags);
        if (customAttributeHelper != null && customAttributeHelper.LocalizationResources != null)
          resources = new ResourceManager(customAttributeHelper.LocalizationResources, ReflectionExtensions.Assembly(eventSourceType));
        manifest = new ManifestBuilder(EventSource.GetName(eventSourceType, flags), EventSource.GetGuid(eventSourceType), eventSourceDllName, resources, flags);
        manifest.StartEvent("EventSourceMessage", new EventAttribute(0)
        {
          Level = EventLevel.LogAlways,
          Task = (EventTask) 65534
        });
        manifest.AddEventParameter(typeof (string), "message");
        manifest.EndEvent();
        if ((flags & EventManifestOptions.Strict) != EventManifestOptions.None)
        {
          if ((object) EventSource.GetEventSourceBaseType(eventSourceType, (flags & EventManifestOptions.AllowEventSourceOverride) != EventManifestOptions.None, ReflectionExtensions.ReflectionOnly(ReflectionExtensions.Assembly(eventSourceType))) == null)
            manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TypeMustDeriveFromEventSource"));
          if (!ReflectionExtensions.IsAbstract(eventSourceType) && !ReflectionExtensions.IsSealed(eventSourceType))
            manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TypeMustBeSealedOrAbstract"));
        }
        string[] strArray = new string[3]
        {
          "Keywords",
          "Tasks",
          "Opcodes"
        };
        foreach (string str in strArray)
        {
          Type nestedType = ReflectionExtensions.GetNestedType(eventSourceType, str);
          if ((object) nestedType != null)
          {
            if (ReflectionExtensions.IsAbstract(eventSourceType))
            {
              manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_AbstractMustNotDeclareKTOC", (object) nestedType.Name));
            }
            else
            {
              foreach (FieldInfo field in nestedType.GetFields(Microsoft.Reflection.BindingFlags.DeclaredOnly | Microsoft.Reflection.BindingFlags.Static | Microsoft.Reflection.BindingFlags.Public | Microsoft.Reflection.BindingFlags.NonPublic))
                EventSource.AddProviderEnumKind(manifest, field, str);
            }
          }
        }
        manifest.AddKeyword("Session3", 17592186044416UL);
        manifest.AddKeyword("Session2", 35184372088832UL);
        manifest.AddKeyword("Session1", 70368744177664UL);
        manifest.AddKeyword("Session0", 140737488355328UL);
        if (eventSourceType.Name != nameof (EventSource))
        {
          for (int index1 = 0; index1 < methods.Length; ++index1)
          {
            MethodInfo method = methods[index1];
            ParameterInfo[] parameters = method.GetParameters();
            EventAttribute eventAttribute = (EventAttribute) EventSource.GetCustomAttributeHelper((MemberInfo) method, typeof (EventAttribute), flags);
            if (!method.IsStatic)
            {
              if (ReflectionExtensions.IsAbstract(eventSourceType))
              {
                if (eventAttribute != null)
                  manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_AbstractMustNotDeclareEventMethods", (object) method.Name, (object) eventAttribute.EventId));
              }
              else
              {
                if (eventAttribute == null)
                {
                  if ((object) method.ReturnType == (object) typeof (void) && !method.IsVirtual && EventSource.GetCustomAttributeHelper((MemberInfo) method, typeof (NonEventAttribute), flags) == null)
                    eventAttribute = new EventAttribute(eventId);
                  else
                    continue;
                }
                else if (eventAttribute.EventId <= 0)
                {
                  manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NeedPositiveId", (object) method.Name), true);
                  continue;
                }
                if (method.Name.LastIndexOf('.') >= 0)
                  manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventMustNotBeExplicitImplementation", (object) method.Name, (object) eventAttribute.EventId));
                ++eventId;
                string name = method.Name;
                if (eventAttribute.Opcode == EventOpcode.Info)
                {
                  bool flag2 = eventAttribute.Task == EventTask.None;
                  if (flag2)
                    eventAttribute.Task = (EventTask) (65534 - eventAttribute.EventId);
                  if (!eventAttribute.IsOpcodeSet)
                    eventAttribute.Opcode = EventSource.GetOpcodeWithDefault(EventOpcode.Info, name);
                  if (flag2)
                  {
                    if (eventAttribute.Opcode == EventOpcode.Start)
                    {
                      string str = name.Substring(0, name.Length - "Start".Length);
                      if (string.Compare(name, 0, str, 0, str.Length) == 0 && string.Compare(name, str.Length, "Start", 0, Math.Max(name.Length - str.Length, "Start".Length)) == 0)
                        manifest.AddTask(str, (int) eventAttribute.Task);
                    }
                    else if (eventAttribute.Opcode == EventOpcode.Stop)
                    {
                      int index2 = eventAttribute.EventId - 1;
                      if (index2 < eventData.Length)
                      {
                        EventSource.EventMetadata eventMetadata = eventData[index2];
                        string strB = name.Substring(0, name.Length - "Stop".Length);
                        if (eventMetadata.Descriptor.Opcode == (byte) 1 && string.Compare(eventMetadata.Name, 0, strB, 0, strB.Length) == 0 && string.Compare(eventMetadata.Name, strB.Length, "Start", 0, Math.Max(eventMetadata.Name.Length - strB.Length, "Start".Length)) == 0)
                        {
                          eventAttribute.Task = (EventTask) eventMetadata.Descriptor.Task;
                          flag2 = false;
                        }
                      }
                      if (flag2 && (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
                        throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_StopsFollowStarts"));
                    }
                  }
                }
                EventSource.RemoveFirstArgIfRelatedActivityId(ref parameters);
                if (source == null || !source.SelfDescribingEvents)
                {
                  manifest.StartEvent(name, eventAttribute);
                  for (int index2 = 0; index2 < parameters.Length; ++index2)
                    manifest.AddEventParameter(parameters[index2].ParameterType, parameters[index2].Name);
                  manifest.EndEvent();
                }
                if (source != null || (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
                {
                  EventSource.DebugCheckEvent(ref eventsByName, eventData, method, eventAttribute, manifest);
                  if (eventAttribute.Channel != EventChannel.None)
                    eventAttribute.Keywords |= (EventKeywords) manifest.GetChannelKeyword(eventAttribute.Channel);
                  string key = "event_" + name;
                  string localizedMessage = manifest.GetLocalizedMessage(key, CultureInfo.CurrentUICulture, false);
                  if (localizedMessage != null)
                    eventAttribute.Message = localizedMessage;
                  EventSource.AddEventDescriptor(ref eventData, name, eventAttribute, parameters);
                }
              }
            }
          }
        }
        NameInfo.ReserveEventIDsBelow(eventId);
        if (source != null)
        {
          EventSource.TrimEventDescriptors(ref eventData);
          source.m_eventData = eventData;
          source.m_channelData = manifest.GetChannelData();
        }
        if (!ReflectionExtensions.IsAbstract(eventSourceType))
        {
          if (source != null)
          {
            if (source.SelfDescribingEvents)
              goto label_73;
          }
          flag1 = (flags & EventManifestOptions.OnlyIfNeededForRegistration) == EventManifestOptions.None || manifest.GetChannelData().Length > 0;
          if (!flag1 && (flags & EventManifestOptions.Strict) == EventManifestOptions.None)
            return (byte[]) null;
          numArray = manifest.CreateManifest();
        }
      }
      catch (Exception ex)
      {
        if ((flags & EventManifestOptions.Strict) == EventManifestOptions.None)
          throw;
        else
          innerException = ex;
      }
label_73:
      if ((flags & EventManifestOptions.Strict) != EventManifestOptions.None && (manifest.Errors.Count > 0 || innerException != null))
      {
        string message = string.Empty;
        if (manifest.Errors.Count > 0)
        {
          bool flag2 = true;
          foreach (string error in (IEnumerable<string>) manifest.Errors)
          {
            if (!flag2)
              message += Microsoft.Diagnostics.Tracing.Internal.Environment.NewLine;
            flag2 = false;
            message += error;
          }
        }
        else
          message = "Unexpected error: " + innerException.Message;
        throw new ArgumentException(message, innerException);
      }
      return !flag1 ? (byte[]) null : numArray;
    }

    private static void RemoveFirstArgIfRelatedActivityId(ref ParameterInfo[] args)
    {
      if (args.Length <= 0 || (object) args[0].ParameterType != (object) typeof (Guid) || string.Compare(args[0].Name, "relatedActivityId", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      ParameterInfo[] parameterInfoArray = new ParameterInfo[args.Length - 1];
      Array.Copy((Array) args, 1, (Array) parameterInfoArray, 0, args.Length - 1);
      args = parameterInfoArray;
    }

    private static void AddProviderEnumKind(
      ManifestBuilder manifest,
      FieldInfo staticField,
      string providerEnumKind)
    {
      bool flag = ReflectionExtensions.ReflectionOnly(staticField.Module.Assembly);
      Type fieldType = staticField.FieldType;
      if (!flag && (object) fieldType == (object) typeof (EventOpcode) || EventSource.AttributeTypeNamesMatch(fieldType, typeof (EventOpcode)))
      {
        if (!(providerEnumKind != "Opcodes"))
        {
          int rawConstantValue = (int) ReflectionExtensions.GetRawConstantValue(staticField);
          manifest.AddOpcode(staticField.Name, rawConstantValue);
          return;
        }
      }
      else if (!flag && (object) fieldType == (object) typeof (EventTask) || EventSource.AttributeTypeNamesMatch(fieldType, typeof (EventTask)))
      {
        if (!(providerEnumKind != "Tasks"))
        {
          int rawConstantValue = (int) ReflectionExtensions.GetRawConstantValue(staticField);
          manifest.AddTask(staticField.Name, rawConstantValue);
          return;
        }
      }
      else
      {
        if ((flag || (object) fieldType != (object) typeof (EventKeywords)) && !EventSource.AttributeTypeNamesMatch(fieldType, typeof (EventKeywords)))
          return;
        if (!(providerEnumKind != "Keywords"))
        {
          ulong rawConstantValue = (ulong) (long) ReflectionExtensions.GetRawConstantValue(staticField);
          manifest.AddKeyword(staticField.Name, rawConstantValue);
          return;
        }
      }
      manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EnumKindMismatch", (object) staticField.Name, (object) staticField.FieldType.Name, (object) providerEnumKind));
    }

    private static void AddEventDescriptor(
      ref EventSource.EventMetadata[] eventData,
      string eventName,
      EventAttribute eventAttribute,
      ParameterInfo[] eventParameters)
    {
      if (eventData == null || eventData.Length <= eventAttribute.EventId)
      {
        EventSource.EventMetadata[] eventMetadataArray = new EventSource.EventMetadata[Math.Max(eventData.Length + 16, eventAttribute.EventId + 1)];
        Array.Copy((Array) eventData, (Array) eventMetadataArray, eventData.Length);
        eventData = eventMetadataArray;
      }
      eventData[eventAttribute.EventId].Descriptor = new EventDescriptor(eventAttribute.EventId, eventAttribute.Version, (byte) eventAttribute.Channel, (byte) eventAttribute.Level, (byte) eventAttribute.Opcode, (int) eventAttribute.Task, (long) (eventAttribute.Keywords | (EventKeywords) SessionMask.All.ToEventKeywords()));
      eventData[eventAttribute.EventId].Tags = eventAttribute.Tags;
      eventData[eventAttribute.EventId].Name = eventName;
      eventData[eventAttribute.EventId].Parameters = eventParameters;
      eventData[eventAttribute.EventId].Message = eventAttribute.Message;
      eventData[eventAttribute.EventId].ActivityOptions = eventAttribute.ActivityOptions;
    }

    private static void TrimEventDescriptors(ref EventSource.EventMetadata[] eventData)
    {
      int length = eventData.Length;
      while (0 < length)
      {
        --length;
        if (eventData[length].Descriptor.EventId != 0)
          break;
      }
      if (eventData.Length - length <= 2)
        return;
      EventSource.EventMetadata[] eventMetadataArray = new EventSource.EventMetadata[length + 1];
      Array.Copy((Array) eventData, (Array) eventMetadataArray, eventMetadataArray.Length);
      eventData = eventMetadataArray;
    }

    internal void AddListener(EventListener listener)
    {
      lock (EventListener.EventListenersLock)
      {
        bool[] eventEnabled = (bool[]) null;
        if (this.m_eventData != null)
          eventEnabled = new bool[this.m_eventData.Length];
        this.m_Dispatchers = new EventDispatcher(this.m_Dispatchers, eventEnabled, listener);
        listener.OnEventSourceCreated(this);
      }
    }

    private static void DebugCheckEvent(
      ref Dictionary<string, string> eventsByName,
      EventSource.EventMetadata[] eventData,
      MethodInfo method,
      EventAttribute eventAttribute,
      ManifestBuilder manifest)
    {
      int eventId = eventAttribute.EventId;
      string name = method.Name;
      int helperCallFirstArg = EventSource.GetHelperCallFirstArg(method);
      if (helperCallFirstArg >= 0 && eventId != helperCallFirstArg)
        manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_MismatchIdToWriteEvent", (object) name, (object) eventId, (object) helperCallFirstArg), true);
      if (eventId < eventData.Length && eventData[eventId].Descriptor.EventId != 0)
        manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventIdReused", (object) name, (object) eventId, (object) eventData[eventId].Name), true);
      for (int index = 0; index < eventData.Length; ++index)
      {
        if ((EventTask) eventData[index].Descriptor.Task == eventAttribute.Task && (EventOpcode) eventData[index].Descriptor.Opcode == eventAttribute.Opcode)
          manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TaskOpcodePairReused", (object) name, (object) eventId, (object) eventData[index].Name, (object) index));
      }
      if (eventAttribute.Opcode != EventOpcode.Info)
      {
        bool flag = false;
        if (eventAttribute.Task == EventTask.None)
        {
          flag = true;
        }
        else
        {
          EventTask eventTask = (EventTask) (65534 - eventId);
          if (eventAttribute.Opcode != EventOpcode.Start && eventAttribute.Opcode != EventOpcode.Stop && eventAttribute.Task == eventTask)
            flag = true;
        }
        if (flag)
          manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventMustHaveTaskIfNonDefaultOpcode", (object) name, (object) eventId));
      }
      if (eventsByName == null)
        eventsByName = new Dictionary<string, string>();
      if (eventsByName.ContainsKey(name))
        manifest.ManifestError(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_EventNameReused", (object) name));
      eventsByName[name] = name;
    }

    [SecuritySafeCritical]
    private static int GetHelperCallFirstArg(MethodInfo method) => -1;

    internal void ReportOutOfBandMessage(string msg, bool flush)
    {
      try
      {
        if (this.m_outOfBandMessageCount < (byte) 254)
        {
          ++this.m_outOfBandMessageCount;
        }
        else
        {
          if (this.m_outOfBandMessageCount == byte.MaxValue)
            return;
          this.m_outOfBandMessageCount = byte.MaxValue;
          msg = "Reached message limit.   End of EventSource error messages.";
        }
        this.WriteEventString(EventLevel.LogAlways, -1L, msg);
        this.WriteStringToAllListeners("EventSourceMessage", msg);
      }
      catch (Exception ex)
      {
      }
    }

    private EventSourceSettings ValidateSettings(EventSourceSettings settings)
    {
      EventSourceSettings eventSourceSettings = EventSourceSettings.EtwManifestEventFormat | EventSourceSettings.EtwSelfDescribingEventFormat;
      if ((settings & eventSourceSettings) == eventSourceSettings)
        throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_InvalidEventFormat"), nameof (settings));
      if ((settings & eventSourceSettings) == EventSourceSettings.Default)
        settings |= EventSourceSettings.EtwSelfDescribingEventFormat;
      return settings;
    }

    private bool ThrowOnEventWriteErrors
    {
      get => (this.m_config & EventSourceSettings.ThrowOnEventWriteErrors) != EventSourceSettings.Default;
      set
      {
        if (value)
          this.m_config |= EventSourceSettings.ThrowOnEventWriteErrors;
        else
          this.m_config &= ~EventSourceSettings.ThrowOnEventWriteErrors;
      }
    }

    private bool SelfDescribingEvents
    {
      get => (this.m_config & EventSourceSettings.EtwSelfDescribingEventFormat) != EventSourceSettings.Default;
      set
      {
        if (!value)
        {
          this.m_config |= EventSourceSettings.EtwManifestEventFormat;
          this.m_config &= ~EventSourceSettings.EtwSelfDescribingEventFormat;
        }
        else
        {
          this.m_config |= EventSourceSettings.EtwSelfDescribingEventFormat;
          this.m_config &= ~EventSourceSettings.EtwManifestEventFormat;
        }
      }
    }

    public EventSource(string eventSourceName)
      : this(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat)
    {
    }

    public EventSource(string eventSourceName, EventSourceSettings config)
      : this(eventSourceName, config, (string[]) null)
    {
    }

    public EventSource(string eventSourceName, EventSourceSettings config, params string[] traits)
      : this(eventSourceName == null ? new Guid() : EventSource.GenerateGuidFromName(eventSourceName.ToUpperInvariant()), eventSourceName, config, traits)
    {
      if (eventSourceName == null)
        throw new ArgumentNullException(nameof (eventSourceName));
    }

    [SecuritySafeCritical]
    public unsafe void Write(string eventName)
    {
      if (eventName == null)
        throw new ArgumentNullException(nameof (eventName));
      if (!this.IsEnabled())
        return;
      EventSourceOptions options = new EventSourceOptions();
      EmptyStruct data = new EmptyStruct();
      this.WriteImpl<EmptyStruct>(eventName, ref options, ref data, (Guid*) null, (Guid*) null);
    }

    [SecuritySafeCritical]
    public unsafe void Write(string eventName, EventSourceOptions options)
    {
      if (eventName == null)
        throw new ArgumentNullException(nameof (eventName));
      if (!this.IsEnabled())
        return;
      EmptyStruct data = new EmptyStruct();
      this.WriteImpl<EmptyStruct>(eventName, ref options, ref data, (Guid*) null, (Guid*) null);
    }

    [SecuritySafeCritical]
    public unsafe void Write<T>(string eventName, T data)
    {
      if (!this.IsEnabled())
        return;
      EventSourceOptions options = new EventSourceOptions();
      this.WriteImpl<T>(eventName, ref options, ref data, (Guid*) null, (Guid*) null);
    }

    [SecuritySafeCritical]
    public unsafe void Write<T>(string eventName, EventSourceOptions options, T data)
    {
      if (!this.IsEnabled())
        return;
      this.WriteImpl<T>(eventName, ref options, ref data, (Guid*) null, (Guid*) null);
    }

    [SecuritySafeCritical]
    public unsafe void Write<T>(string eventName, ref EventSourceOptions options, ref T data)
    {
      if (!this.IsEnabled())
        return;
      this.WriteImpl<T>(eventName, ref options, ref data, (Guid*) null, (Guid*) null);
    }

    [SecuritySafeCritical]
    public unsafe void Write<T>(
      string eventName,
      ref EventSourceOptions options,
      ref Guid activityId,
      ref Guid relatedActivityId,
      ref T data)
    {
      if (!this.IsEnabled())
        return;
      fixed (Guid* pActivityId = &activityId)
        fixed (Guid* guidPtr = &relatedActivityId)
          this.WriteImpl<T>(eventName, ref options, ref data, pActivityId, relatedActivityId == Guid.Empty ? (Guid*) null : guidPtr);
    }

    [SecuritySafeCritical]
    private unsafe void WriteMultiMerge(
      string eventName,
      ref EventSourceOptions options,
      TraceLoggingEventTypes eventTypes,
      Guid* activityID,
      Guid* childActivityID,
      params object[] values)
    {
      if (!this.IsEnabled() || !this.IsEnabled(((int) options.valuesSet & 4) != 0 ? (EventLevel) options.level : (EventLevel) eventTypes.level, ((int) options.valuesSet & 1) != 0 ? options.keywords : eventTypes.keywords))
        return;
      this.WriteMultiMergeInner(eventName, ref options, eventTypes, activityID, childActivityID, values);
    }

    [SecuritySafeCritical]
    private unsafe void WriteMultiMergeInner(
      string eventName,
      ref EventSourceOptions options,
      TraceLoggingEventTypes eventTypes,
      Guid* activityID,
      Guid* childActivityID,
      params object[] values)
    {
      byte level = ((int) options.valuesSet & 4) != 0 ? options.level : eventTypes.level;
      byte opcode = ((int) options.valuesSet & 8) != 0 ? options.opcode : eventTypes.opcode;
      EventTags tags = ((int) options.valuesSet & 2) != 0 ? options.tags : eventTypes.Tags;
      EventKeywords eventKeywords = ((int) options.valuesSet & 1) != 0 ? options.keywords : eventTypes.keywords;
      NameInfo nameInfo = eventTypes.GetNameInfo(eventName ?? eventTypes.Name, tags);
      if (nameInfo == null)
        return;
      EventDescriptor eventDescriptor = new EventDescriptor(nameInfo.identity, level, opcode, (long) eventKeywords);
      int pinCount = eventTypes.pinCount;
      byte* scratch = stackalloc byte[eventTypes.scratchSize];
      EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[eventTypes.dataCount + 3];
      GCHandle* gcHandlePtr = stackalloc GCHandle[pinCount];
      fixed (byte* pointer1 = this.providerMetadata)
        fixed (byte* pointer2 = nameInfo.nameMetadata)
          fixed (byte* pointer3 = eventTypes.typeMetadata)
          {
            eventDataPtr->SetMetadata(pointer1, this.providerMetadata.Length, 2);
            eventDataPtr[1].SetMetadata(pointer2, nameInfo.nameMetadata.Length, 1);
            eventDataPtr[2].SetMetadata(pointer3, eventTypes.typeMetadata.Length, 1);
            try
            {
              DataCollector.ThreadInstance.Enable(scratch, eventTypes.scratchSize, eventDataPtr + 3, eventTypes.dataCount, gcHandlePtr, pinCount);
              for (int index = 0; index < eventTypes.typeInfos.Length; ++index)
                eventTypes.typeInfos[index].WriteObjectData(TraceLoggingDataCollector.Instance, values[index]);
              this.WriteEventRaw(ref eventDescriptor, activityID, childActivityID, (int) (DataCollector.ThreadInstance.Finish() - eventDataPtr), (IntPtr) (void*) eventDataPtr);
            }
            finally
            {
              this.WriteCleanup(gcHandlePtr, pinCount);
            }
          }
    }

    [SecuritySafeCritical]
    internal unsafe void WriteMultiMerge(
      string eventName,
      ref EventSourceOptions options,
      TraceLoggingEventTypes eventTypes,
      Guid* activityID,
      Guid* childActivityID,
      EventSource.EventData* data)
    {
      if (!this.IsEnabled())
        return;
      EventDescriptor descriptor;
      NameInfo nameInfo = this.UpdateDescriptor(eventName, eventTypes, ref options, out descriptor);
      if (nameInfo == null)
        return;
      EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[eventTypes.dataCount + eventTypes.typeInfos.Length * 2 + 3];
      fixed (byte* pointer1 = this.providerMetadata)
        fixed (byte* pointer2 = nameInfo.nameMetadata)
          fixed (byte* pointer3 = eventTypes.typeMetadata)
          {
            eventDataPtr->SetMetadata(pointer1, this.providerMetadata.Length, 2);
            eventDataPtr[1].SetMetadata(pointer2, nameInfo.nameMetadata.Length, 1);
            eventDataPtr[2].SetMetadata(pointer3, eventTypes.typeMetadata.Length, 1);
            int dataCount = 3;
            for (int index1 = 0; index1 < eventTypes.typeInfos.Length; ++index1)
            {
              if ((object) eventTypes.typeInfos[index1].DataType == (object) typeof (string))
              {
                eventDataPtr[dataCount].m_Ptr = (long) &eventDataPtr[dataCount + 1].m_Size;
                eventDataPtr[dataCount].m_Size = 2;
                int index2 = dataCount + 1;
                eventDataPtr[index2].m_Ptr = data[index1].m_Ptr;
                eventDataPtr[index2].m_Size = data[index1].m_Size - 2;
                dataCount = index2 + 1;
              }
              else
              {
                eventDataPtr[dataCount].m_Ptr = data[index1].m_Ptr;
                eventDataPtr[dataCount].m_Size = data[index1].m_Size;
                if (data[index1].m_Size == 4 && (object) eventTypes.typeInfos[index1].DataType == (object) typeof (bool))
                  eventDataPtr[dataCount].m_Size = 1;
                ++dataCount;
              }
            }
            this.WriteEventRaw(ref descriptor, activityID, childActivityID, dataCount, (IntPtr) (void*) eventDataPtr);
          }
    }

    [SecuritySafeCritical]
    private unsafe void WriteImpl<T>(
      string eventName,
      ref EventSourceOptions options,
      ref T data,
      Guid* pActivityId,
      Guid* pRelatedActivityId)
    {
      try
      {
        SimpleEventTypes<T> instance = SimpleEventTypes<T>.Instance;
        options.Opcode = options.IsOpcodeSet ? options.Opcode : EventSource.GetOpcodeWithDefault(options.Opcode, eventName);
        EventDescriptor descriptor;
        NameInfo nameInfo = this.UpdateDescriptor(eventName, (TraceLoggingEventTypes) instance, ref options, out descriptor);
        if (nameInfo == null)
          return;
        int pinCount = instance.pinCount;
        byte* scratch = stackalloc byte[instance.scratchSize];
        EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[instance.dataCount + 3];
        GCHandle* gcHandlePtr = stackalloc GCHandle[pinCount];
        fixed (byte* pointer1 = this.providerMetadata)
          fixed (byte* pointer2 = nameInfo.nameMetadata)
            fixed (byte* pointer3 = instance.typeMetadata)
            {
              eventDataPtr->SetMetadata(pointer1, this.providerMetadata.Length, 2);
              eventDataPtr[1].SetMetadata(pointer2, nameInfo.nameMetadata.Length, 1);
              eventDataPtr[2].SetMetadata(pointer3, instance.typeMetadata.Length, 1);
              EventOpcode opcode = (EventOpcode) descriptor.Opcode;
              Guid empty1 = Guid.Empty;
              Guid empty2 = Guid.Empty;
              if ((IntPtr) pActivityId == IntPtr.Zero)
              {
                if ((IntPtr) pRelatedActivityId == IntPtr.Zero)
                {
                  if ((options.ActivityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
                  {
                    switch (opcode)
                    {
                      case EventOpcode.Start:
                        this.m_activityTracker.OnStart(this.m_name, eventName, 0, ref empty1, ref empty2, options.ActivityOptions);
                        break;
                      case EventOpcode.Stop:
                        this.m_activityTracker.OnStop(this.m_name, eventName, 0, ref empty1);
                        break;
                    }
                    if (empty1 != Guid.Empty)
                      pActivityId = &empty1;
                    if (empty2 != Guid.Empty)
                      pRelatedActivityId = &empty2;
                  }
                }
              }
              try
              {
                DataCollector.ThreadInstance.Enable(scratch, instance.scratchSize, eventDataPtr + 3, instance.dataCount, gcHandlePtr, pinCount);
                instance.typeInfo.WriteData(TraceLoggingDataCollector.Instance, ref data);
                this.WriteEventRaw(ref descriptor, pActivityId, pRelatedActivityId, (int) (DataCollector.ThreadInstance.Finish() - eventDataPtr), (IntPtr) (void*) eventDataPtr);
                if (this.m_Dispatchers == null)
                  return;
                EventPayload data1 = (EventPayload) instance.typeInfo.GetData((object) data);
                this.WriteToAllListeners(eventName, ref descriptor, nameInfo.tags, pActivityId, data1);
              }
              catch (Exception ex)
              {
                if (ex is EventSourceException)
                  throw;
                else
                  this.ThrowEventSourceException(ex);
              }
              finally
              {
                this.WriteCleanup(gcHandlePtr, pinCount);
              }
            }
      }
      catch (Exception ex)
      {
        if (ex is EventSourceException)
          throw;
        else
          this.ThrowEventSourceException(ex);
      }
    }

    [SecurityCritical]
    private unsafe void WriteToAllListeners(
      string eventName,
      ref EventDescriptor eventDescriptor,
      EventTags tags,
      Guid* pActivityId,
      EventPayload payload)
    {
      EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
      eventCallbackArgs.EventName = eventName;
      eventCallbackArgs.m_keywords = (EventKeywords) eventDescriptor.Keywords;
      eventCallbackArgs.m_opcode = (EventOpcode) eventDescriptor.Opcode;
      eventCallbackArgs.m_tags = tags;
      eventCallbackArgs.EventId = -1;
      if ((IntPtr) pActivityId != IntPtr.Zero)
        eventCallbackArgs.RelatedActivityId = *pActivityId;
      if (payload != null)
      {
        eventCallbackArgs.Payload = new ReadOnlyCollection<object>((IList<object>) payload.Values);
        eventCallbackArgs.PayloadNames = new ReadOnlyCollection<string>((IList<string>) payload.Keys);
      }
      this.DisptachToAllListeners(-1, pActivityId, eventCallbackArgs);
    }

    [SecurityCritical]
    [NonEvent]
    private unsafe void WriteCleanup(GCHandle* pPins, int cPins)
    {
      DataCollector.ThreadInstance.Disable();
      for (int index = 0; index != cPins; ++index)
      {
        if (IntPtr.Zero != (IntPtr) pPins[index])
          pPins[index].Free();
      }
    }

    private void InitializeProviderMetadata()
    {
      if (this.m_traits != null)
      {
        List<byte> metaData = new List<byte>(100);
        for (int index = 0; index < this.m_traits.Length - 1; index += 2)
        {
          if (this.m_traits[index].StartsWith("ETW_"))
          {
            string s = this.m_traits[index].Substring(4);
            byte result;
            if (!byte.TryParse(s, out result))
            {
              if (s == "GROUP")
                result = (byte) 1;
              else
                throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("UnknownEtwTrait", (object) s), "traits");
            }
            string trait = this.m_traits[index + 1];
            int count = metaData.Count;
            metaData.Add((byte) 0);
            metaData.Add((byte) 0);
            metaData.Add(result);
            int num = EventSource.AddValueToMetaData(metaData, trait) + 3;
            metaData[count] = (byte) num;
            metaData[count + 1] = (byte) (num >> 8);
          }
        }
        this.providerMetadata = Statics.MetadataForString(this.Name, 0, metaData.Count, 0);
        int num1 = this.providerMetadata.Length - metaData.Count;
        foreach (byte num2 in metaData)
          this.providerMetadata[num1++] = num2;
      }
      else
        this.providerMetadata = Statics.MetadataForString(this.Name, 0, 0, 0);
    }

    private static int AddValueToMetaData(List<byte> metaData, string value)
    {
      if (value.Length == 0)
        return 0;
      int count = metaData.Count;
      char ch = value[0];
      switch (ch)
      {
        case '#':
          for (int index = 1; index < value.Length; ++index)
          {
            if (value[index] != ' ')
            {
              if (index + 1 >= value.Length)
                throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EvenHexDigits"), "traits");
              metaData.Add((byte) (EventSource.HexDigit(value[index]) * 16 + EventSource.HexDigit(value[index + 1])));
              ++index;
            }
          }
          break;
        case '@':
          metaData.AddRange((IEnumerable<byte>) Encoding.UTF8.GetBytes(value.Substring(1)));
          break;
        case '{':
          metaData.AddRange((IEnumerable<byte>) new Guid(value).ToByteArray());
          break;
        default:
          if ('A' <= ch || ' ' == ch)
          {
            metaData.AddRange((IEnumerable<byte>) Encoding.UTF8.GetBytes(value));
            break;
          }
          throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("IllegalValue", (object) value), "traits");
      }
      return metaData.Count - count;
    }

    private static int HexDigit(char c)
    {
      if ('0' <= c && c <= '9')
        return (int) c - 48;
      if ('a' <= c)
        c -= ' ';
      if ('A' <= c && c <= 'F')
        return (int) c - 65 + 10;
      throw new ArgumentException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("BadHexDigit", (object) c), "traits");
    }

    private NameInfo UpdateDescriptor(
      string name,
      TraceLoggingEventTypes eventInfo,
      ref EventSourceOptions options,
      out EventDescriptor descriptor)
    {
      NameInfo nameInfo = (NameInfo) null;
      int traceloggingId = 0;
      byte level = ((int) options.valuesSet & 4) != 0 ? options.level : eventInfo.level;
      byte opcode = ((int) options.valuesSet & 8) != 0 ? options.opcode : eventInfo.opcode;
      EventTags tags = ((int) options.valuesSet & 2) != 0 ? options.tags : eventInfo.Tags;
      EventKeywords keywords = ((int) options.valuesSet & 1) != 0 ? options.keywords : eventInfo.keywords;
      if (this.IsEnabled((EventLevel) level, keywords))
      {
        nameInfo = eventInfo.GetNameInfo(name ?? eventInfo.Name, tags);
        traceloggingId = nameInfo.identity;
      }
      descriptor = new EventDescriptor(traceloggingId, level, opcode, (long) keywords);
      return nameInfo;
    }

    protected internal struct EventData
    {
      internal long m_Ptr;
      internal int m_Size;
      internal int m_Reserved;

      public IntPtr DataPointer
      {
        get => (IntPtr) this.m_Ptr;
        set => this.m_Ptr = (long) value;
      }

      public int Size
      {
        get => this.m_Size;
        set => this.m_Size = value;
      }

      [SecurityCritical]
      internal unsafe void SetMetadata(byte* pointer, int size, int reserved)
      {
        this.m_Ptr = (long) (ulong) (UIntPtr) (void*) pointer;
        this.m_Size = size;
        this.m_Reserved = reserved;
      }
    }

    private struct Sha1ForNonSecretPurposes
    {
      private long length;
      private uint[] w;
      private int pos;

      public void Start()
      {
        if (this.w == null)
          this.w = new uint[85];
        this.length = 0L;
        this.pos = 0;
        this.w[80] = 1732584193U;
        this.w[81] = 4023233417U;
        this.w[82] = 2562383102U;
        this.w[83] = 271733878U;
        this.w[84] = 3285377520U;
      }

      public void Append(byte input)
      {
        this.w[this.pos / 4] = this.w[this.pos / 4] << 8 | (uint) input;
        if (64 != ++this.pos)
          return;
        this.Drain();
      }

      public void Append(byte[] input)
      {
        foreach (byte input1 in input)
          this.Append(input1);
      }

      public void Finish(byte[] output)
      {
        long num1 = this.length + (long) (8 * this.pos);
        this.Append((byte) 128);
        while (this.pos != 56)
          this.Append((byte) 0);
        this.Append((byte) (num1 >> 56));
        this.Append((byte) (num1 >> 48));
        this.Append((byte) (num1 >> 40));
        this.Append((byte) (num1 >> 32));
        this.Append((byte) (num1 >> 24));
        this.Append((byte) (num1 >> 16));
        this.Append((byte) (num1 >> 8));
        this.Append((byte) num1);
        int num2 = output.Length < 20 ? output.Length : 20;
        for (int index = 0; index != num2; ++index)
        {
          uint num3 = this.w[80 + index / 4];
          output[index] = (byte) (num3 >> 24);
          this.w[80 + index / 4] = num3 << 8;
        }
      }

      private void Drain()
      {
        for (int index = 16; index != 80; ++index)
          this.w[index] = EventSource.Sha1ForNonSecretPurposes.Rol1(this.w[index - 3] ^ this.w[index - 8] ^ this.w[index - 14] ^ this.w[index - 16]);
        uint input1 = this.w[80];
        uint input2 = this.w[81];
        uint num1 = this.w[82];
        uint num2 = this.w[83];
        uint num3 = this.w[84];
        for (int index = 0; index != 20; ++index)
        {
          uint num4 = (uint) ((int) input2 & (int) num1 | ~(int) input2 & (int) num2);
          uint num5 = (uint) ((int) EventSource.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num4 + (int) num3 + 1518500249) + this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = EventSource.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = num5;
        }
        for (int index = 20; index != 40; ++index)
        {
          uint num4 = input2 ^ num1 ^ num2;
          uint num5 = (uint) ((int) EventSource.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num4 + (int) num3 + 1859775393) + this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = EventSource.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = num5;
        }
        for (int index = 40; index != 60; ++index)
        {
          uint num4 = (uint) ((int) input2 & (int) num1 | (int) input2 & (int) num2 | (int) num1 & (int) num2);
          uint num5 = (uint) ((int) EventSource.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num4 + (int) num3 - 1894007588) + this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = EventSource.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = num5;
        }
        for (int index = 60; index != 80; ++index)
        {
          uint num4 = input2 ^ num1 ^ num2;
          uint num5 = (uint) ((int) EventSource.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num4 + (int) num3 - 899497514) + this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = EventSource.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = num5;
        }
        this.w[80] += input1;
        this.w[81] += input2;
        this.w[82] += num1;
        this.w[83] += num2;
        this.w[84] += num3;
        this.length += 512L;
        this.pos = 0;
      }

      private static uint Rol1(uint input) => input << 1 | input >> 31;

      private static uint Rol5(uint input) => input << 5 | input >> 27;

      private static uint Rol30(uint input) => input << 30 | input >> 2;
    }

    private class OverideEventProvider : EventProvider
    {
      private EventSource m_eventSource;

      public OverideEventProvider(EventSource eventSource) => this.m_eventSource = eventSource;

      protected override void OnControllerCommand(
        ControllerCommand command,
        IDictionary<string, string> arguments,
        int perEventSourceSessionId,
        int etwSessionId)
      {
        this.m_eventSource.SendCommand((EventListener) null, perEventSourceSessionId, etwSessionId, (EventCommand) command, this.IsEnabled(), this.Level, this.MatchAnyKeyword, arguments);
      }
    }

    internal struct EventMetadata
    {
      public EventDescriptor Descriptor;
      public EventTags Tags;
      public bool EnabledForAnyListener;
      public bool EnabledForETW;
      public byte TriggersActivityTracking;
      public string Name;
      public string Message;
      public ParameterInfo[] Parameters;
      public TraceLoggingEventTypes TraceLoggingEventTypes;
      public EventActivityOptions ActivityOptions;
    }
  }
}
