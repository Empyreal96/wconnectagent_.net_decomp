// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.AowInstanceWrapper
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using AowUser;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  internal class AowInstanceWrapper : IAowInstanceWrapper, IDisposable
  {
    private AoWUserSessionClass userSessionClass;
    private IAoWInstance aowInstance;
    private object lockObj = new object();
    private bool hasDisposed;

    ~AowInstanceWrapper()
    {
      LoggerCore.Log("Destructor is being called!");
      this.Dispose(false);
    }

    public void StartAow()
    {
      lock (this.lockObj)
      {
        if (this.userSessionClass != null)
        {
          LoggerCore.Log("AOW instance already started, nothing to do here");
        }
        else
        {
          try
          {
            this.CreateInstance();
          }
          catch (Exception ex)
          {
            EtwLogger.Instance.ProjectAStartFailure(ex.Message);
            throw new ProjectAStartFailureException(ex);
          }
          try
          {
            this.StartAdbDaemon();
          }
          catch (Exception ex)
          {
            EtwLogger.Instance.DaemonStartFailure(ex.Message);
            throw new AdbdStartFailureException(ex);
          }
        }
      }
    }

    public void ReleaseAow()
    {
      lock (this.lockObj)
      {
        if (this.aowInstance != null)
        {
          Marshal.ReleaseComObject((object) this.aowInstance);
          this.aowInstance = (IAoWInstance) null;
        }
        if (this.userSessionClass == null)
          return;
        this.userSessionClass = (AoWUserSessionClass) null;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public string AndroidPackageToWindowsPackage(string androidPackage)
    {
      lock (this.lockObj)
      {
        if (this.aowInstance == null)
          throw new InvalidOperationException("StartAow() must be called first.");
        LoggerCore.Log("Resolving {0} to APPX Full Package Name.", (object) androidPackage);
        IAoWSupportHandler supportHandler = this.aowInstance.GetSupportHandler();
        IntPtr num = IntPtr.Zero;
        try
        {
          num = Marshal.GetComInterfaceForObject((object) supportHandler, typeof (IAoWSupportHandler));
          AowInstanceWrapper.Impersonate(num);
          return AowInstanceWrapper.GetPackageFullName(androidPackage, supportHandler);
        }
        finally
        {
          if (num != IntPtr.Zero)
            Marshal.Release(num);
        }
      }
    }

    private static string GetPackageFullName(
      string androidPackage,
      IAoWSupportHandler supportHandler)
    {
      string pPackageFullName = string.Empty;
      supportHandler.AndroidPackageToWindows(androidPackage, out pPackageFullName);
      if (!string.IsNullOrWhiteSpace(pPackageFullName))
      {
        LoggerCore.Log("Resolved Android Package: {0} to APPX FullName: {1}.", (object) androidPackage, (object) pPackageFullName);
        return pPackageFullName;
      }
      LoggerCore.Log("{0} package does not exist.", (object) androidPackage);
      return (string) null;
    }

    private static void Impersonate(IntPtr interfacePointer)
    {
      if (Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods.CoSetProxyBlanket(interfacePointer, Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods.RPC_C_AUTHN.DEFAULT, Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods.RPC_C_AUTHZ.DEFAULT, (string) null, Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods.RPC_C_AUTHN_LEVEL.DEFAULT, Microsoft.Arcadia.Debugging.AdbAgentExe.Mobile.NativeMethods.RPC_C_IMP_LEVEL.IMPERSONATE, IntPtr.Zero, 32U) != 0)
        throw new InvalidOperationException("CoSetProxyBlanket() failed.");
    }

    private void Dispose(bool disposing)
    {
      if (this.hasDisposed)
        return;
      this.ReleaseAow();
      this.hasDisposed = true;
    }

    private void CreateInstance()
    {
      LoggerCore.Log("Creating new instance of AoWUserSessionClass");
      this.userSessionClass = new AoWUserSessionClass();
      byte[] byteArray = new Guid("292997C8-FA0D-4FB6-8BC5-366F9382CC81").ToByteArray();
      GUID instanceIid = new GUID();
      instanceIid.Data1 = BitConverter.ToUInt32(byteArray, 0);
      instanceIid.Data2 = BitConverter.ToUInt16(byteArray, 4);
      instanceIid.Data3 = BitConverter.ToUInt16(byteArray, 6);
      instanceIid.Data4 = new byte[8];
      Buffer.BlockCopy((Array) byteArray, 8, (Array) instanceIid.Data4, 0, 8);
      LoggerCore.Log("Calling StartDefaultInstance");
      IntPtr ppInstance;
      this.userSessionClass.StartDefaultInstance(ref instanceIid, out ppInstance);
      LoggerCore.Log("Marshalling the object");
      AowInstanceWrapper.Impersonate(ppInstance);
      this.aowInstance = Marshal.GetObjectForIUnknown(ppInstance) as IAoWInstance;
    }

    private void StartAdbDaemon()
    {
      if (this.aowInstance is IInteropServiceProvider aowInstance)
      {
        Guid guid1 = new Guid("8CF5FB9E-1601-4C3D-BB3C-D47DF3976E13");
        Guid guid2 = new Guid("c60b7e58-8fe7-4a9c-a86e-42ca6926cc40");
        object obj = (object) null;
        Guid guid3 = guid1;
        ref Guid local1 = ref guid3;
        Guid guid4 = guid2;
        ref Guid local2 = ref guid4;
        ref object local3 = ref obj;
        aowInstance.QueryService(ref local1, ref local2, out local3);
        (obj as IAoWDebugService).StartDebugging();
      }
      else
        LoggerCore.Log("The Aow framework is out of data");
    }
  }
}
