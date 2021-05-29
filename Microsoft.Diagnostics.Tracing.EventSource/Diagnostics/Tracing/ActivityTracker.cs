// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.ActivityTracker
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Security;
using System.Threading;

namespace Microsoft.Diagnostics.Tracing
{
  internal class ActivityTracker
  {
    private const ushort MAX_ACTIVITY_DEPTH = 100;
    private AsyncLocal<ActivityTracker.ActivityInfo> m_current;
    private static ActivityTracker s_activityTrackerInstance = new ActivityTracker();
    private static long m_nextId = 0;

    public void OnStart(
      string providerName,
      string activityName,
      int task,
      ref Guid activityId,
      ref Guid relatedActivityId,
      EventActivityOptions options)
    {
      if (this.m_current == null)
        return;
      ActivityTracker.ActivityInfo activityInfo = this.m_current.Value;
      string name = this.NormalizeActivityName(providerName, activityName, task);
      TplEtwProvider.Logger log = TplEtwProvider.Log;
      if (log.Debug)
      {
        log.DebugFacilityMessage((object) "OnStartEnter", (object) name);
        log.DebugFacilityMessage((object) "OnStartEnterActivityState", (object) ActivityTracker.ActivityInfo.LiveActivities(activityInfo));
      }
      if (activityInfo != null)
      {
        if (activityInfo.m_level >= 100)
        {
          activityId = Guid.Empty;
          relatedActivityId = Guid.Empty;
          if (!log.Debug)
            return;
          log.DebugFacilityMessage((object) "OnStartRET", (object) "Fail");
          return;
        }
        if ((options & EventActivityOptions.Recursive) == EventActivityOptions.None && this.FindActiveActivity(name, activityInfo) != null)
        {
          this.OnStop(providerName, activityName, task, ref activityId);
          activityInfo = this.m_current.Value;
        }
      }
      long uniqueId = activityInfo != null ? Interlocked.Increment(ref activityInfo.m_lastChildID) : Interlocked.Increment(ref ActivityTracker.m_nextId);
      relatedActivityId = activityInfo != null ? activityInfo.ActivityId : EventSource.CurrentThreadActivityId;
      ActivityTracker.ActivityInfo list = new ActivityTracker.ActivityInfo(name, uniqueId, activityInfo, options);
      this.m_current.Value = list;
      activityId = list.ActivityId;
      if (!log.Debug)
        return;
      log.DebugFacilityMessage((object) "OnStartRetActivityState", (object) ActivityTracker.ActivityInfo.LiveActivities(list));
      log.DebugFacilityMessage((object) "OnStartRet", (object) activityId.ToString(), (object) relatedActivityId.ToString());
    }

    public void OnStop(string providerName, string activityName, int task, ref Guid activityId)
    {
      if (this.m_current == null)
        return;
      string name = this.NormalizeActivityName(providerName, activityName, task);
      TplEtwProvider.Logger log = TplEtwProvider.Log;
      if (log.Debug)
      {
        log.DebugFacilityMessage((object) "OnStopEnter", (object) name);
        log.DebugFacilityMessage((object) "OnStopEnterActivityState", (object) ActivityTracker.ActivityInfo.LiveActivities(this.m_current.Value));
      }
      ActivityTracker.ActivityInfo list;
      ActivityTracker.ActivityInfo activeActivity;
      do
      {
        ActivityTracker.ActivityInfo startLocation = this.m_current.Value;
        list = (ActivityTracker.ActivityInfo) null;
        activeActivity = this.FindActiveActivity(name, startLocation);
        if (activeActivity == null)
        {
          activityId = Guid.Empty;
          if (!log.Debug)
            return;
          log.DebugFacilityMessage((object) "OnStopRET", (object) "Fail");
          return;
        }
        activityId = activeActivity.ActivityId;
        ActivityTracker.ActivityInfo activityInfo = startLocation;
        while (activityInfo != activeActivity && activityInfo != null)
        {
          if (activityInfo.m_stopped != 0)
          {
            activityInfo = activityInfo.m_creator;
          }
          else
          {
            if (activityInfo.CanBeOrphan())
            {
              if (list == null)
                list = activityInfo;
            }
            else
              activityInfo.m_stopped = 1;
            activityInfo = activityInfo.m_creator;
          }
        }
      }
      while (Interlocked.CompareExchange(ref activeActivity.m_stopped, 1, 0) != 0);
      if (list == null)
        list = activeActivity.m_creator;
      this.m_current.Value = list;
      if (!log.Debug)
        return;
      log.DebugFacilityMessage((object) "OnStopRetActivityState", (object) ActivityTracker.ActivityInfo.LiveActivities(list));
      log.DebugFacilityMessage((object) "OnStopRet", (object) activityId.ToString());
    }

