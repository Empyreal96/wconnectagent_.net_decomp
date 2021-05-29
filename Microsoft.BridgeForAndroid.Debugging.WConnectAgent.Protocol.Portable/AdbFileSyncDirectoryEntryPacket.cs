// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncDirectoryEntryPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public class AdbFileSyncDirectoryEntryPacket : AdbFileSyncPacket
  {
    private AdbFileSyncDirectoryEntryPacket(
      uint mode,
      uint size,
      uint lastModifiedTime,
      string deviceDirectoryName)
    {
      this.Mode = mode;
      this.Size = size;
      this.LastModifiedTime = lastModifiedTime;
      this.DeviceDirectoryName = deviceDirectoryName;
    }

    public uint Mode { get; private set; }

    public uint Size { get; private set; }

    public uint LastModifiedTime { get; private set; }

    public string DeviceDirectoryName { get; private set; }

    public static async Task<AdbFileSyncDirectoryEntryPacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? mode = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (!mode.HasValue)
        return (AdbFileSyncDirectoryEntryPacket) null;
      uint? size = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (!size.HasValue)
        return (AdbFileSyncDirectoryEntryPacket) null;
      uint? time = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (!time.HasValue)
        return (AdbFileSyncDirectoryEntryPacket) null;
      uint? bytes = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (bytes.HasValue)
      {
        uint? nullable = bytes;
        if ((nullable.GetValueOrDefault() != 0U ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
        {
          string name = await AdbFileSyncPacket.ReadStringFromUtf8Async(stream, bytes.Value);
          return name != null ? new AdbFileSyncDirectoryEntryPacket(mode.Value, size.Value, time.Value, name) : (AdbFileSyncDirectoryEntryPacket) null;
        }
      }
      return (AdbFileSyncDirectoryEntryPacket) null;
    }
  }
}
