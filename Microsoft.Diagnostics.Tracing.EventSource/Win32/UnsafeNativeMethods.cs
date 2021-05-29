// Decompiled with JetBrains decompiler
// Type: Microsoft.Win32.UnsafeNativeMethods
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using Microsoft.Diagnostics.Tracing;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32
{
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethods
  {
    private const string EventingProviderApiSet = "api-ms-win-eventing-provider-l1-1-0";
    private const string EventingControllerApiSet = "api-ms-win-eventing-controller-l1-1-0";

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal static class ManifestEtw
    {
      internal const int ERROR_ARITHMETIC_OVERFLOW = 534;
      internal const int ERROR_NOT_ENOUGH_MEMORY = 8;
      internal const int ERROR_MORE_DATA = 234;
      internal const int ERROR_NOT_SUPPORTED = 50;
      internal const int ERROR_INVALID_PARAMETER = 87;
      internal const int EVENT_CONTROL_CODE_DISABLE_PROVIDER = 0;
      internal const int EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
      internal const int EVENT_CONTROL_CODE_CAPTURE_STATE = 2;

      [SecurityCritical]
      [DllImport("api-ms-win-eventing-provider-l1-1-0", CharSet = CharSet.Unicode)]
      internal static extern unsafe uint EventRegister(
        [In] ref Guid providerId,
        [In] UnsafeNativeMethods.ManifestEtw.EtwEnableCallback enableCallback,
        [In] void* callbackContext,
        [In, Out] ref long registrationHandle);

      [SecurityCritical]
      [DllImport("api-ms-win-eventing-provider-l1-1-0", CharSet = CharSet.Unicode)]
      internal static extern uint EventUnregister([In] long registrationHandle);

      internal static unsafe int EventWriteTransferWrapper(
        long registrationHandle,
        ref EventDescriptor eventDescriptor,
        Guid* activityId,
        Guid* relatedActivityId,
        int userDataCount,
        EventProvider.EventData* userData)
      {
        int num = UnsafeNativeMethods.ManifestEtw.EventWriteTransfer(registrationHandle, ref eventDescriptor, activityId, relatedActivityId, userDataCount, userData);
        if (num == 87 && (IntPtr) relatedActivityId == IntPtr.Zero)
        {
          Guid empty = Guid.Empty;
          num = UnsafeNativeMethods.ManifestEtw.EventWriteTransfer(registrationHandle, ref eventDescriptor, activityId, &empty, userDataCount, userData);
        }
        return num;
      }

      [SuppressUnmanagedCodeSecurity]
      [DllImport("api-ms-win-eventing-provider-l1-1-0", CharSet = CharSet.Unicode)]
      private static extern unsafe int EventWriteTransfer(
        [In] long registrationHandle,
        [In] ref EventDescriptor eventDescriptor,
        [In] Guid* activityId,
        [In] Guid* relatedActivityId,
        [In] int userDataCount,
        [In] EventProvider.EventData* userData);

      [SuppressUnmanagedCodeSecurity]
      [DllImport("api-ms-win-eventing-provider-l1-1-0", CharSet = CharSet.Unicode)]
      internal static extern int EventActivityIdControl(
        [In] UnsafeNativeMethods.ManifestEtw.ActivityControl ControlCode,
        [In, Out] ref Guid ActivityId);

      [SuppressUnmanagedCodeSecurity]
      [DllImport("api-ms-win-eventing-provider-l1-1-0", CharSet = CharSet.Unicode)]
      internal static extern unsafe int EventSetInformation(
        [In] long registrationHandle,
        [In] UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS informationClass,
        [In] void* eventInformation,
        [In] int informationLength);

      [SecurityCritical]
      internal unsafe delegate void EtwEnableCallback(
        [In] ref Guid sourceId,
        [In] int isEnabled,
        [In] byte level,
        [In] long matchAnyKeywords,
        [In] long matchAllKeywords,
        [In] UnsafeNativeMethods.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
        [In] void* callbackContext);

      internal struct EVENT_FILTER_DESCRIPTOR
      {
        public long Ptr;
        public int Size;
        public int Type;
      }

      internal enum ActivityControl : uint
      {
        EVENT_ACTIVITY_CTRL_GET_ID = 1,
        EVENT_ACTIVITY_CTRL_SET_ID = 2,
        EVENT_ACTIVITY_CTRL_CREATE_ID = 3,
        EVENT_ACTIVITY_CTRL_GET_SET_ID = 4,
        EVENT_ACTIVITY_CTRL_CREATE_SET_ID = 5,
      }

      internal enum EVENT_INFO_CLASS
      {
        BinaryTrackInfo,
        SetEnableAllKeywords,
        SetTraits,
      }
    }
  }
}