    [SecuritySafeCritical]
    public void Enable()
    {
      if (this.m_current != null)
        return;
      this.m_current = new AsyncLocal<ActivityTracker.ActivityInfo>(new Action<AsyncLocalValueChangedArgs<ActivityTracker.ActivityInfo>>(this.ActivityChanging));
    }

    public static ActivityTracker Instance => ActivityTracker.s_activityTrackerInstance;

    private Guid CurrentActivityId => this.m_current.Value.ActivityId;

    private ActivityTracker.ActivityInfo FindActiveActivity(
      string name,
      ActivityTracker.ActivityInfo startLocation)
    {
      for (ActivityTracker.ActivityInfo activityInfo = startLocation; activityInfo != null; activityInfo = activityInfo.m_creator)
      {
        if (name == activityInfo.m_name && activityInfo.m_stopped == 0)
          return activityInfo;
      }
      return (ActivityTracker.ActivityInfo) null;
    }

    private string NormalizeActivityName(string providerName, string activityName, int task)
    {
      if (activityName.EndsWith("Start"))
        activityName = activityName.Substring(0, activityName.Length - "Start".Length);
      else if (activityName.EndsWith("Stop"))
        activityName = activityName.Substring(0, activityName.Length - "Stop".Length);
      else if (task != 0)
        activityName = nameof (task) + task.ToString();
      return providerName + activityName;
    }

    private void ActivityChanging(
      AsyncLocalValueChangedArgs<ActivityTracker.ActivityInfo> args)
    {
      if (args.PreviousValue == args.CurrentValue)
        return;
      if (args.CurrentValue != null)
        EventSource.SetCurrentThreadActivityId(args.CurrentValue.ActivityId);
      else
        EventSource.SetCurrentThreadActivityId(Guid.Empty);
    }

    private class ActivityInfo
    {
      internal readonly string m_name;
      private readonly long m_uniqueId;
      internal readonly Guid m_guid;
      internal readonly int m_activityPathGuidOffset;
      internal readonly int m_level;
      internal readonly EventActivityOptions m_eventOptions;
      internal long m_lastChildID;
      internal int m_stopped;
      internal readonly ActivityTracker.ActivityInfo m_creator;

      public ActivityInfo(
        string name,
        long uniqueId,
        ActivityTracker.ActivityInfo creator,
        EventActivityOptions options)
      {
        this.m_name = name;
        this.m_eventOptions = options;
        this.m_creator = creator;
        this.m_uniqueId = uniqueId;
        this.m_level = creator != null ? creator.m_level + 1 : 0;
        this.CreateActivityPathGuid(out this.m_guid, out this.m_activityPathGuidOffset);
      }

      public Guid ActivityId => this.m_guid;

      public static string Path(ActivityTracker.ActivityInfo activityInfo) => activityInfo == null ? "" : ActivityTracker.ActivityInfo.Path(activityInfo.m_creator) + "/" + (object) activityInfo.m_uniqueId;

      public override string ToString()
      {
        string str = "";
        if (this.m_stopped != 0)
          str = ",DEAD";
        return this.m_name + "(" + ActivityTracker.ActivityInfo.Path(this) + str + ")";
      }

      public static string LiveActivities(ActivityTracker.ActivityInfo list) => list == null ? "" : list.ToString() + ";" + ActivityTracker.ActivityInfo.LiveActivities(list.m_creator);

