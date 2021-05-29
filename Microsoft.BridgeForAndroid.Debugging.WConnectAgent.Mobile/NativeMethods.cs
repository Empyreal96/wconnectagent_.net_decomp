// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.NativeMethods
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A3725DA-8D01-4D08-90D4-BC0331796EE5
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  internal static class NativeMethods
  {
    public const int TOKEN_QUERY = 8;

    [DllImport("api-ms-win-core-processthreads-l1-1-2.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool OpenProcessToken(
      IntPtr ProcessHandle,
      int DesiredAccess,
      out IntPtr TokenHandle);

    [DllImport("api-ms-win-core-processthreads-l1-1-2.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("api-ms-win-security-base-l1-2-0.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetTokenInformation(
      IntPtr hToken,
      NativeMethods.TOKEN_INFORMATION_CLASS tokenInfoClass,
      IntPtr TokenInformation,
      int tokeInfoLength,
      out int reqLength);

    [DllImport("api-ms-win-core-handle-l1-1-0.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("api-ms-win-security-sddl-l1-1-0.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ConvertSidToStringSid(IntPtr pSID, [MarshalAs(UnmanagedType.LPTStr)] out string pStringSid);

    [DllImport("ShellChromeAPI.dll", EntryPoint = "Shell_IsLocked", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShellIsLocked();

    public enum TOKEN_INFORMATION_CLASS
    {
      TokenUser = 1,
      TokenGroups = 2,
      TokenPrivileges = 3,
      TokenOwner = 4,
      TokenPrimaryGroup = 5,
      TokenDefaultDacl = 6,
      TokenSource = 7,
      TokenType = 8,
      TokenImpersonationLevel = 9,
      TokenStatistics = 10, // 0x0000000A
      TokenRestrictedSids = 11, // 0x0000000B
      TokenSessionId = 12, // 0x0000000C
      TokenGroupsAndPrivileges = 13, // 0x0000000D
      TokenSessionReference = 14, // 0x0000000E
      TokenSandBoxInert = 15, // 0x0000000F
      TokenAuditPolicy = 16, // 0x00000010
      TokenOrigin = 17, // 0x00000011
      TokenElevationType = 18, // 0x00000012
      TokenLinkedToken = 19, // 0x00000013
      TokenElevation = 20, // 0x00000014
      TokenHasRestrictions = 21, // 0x00000015
      TokenAccessInformation = 22, // 0x00000016
      TokenVirtualizationAllowed = 23, // 0x00000017
      TokenVirtualizationEnabled = 24, // 0x00000018
      TokenIntegrityLevel = 25, // 0x00000019
      TokenUIAccess = 26, // 0x0000001A
      TokenMandatoryPolicy = 27, // 0x0000001B
      TokenLogonSid = 28, // 0x0000001C
      MaxTokenInfoClass = 29, // 0x0000001D
    }

    public struct SID_AND_ATTRIBUTES
    {
      public IntPtr Sid;
      public uint Attributes;
    }

    public struct TOKEN_USER
    {
      public NativeMethods.SID_AND_ATTRIBUTES User;
    }
  }
}
