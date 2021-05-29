// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.IStreamWriter
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public interface IStreamWriter
  {
    Task<int> WriteAsync(byte[] buffer, int startIndex, int bytesToWrite);
  }
}
