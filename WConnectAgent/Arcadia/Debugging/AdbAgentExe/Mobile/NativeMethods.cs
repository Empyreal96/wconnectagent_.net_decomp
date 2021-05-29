// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile
{
  internal static class NativeMethods
  {
    public const uint EOAC_STATIC_CLOAKING = 32;

    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", Justification = "Necessary.")]
    [DllImport("api-ms-win-core-com-l1-1-1.dll", CharSet = CharSet.Unicode)]
    internal static extern int CoSetProxyBlanket(
      IntPtr proxy,
      NativeMethods.RPC_C_AUTHN authenticationService,
      NativeMethods.RPC_C_AUTHZ authorizationService,
      string serverPrincipalName,
      NativeMethods.RPC_C_AUTHN_LEVEL authenticationLevel,
      NativeMethods.RPC_C_IMP_LEVEL impersonationLevel,
      IntPtr authInfo,
      uint capabilities);

    internal enum RPC_C_AUTHN : uint
    {
      NONE = 0,
      DCE_PRIVATE = 1,
      DCE_PUBLIC = 2,
      DEC_PUBLIC = 4,
      GSS_NEGOTIATE = 9,
      WINNT = 10, // 0x0000000A
      GSS_SCHANNEL = 14, // 0x0000000E
      GSS_KERBEROS = 16, // 0x00000010
      DPA = 17, // 0x00000011
      MSN = 18, // 0x00000012
      DIGEST = 21, // 0x00000015
      MQ = 100, // 0x00000064
      DEFAULT = 4294967295, // 0xFFFFFFFF
    }

    internal enum RPC_C_AUTHN_LEVEL : uint
    {
      DEFAULT,
      NONE,
      CONNECT,
      CALL,
      PKT,
      PKT_INTEGRITY,
      PKT_PRIVACY,
    }

    internal enum RPC_C_AUTHZ : uint
    {
      NONE = 0,
      NAME = 1,
      DCE = 2,
      DEFAULT = 4294967295, // 0xFFFFFFFF
    }

    internal enum RPC_C_IMP_LEVEL
    {
      DEFAULT,
      ANONYMOUS,
      IDENTIFY,
      IMPERSONATE,
      DELEGATE,
    }
  }
}
