// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.MemoryPipe
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  internal sealed class MemoryPipe
  {
    private object lockObject = new object();
    private IList<byte[]> dataPieces = (IList<byte[]>) new List<byte[]>();
    private int head;

    public void Write(byte[] buffer) => this.Write(buffer, 0, buffer.Length);

    public void Write(byte[] buffer, int start, int bytes)
    {
      BufferHelper.CheckAccessRange(buffer, start, bytes);
      byte[] numArray = new byte[bytes];
      Array.Copy((Array) buffer, start, (Array) numArray, 0, bytes);
      lock (this.lockObject)
        this.dataPieces.Add(numArray);
    }

    public int Read(byte[] buffer, int startIndex, int bytesToRead)
    {
      BufferHelper.CheckAccessRange(buffer, startIndex, bytesToRead);
      int destinationIndex = startIndex;
      int val2 = bytesToRead;
      lock (this.lockObject)
      {
        while (this.dataPieces.Count != 0)
        {
          int length = Math.Min(this.dataPieces[0].Length - this.head, val2);
          Array.Copy((Array) this.dataPieces[0], this.head, (Array) buffer, destinationIndex, length);
          destinationIndex += length;
          val2 -= length;
          this.head += length;
          if (this.head == this.dataPieces[0].Length)
          {
            this.dataPieces.RemoveAt(0);
            this.head = 0;
          }
          if (val2 == 0)
            break;
        }
      }
      return bytesToRead - val2;
    }
  }
}
