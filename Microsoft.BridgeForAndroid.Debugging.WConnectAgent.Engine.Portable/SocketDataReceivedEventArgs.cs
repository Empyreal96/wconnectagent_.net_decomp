// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.SocketDataReceivedEventArgs
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class SocketDataReceivedEventArgs : EventArgs
  {
    public SocketDataReceivedEventArgs(byte[] buffer, int start, int bytes)
    {
      BufferHelper.CheckAccessRange(buffer, start, bytes);
      byte[] numArray = new byte[bytes];
      Array.Copy((Array) buffer, start, (Array) numArray, 0, bytes);
      this.Data = numArray;
    }

    public SocketDataReceivedEventArgs(byte[] data) => this.Data = data != null && data.Length != 0 ? data : throw new ArgumentException("data array should not be null or empty", nameof (data));

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Array property is more appropriate in this case")]
    public byte[] Data { get; private set; }
  }
}
