// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncSendPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public sealed class AdbFileSyncSendPacket : AdbFileSyncPacket
  {
    private AdbFileSyncSendPacket(string deviceFilePath, uint permission)
    {
      this.DeviceFilePath = deviceFilePath;
      this.Permission = permission;
    }

    public string DeviceFilePath { get; private set; }

    public uint Permission { get; private set; }

    public static async Task<AdbFileSyncSendPacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? bytes = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (bytes.HasValue)
      {
        uint? nullable = bytes;
        if ((nullable.GetValueOrDefault() != 0U ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
        {
          string data = await AdbFileSyncPacket.ReadStringFromUtf8Async(stream, bytes.Value);
          if (data == null)
            return (AdbFileSyncSendPacket) null;
          string[] subStrings = data.Split(',');
          uint permission;
          return subStrings.Length == 2 ? (uint.TryParse(subStrings[1], out permission) ? new AdbFileSyncSendPacket(subStrings[0], permission) : (AdbFileSyncSendPacket) null) : (AdbFileSyncSendPacket) null;
        }
      }
      return (AdbFileSyncSendPacket) null;
    }
  }
}
