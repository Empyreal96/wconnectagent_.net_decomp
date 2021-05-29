// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncStatPacketFromServer
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public class AdbFileSyncStatPacketFromServer : AdbFileSyncPacket
  {
    private AdbFileSyncStatPacketFromServer(uint mode, uint size, uint lastModifedTime)
    {
      this.Mode = mode;
      this.Size = size;
      this.LastModifiedTime = lastModifedTime;
    }

    public uint Mode { get; private set; }

    public uint Size { get; private set; }

    public uint LastModifiedTime { get; private set; }

    public static async Task<AdbFileSyncStatPacketFromServer> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? mode = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (!mode.HasValue)
        return (AdbFileSyncStatPacketFromServer) null;
      uint? size = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (!size.HasValue)
        return (AdbFileSyncStatPacketFromServer) null;
      uint? time = await AdbFileSyncPacket.ReadUnitAsync(stream);
      return time.HasValue ? new AdbFileSyncStatPacketFromServer(mode.Value, size.Value, time.Value) : (AdbFileSyncStatPacketFromServer) null;
    }
  }
}
