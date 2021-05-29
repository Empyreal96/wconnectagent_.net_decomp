// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncDonePacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public sealed class AdbFileSyncDonePacket : AdbFileSyncPacket
  {
    private AdbFileSyncDonePacket(uint lastModifiedTime) => this.LastModifiedTime = lastModifiedTime;

    public uint LastModifiedTime { get; private set; }

    public static async Task<AdbFileSyncDonePacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? time = await AdbFileSyncPacket.ReadUnitAsync(stream);
      return time.HasValue ? new AdbFileSyncDonePacket(time.Value) : (AdbFileSyncDonePacket) null;
    }
  }
}
