// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.ILogMessageFormatter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public interface ILogMessageFormatter
  {
    ILogMessage CreateMessage(
      LoggerCore.LogLevels logLevel,
      string message,
      IMessageArg[] messageArgs);

    string GetExceptionMessage(Exception exception);
  }
}
