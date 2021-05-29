// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncStatPacketFromClient
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public class AdbFileSyncStatPacketFromClient : AdbFileSyncPacket
  {
    public AdbFileSyncStatPacketFromClient(string deviceFilePath) => this.DeviceFilePath = deviceFilePath;

    public string DeviceFilePath { get; private set; }

    public static async Task<AdbFileSyncStatPacketFromClient> ReadBodyAsync(
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
          return data != null ? new AdbFileSyncStatPacketFromClient(data) : (AdbFileSyncStatPacketFromClient) null;
        }
      }
      return (AdbFileSyncStatPacketFromClient) null;
    }

    public override async Task<bool> WriteAsync(IStreamWriter stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      byte[] data = Encoding.UTF8.GetBytes(this.DeviceFilePath);
      byte[] packet = new byte[8 + data.Length];
      IntegerHelper.WriteUintToLittleEndianBytes(1413567571U, packet, 0);
      IntegerHelper.WriteUintToLittleEndianBytes((uint) data.Length, packet, 4);
      data.CopyTo((Array) packet, 8);
      int bytesWritten = await stream.WriteAsync(packet, 0, packet.Length);
      return bytesWritten == packet.Length;
    }
  }
}
