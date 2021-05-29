// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventProvider
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Diagnostics.Tracing
{
  [HostProtection(MayLeakOnAbort = true)]
  internal class EventProvider : IDisposable
  {
    private const int s_basicTypeAllocationBufferSize = 16;
    private const int s_etwMaxNumberArguments = 32;
    private const int s_etwAPIMaxRefObjCount = 8;
    private const int s_maxEventDataDescriptors = 128;
    private const int s_traceEventMaximumSize = 65482;
    private const int s_traceEventMaximumStringSize = 32724;
    private static bool m_setInformationMissing;
    [SecurityCritical]
    private UnsafeNativeMethods.ManifestEtw.EtwEnableCallback m_etwCallback;
    private long m_regHandle;
    private byte m_level;
    private long m_anyKeywordMask;
    private long m_allKeywordMask;
    private bool m_enabled;
    private Guid m_providerId;
    internal bool m_disposed;
    [ThreadStatic]
    private static EventProvider.WriteEventErrorCode s_returnCode;
    private static int[] nibblebits = new int[16]
    {
      0,
      1,
      1,
      2,
      1,
      2,
      2,
      3,
      1,
      2,
      2,
      3,
      2,
      3,
      3,
      4
    };

    internal EventProvider()
    {
    }

    [SecurityCritical]
    internal void Register(Guid providerGuid)
    {
      this.m_providerId = providerGuid;
      this.m_etwCallback = new UnsafeNativeMethods.ManifestEtw.EtwEnableCallback(this.EtwEnableCallBack);
      uint num = this.EventRegister(ref this.m_providerId, this.m_etwCallback);
      if (num != 0U)
        throw new ArgumentException(Win32Native.GetMessage((int) num));
    }

    [SecurityCritical]
    internal unsafe int SetInformation(
      UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS eventInfoClass,
      void* data,
      int dataSize)
    {
      int num = 50;
      if (!EventProvider.m_setInformationMissing)
      {
        try
        {
          num = UnsafeNativeMethods.ManifestEtw.EventSetInformation(this.m_regHandle, eventInfoClass, data, dataSize);
        }
        catch (TypeLoadException ex)
        {
          EventProvider.m_setInformationMissing = true;
        }
      }
      return num;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SecuritySafeCritical]
    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      this.m_enabled = false;
      lock (EventListener.EventListenersLock)
      {
        if (this.m_disposed)
          return;
        this.Deregister();
        this.m_disposed = true;
      }
    }

    public virtual void Close() => this.Dispose();

    ~EventProvider() => this.Dispose(false);

    [SecurityCritical]
    private void Deregister()
    {
      if (this.m_regHandle == 0L)
        return;
      int num = (int) this.EventUnregister();
      this.m_regHandle = 0L;
    }

    [SecurityCritical]
    private unsafe void EtwEnableCallBack(
      [In] ref Guid sourceId,
      [In] int controlCode,
      [In] byte setLevel,
      [In] long anyKeyword,
      [In] long allKeyword,
      [In] UnsafeNativeMethods.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
      [In] void* callbackContext)
    {
      try
      {
        ControllerCommand command = ControllerCommand.Update;
        IDictionary<string, string> arguments = (IDictionary<string, string>) null;
        bool flag = false;
        switch (controlCode)
        {
          case 0:
            this.m_enabled = false;
            this.m_level = (byte) 0;
            this.m_anyKeywordMask = 0L;
            this.m_allKeywordMask = 0L;
            break;
          case 1:
            this.m_enabled = true;
            this.m_level = setLevel;
            this.m_anyKeywordMask = anyKeyword;
            this.m_allKeywordMask = allKeyword;
            break;
          case 2:
            command = ControllerCommand.SendManifest;
            break;
          default:
            return;
        }
        if (flag)
          return;
        this.OnControllerCommand(command, arguments, 0, 0);
      }
      catch (Exception ex)
      {
      }
    }

    protected virtual void OnControllerCommand(
      ControllerCommand command,
      IDictionary<string, string> arguments,
      int sessionId,
      int etwSessionId)
    {
    }

    protected EventLevel Level
    {
      get => (EventLevel) this.m_level;
      set => this.m_level = (byte) value;
    }

    protected EventKeywords MatchAnyKeyword
    {
      get => (EventKeywords) this.m_anyKeywordMask;
      set => this.m_anyKeywordMask = (long) value;
    }

    protected EventKeywords MatchAllKeyword
    {
      get => (EventKeywords) this.m_allKeywordMask;
      set => this.m_allKeywordMask = (long) value;
    }

    private static int FindNull(byte[] buffer, int idx)
    {
      while (idx < buffer.Length && buffer[idx] != (byte) 0)
        ++idx;
      return idx;
    }

    [SecurityCritical]
    private unsafe bool GetDataFromController(
      int etwSessionId,
      UnsafeNativeMethods.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
      out ControllerCommand command,
      out byte[] data,
      out int dataStart)
    {
      data = (byte[]) null;
      dataStart = 0;
      if ((IntPtr) filterData != IntPtr.Zero)
      {
        if (filterData->Ptr != 0L && 0 < filterData->Size && filterData->Size <= 1024)
        {
          data = new byte[filterData->Size];
          Marshal.Copy((IntPtr) filterData->Ptr, data, 0, data.Length);
        }
        command = (ControllerCommand) filterData->Type;
        return true;
      }
      command = ControllerCommand.Update;
      return false;
    }

    public bool IsEnabled() => this.m_enabled;

    public bool IsEnabled(byte level, long keywords) => this.m_enabled && ((int) level <= (int) this.m_level || this.m_level == (byte) 0) && (keywords == 0L || (keywords & this.m_anyKeywordMask) != 0L && (keywords & this.m_allKeywordMask) == this.m_allKeywordMask);

    public static EventProvider.WriteEventErrorCode GetLastWriteEventError() => EventProvider.s_returnCode;

    private static void SetLastError(int error)
    {
      switch (error)
      {
        case 8:
          EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.NoFreeBuffers;
          break;
        case 234:
        case 534:
          EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.EventTooBig;
          break;
      }
    }

    [SecurityCritical]
    private static unsafe object EncodeObject(
      ref object data,
      ref EventProvider.EventData* dataDescriptor,
      ref byte* dataBuffer,
      ref uint totalEventSize)
    {
      string str;
      while (true)
      {
        dataDescriptor->Reserved = 0U;
        str = data as string;
        numArray4 = (byte[]) null;
        if (str == null)
        {
          if (!(data is byte[] numArray4))
          {
            if (!(data is IntPtr))
            {
              if (!(data is int))
              {
                if (!(data is long))
                {
                  if (!(data is uint))
                  {
                    if (!(data is ulong))
                    {
                      if (!(data is char))
                      {
                        if (!(data is byte))
                        {
                          if (!(data is short))
                          {
                            if (!(data is sbyte))
                            {
                              if (!(data is ushort))
                              {
                                if (!(data is float))
                                {
                                  if (!(data is double))
                                  {
                                    if (!(data is bool))
                                    {
                                      if (!(data is Guid))
                                      {
                                        if (!(data is Decimal))
                                        {
                                          if (!(data is DateTime))
                                          {
                                            if (data is Enum)
                                            {
                                              Type underlyingType = Enum.GetUnderlyingType(data.GetType());
                                              if ((object) underlyingType == (object) typeof (int))
                                                data = (object) (int) data;
                                              else if ((object) underlyingType == (object) typeof (long))
                                                data = (object) (long) data;
                                              else
                                                goto label_43;
                                            }
                                            else
                                              goto label_43;
                                          }
                                          else
                                            goto label_35;
                                        }
                                        else
                                          goto label_33;
                                      }
                                      else
                                        goto label_31;
                                    }
                                    else
                                      goto label_29;
                                  }
                                  else
                                    goto label_27;
                                }
                                else
                                  goto label_25;
                              }
                              else
                                goto label_23;
                            }
                            else
                              goto label_21;
                          }
                          else
                            goto label_19;
                        }
                        else
                          goto label_17;
                      }
                      else
                        goto label_15;
                    }
                    else
                      goto label_13;
                  }
                  else
                    goto label_11;
                }
                else
                  goto label_9;
              }
              else
                goto label_7;
            }
            else
              goto label_5;
          }
          else
            goto label_3;
        }
        else
          break;
      }
      dataDescriptor->Size = (uint) ((str.Length + 1) * 2);
      goto label_44;
label_3:
      *(int*) dataBuffer = numArray4.Length;
      dataDescriptor->Ptr = (ulong) dataBuffer;
      dataDescriptor->Size = 4U;
      totalEventSize += dataDescriptor->Size;
      ++dataDescriptor;
      dataBuffer += 16;
      dataDescriptor->Size = (uint) numArray4.Length;
      goto label_44;
label_5:
      dataDescriptor->Size = (uint) sizeof (IntPtr);
      IntPtr* numPtr1 = (IntPtr*) dataBuffer;
      *numPtr1 = (IntPtr) data;
      dataDescriptor->Ptr = (ulong) numPtr1;
      goto label_44;
label_7:
      dataDescriptor->Size = 4U;
      int* numPtr2 = (int*) dataBuffer;
      *numPtr2 = (int) data;
      dataDescriptor->Ptr = (ulong) numPtr2;
      goto label_44;
label_9:
      dataDescriptor->Size = 8U;
      long* numPtr3 = (long*) dataBuffer;
      *numPtr3 = (long) data;
      dataDescriptor->Ptr = (ulong) numPtr3;
      goto label_44;
label_11:
      dataDescriptor->Size = 4U;
      uint* numPtr4 = (uint*) dataBuffer;
      *numPtr4 = (uint) data;
      dataDescriptor->Ptr = (ulong) numPtr4;
      goto label_44;
label_13:
      dataDescriptor->Size = 8U;
      ulong* numPtr5 = (ulong*) dataBuffer;
      *numPtr5 = (ulong) data;
      dataDescriptor->Ptr = (ulong) numPtr5;
      goto label_44;
label_15:
      dataDescriptor->Size = 2U;
      char* chPtr = (char*) dataBuffer;
      *chPtr = (char) data;
      dataDescriptor->Ptr = (ulong) chPtr;
      goto label_44;
label_17:
      dataDescriptor->Size = 1U;
      byte* numPtr6 = dataBuffer;
      *numPtr6 = (byte) data;
      dataDescriptor->Ptr = (ulong) numPtr6;
      goto label_44;
label_19:
      dataDescriptor->Size = 2U;
      short* numPtr7 = (short*) dataBuffer;
      *numPtr7 = (short) data;
      dataDescriptor->Ptr = (ulong) numPtr7;
      goto label_44;
label_21:
      dataDescriptor->Size = 1U;
      sbyte* numPtr8 = (sbyte*) dataBuffer;
      *numPtr8 = (sbyte) data;
      dataDescriptor->Ptr = (ulong) numPtr8;
      goto label_44;
label_23:
      dataDescriptor->Size = 2U;
      ushort* numPtr9 = (ushort*) dataBuffer;
      *numPtr9 = (ushort) data;
      dataDescriptor->Ptr = (ulong) numPtr9;
      goto label_44;
label_25:
      dataDescriptor->Size = 4U;
      float* numPtr10 = (float*) dataBuffer;
      *numPtr10 = (float) data;
      dataDescriptor->Ptr = (ulong) numPtr10;
      goto label_44;
label_27:
      dataDescriptor->Size = 8U;
      double* numPtr11 = (double*) dataBuffer;
      *numPtr11 = (double) data;
      dataDescriptor->Ptr = (ulong) numPtr11;
      goto label_44;
label_29:
      dataDescriptor->Size = 4U;
      int* numPtr12 = (int*) dataBuffer;
      *numPtr12 = !(bool) data ? 0 : 1;
      dataDescriptor->Ptr = (ulong) numPtr12;
      goto label_44;
label_31:
      dataDescriptor->Size = (uint) sizeof (Guid);
      Guid* guidPtr = (Guid*) dataBuffer;
      *guidPtr = (Guid) data;
      dataDescriptor->Ptr = (ulong) guidPtr;
      goto label_44;
label_33:
      dataDescriptor->Size = 16U;
      Decimal* numPtr13 = (Decimal*) dataBuffer;
      *numPtr13 = (Decimal) data;
      dataDescriptor->Ptr = (ulong) numPtr13;
      goto label_44;
label_35:
      long num = 0;
      if (((DateTime) data).Ticks > 504911232000000000L)
        num = ((DateTime) data).ToFileTimeUtc();
      dataDescriptor->Size = 8U;
      long* numPtr14 = (long*) dataBuffer;
      *numPtr14 = num;
      dataDescriptor->Ptr = (ulong) numPtr14;
      goto label_44;
label_43:
      str = data != null ? data.ToString() : "";
      dataDescriptor->Size = (uint) ((str.Length + 1) * 2);
label_44:
      totalEventSize += dataDescriptor->Size;
      ++dataDescriptor;
      dataBuffer += 16;
      return (object) str ?? (object) numArray4;
    }

    [SecurityCritical]
    internal unsafe bool WriteEvent(
      ref EventDescriptor eventDescriptor,
      Guid* activityID,
      Guid* childActivityID,
      params object[] eventPayload)
    {
      int error = 0;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        int length1 = eventPayload.Length;
        if (length1 > 32)
        {
          EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.TooManyArgs;
          return false;
        }
        uint totalEventSize = 0;
        int length2 = 0;
        List<int> intList = new List<int>(8);
        List<object> objectList = new List<object>(8);
        EventProvider.EventData* userData = stackalloc EventProvider.EventData[2 * length1];
        EventProvider.EventData* dataDescriptor = userData;
        byte* numPtr1 = stackalloc byte[32 * length1];
        byte* dataBuffer = numPtr1;
        bool flag = false;
        for (int index = 0; index < eventPayload.Length; ++index)
        {
          if (eventPayload[index] != null)
          {
            object obj = EventProvider.EncodeObject(ref eventPayload[index], ref dataDescriptor, ref dataBuffer, ref totalEventSize);
            if (obj != null)
            {
              int num = (int) (dataDescriptor - userData - 1L);
              if (!(obj is string))
              {
                if (eventPayload.Length + num + 1 - index > 32)
                {
                  EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.TooManyArgs;
                  return false;
                }
                flag = true;
              }
              objectList.Add(obj);
              intList.Add(num);
              ++length2;
            }
          }
          else
          {
            EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.NullInput;
            return false;
          }
        }
        int userDataCount = (int) (dataDescriptor - userData);
        if (totalEventSize > 65482U)
        {
          EventProvider.s_returnCode = EventProvider.WriteEventErrorCode.EventTooBig;
          return false;
        }
        if (!flag && length2 < 8)
        {
          for (; length2 < 8; ++length2)
            objectList.Add((object) null);
          fixed (char* chPtr1 = (string) objectList[0])
            fixed (char* chPtr2 = (string) objectList[1])
              fixed (char* chPtr3 = (string) objectList[2])
                fixed (char* chPtr4 = (string) objectList[3])
                  fixed (char* chPtr5 = (string) objectList[4])
                    fixed (char* chPtr6 = (string) objectList[5])
                      fixed (char* chPtr7 = (string) objectList[6])
                        fixed (char* chPtr8 = (string) objectList[7])
                        {
                          EventProvider.EventData* eventDataPtr = userData;
                          if (objectList[0] != null)
                            eventDataPtr[intList[0]].Ptr = (ulong) chPtr1;
                          if (objectList[1] != null)
                            eventDataPtr[intList[1]].Ptr = (ulong) chPtr2;
                          if (objectList[2] != null)
                            eventDataPtr[intList[2]].Ptr = (ulong) chPtr3;
                          if (objectList[3] != null)
                            eventDataPtr[intList[3]].Ptr = (ulong) chPtr4;
                          if (objectList[4] != null)
                            eventDataPtr[intList[4]].Ptr = (ulong) chPtr5;
                          if (objectList[5] != null)
                            eventDataPtr[intList[5]].Ptr = (ulong) chPtr6;
                          if (objectList[6] != null)
                            eventDataPtr[intList[6]].Ptr = (ulong) chPtr7;
                          if (objectList[7] != null)
                            eventDataPtr[intList[7]].Ptr = (ulong) chPtr8;
                          error = UnsafeNativeMethods.ManifestEtw.EventWriteTransferWrapper(this.m_regHandle, ref eventDescriptor, activityID, childActivityID, userDataCount, userData);
                        }
        }
        else
        {
          EventProvider.EventData* eventDataPtr = userData;
          GCHandle[] gcHandleArray = new GCHandle[length2];
          for (int index = 0; index < length2; ++index)
          {
            gcHandleArray[index] = GCHandle.Alloc(objectList[index], GCHandleType.Pinned);
            if (objectList[index] is string)
            {
              fixed (char* chPtr = (string) objectList[index])
                eventDataPtr[intList[index]].Ptr = (ulong) chPtr;
            }
            else
            {
              fixed (byte* numPtr2 = (byte[]) objectList[index])
                eventDataPtr[intList[index]].Ptr = (ulong) numPtr2;
            }
          }
          error = UnsafeNativeMethods.ManifestEtw.EventWriteTransferWrapper(this.m_regHandle, ref eventDescriptor, activityID, childActivityID, userDataCount, userData);
          for (int index = 0; index < length2; ++index)
            gcHandleArray[index].Free();
        }
      }
      if (error == 0)
        return true;
      EventProvider.SetLastError(error);
      return false;
    }

    [SecurityCritical]
    protected internal unsafe bool WriteEvent(
      ref EventDescriptor eventDescriptor,
      Guid* activityID,
      Guid* childActivityID,
      int dataCount,
      IntPtr data)
    {
      int error = UnsafeNativeMethods.ManifestEtw.EventWriteTransferWrapper(this.m_regHandle, ref eventDescriptor, activityID, childActivityID, dataCount, (EventProvider.EventData*) (void*) data);
      if (error == 0)
        return true;
      EventProvider.SetLastError(error);
      return false;
    }

    [SecurityCritical]
    internal unsafe bool WriteEventRaw(
      ref EventDescriptor eventDescriptor,
      Guid* activityID,
      Guid* relatedActivityID,
      int dataCount,
      IntPtr data)
    {
      int error = UnsafeNativeMethods.ManifestEtw.EventWriteTransferWrapper(this.m_regHandle, ref eventDescriptor, activityID, relatedActivityID, dataCount, (EventProvider.EventData*) (void*) data);
      if (error == 0)
        return true;
      EventProvider.SetLastError(error);
      return false;
    }

    [SecurityCritical]
    private unsafe uint EventRegister(
      ref Guid providerId,
      UnsafeNativeMethods.ManifestEtw.EtwEnableCallback enableCallback)
    {
      this.m_providerId = providerId;
      this.m_etwCallback = enableCallback;
      return UnsafeNativeMethods.ManifestEtw.EventRegister(ref providerId, enableCallback, (void*) null, ref this.m_regHandle);
    }

    [SecurityCritical]
    private uint EventUnregister()
    {
      uint num = UnsafeNativeMethods.ManifestEtw.EventUnregister(this.m_regHandle);
      this.m_regHandle = 0L;
      return num;
    }

    private static int bitcount(uint n)
    {
      int num = 0;
      for (; n != 0U; n >>= 4)
        num += EventProvider.nibblebits[(IntPtr) (n & 15U)];
      return num;
    }

    private static int bitindex(uint n)
    {
      int num = 0;
      while (((long) n & (long) (1 << num)) == 0L)
        ++num;
      return num;
    }

    public struct EventData
    {
      internal ulong Ptr;
      internal uint Size;
      internal uint Reserved;
    }

    public struct SessionInfo
    {
      internal int sessionIdBit;
      internal int etwSessionId;

      internal SessionInfo(int sessionIdBit_, int etwSessionId_)
      {
        this.sessionIdBit = sessionIdBit_;
        this.etwSessionId = etwSessionId_;
      }
    }

    public enum WriteEventErrorCode
    {
      NoError,
      NoFreeBuffers,
      EventTooBig,
      NullInput,
      TooManyArgs,
      Other,
    }
  }
}
