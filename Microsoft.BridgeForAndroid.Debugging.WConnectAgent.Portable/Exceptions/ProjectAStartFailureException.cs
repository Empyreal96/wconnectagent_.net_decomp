// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions.ProjectAStartFailureException
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
  public class ProjectAStartFailureException : Exception
  {
    public ProjectAStartFailureException()
      : base("Failure starting ProjectA.")
    {
    }

    public ProjectAStartFailureException(string message)
      : base(message)
    {
    }

    public ProjectAStartFailureException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ProjectAStartFailureException(Exception ex)
      : base("Failure starting ProjectA.", ex)
    {
    }
  }
}