      public bool CanBeOrphan() => (this.m_eventOptions & EventActivityOptions.Detachable) != EventActivityOptions.None;

      [SecuritySafeCritical]
      private unsafe void CreateActivityPathGuid(out Guid idRet, out int activityPathGuidOffset)
      {
        fixed (Guid* outPtr = &idRet)
        {
          int whereToAddId1 = 0;
          int whereToAddId2;
          if (this.m_creator != null)
          {
            whereToAddId2 = this.m_creator.m_activityPathGuidOffset;
            idRet = this.m_creator.m_guid;
          }
          else
          {
            int num = 0;
            whereToAddId2 = ActivityTracker.ActivityInfo.AddIdToGuid(outPtr, whereToAddId1, (uint) num);
          }
          activityPathGuidOffset = ActivityTracker.ActivityInfo.AddIdToGuid(outPtr, whereToAddId2, (uint) this.m_uniqueId);
          if (12 < activityPathGuidOffset)
            this.CreateOverflowGuid(outPtr);
        }
      }

      [SecurityCritical]
      private unsafe void CreateOverflowGuid(Guid* outPtr)
      {
        for (ActivityTracker.ActivityInfo creator = this.m_creator; creator != null; creator = creator.m_creator)
        {
          if (creator.m_activityPathGuidOffset <= 10)
          {
            uint id = (uint) Interlocked.Increment(ref creator.m_lastChildID);
            *outPtr = creator.m_guid;
            if (ActivityTracker.ActivityInfo.AddIdToGuid(outPtr, creator.m_activityPathGuidOffset, id, true) <= 12)
              break;
          }
        }
      }

      [SecurityCritical]
      private static unsafe int AddIdToGuid(
        Guid* outPtr,
        int whereToAddId,
        uint id,
        bool overflow = false)
      {
        byte* numPtr1 = (byte*) outPtr;
        byte* endPtr = numPtr1 + 12;
        byte* ptr = numPtr1 + whereToAddId;
        if (endPtr <= ptr)
          return 13;
        if (0U < id && id <= 10U && !overflow)
        {
          ActivityTracker.ActivityInfo.WriteNibble(ref ptr, endPtr, id);
        }
        else
        {
          uint num = 4;
          if (id <= (uint) byte.MaxValue)
            num = 1U;
          else if (id <= (uint) ushort.MaxValue)
            num = 2U;
          else if (id <= 16777215U)
            num = 3U;
          if (overflow)
          {
            if (endPtr <= ptr + 2)
              return 13;
            ActivityTracker.ActivityInfo.WriteNibble(ref ptr, endPtr, 11U);
          }
          ActivityTracker.ActivityInfo.WriteNibble(ref ptr, endPtr, (uint) (12 + ((int) num - 1)));
          if (ptr < endPtr && *ptr != (byte) 0)
          {
            if (id < 4096U)
            {
              *ptr = (byte) (192U + (id >> 8));
              id &= (uint) byte.MaxValue;
            }
            ++ptr;
          }
          for (; 0U < num; --num)
          {
            if (endPtr <= ptr)
            {
              ++ptr;
              break;
            }
            *ptr++ = (byte) id;
            id >>= 8;
          }
        }
        uint* numPtr2 = (uint*) outPtr;
        numPtr2[3] = (uint) ((int) *numPtr2 + (int) numPtr2[1] + (int) numPtr2[2] + 1503500717);
        return (int) (ptr - (byte*) outPtr);
      }

      [SecurityCritical]
      private static unsafe void WriteNibble(ref byte* ptr, byte* endPtr, uint value)
      {
        if (*ptr != (byte) 0)
        {
          byte* numPtr = ptr++;
          int num = (int) (byte) ((uint) *numPtr | (uint) (byte) value);
          *numPtr = (byte) num;
        }
        else
          *ptr = (byte) (value << 4);
      }

      private enum NumberListCodes : byte
      {
        End = 0,
        LastImmediateValue = 10, // 0x0A
        PrefixCode = 11, // 0x0B
        MultiByte1 = 12, // 0x0C
      }
    }
  }
}
