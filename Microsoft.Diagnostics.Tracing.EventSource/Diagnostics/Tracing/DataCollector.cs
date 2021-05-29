// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.DataCollector
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Diagnostics.Tracing
{
  [SecurityCritical]
  internal struct DataCollector
  {
    [ThreadStatic]
    internal static DataCollector ThreadInstance;
    private unsafe byte* scratchEnd;
    private unsafe EventSource.EventData* datasEnd;
    private unsafe GCHandle* pinsEnd;
    private unsafe EventSource.EventData* datasStart;
    private unsafe byte* scratch;
    private unsafe EventSource.EventData* datas;
    private unsafe GCHandle* pins;
    private byte[] buffer;
    private int bufferPos;
    private int bufferNesting;
    private bool writingScalars;

    internal unsafe void Enable(
      byte* scratch,
      int scratchSize,
      EventSource.EventData* datas,
      int dataCount,
      GCHandle* pins,
      int pinCount)
    {
      this.datasStart = datas;
      this.scratchEnd = scratch + scratchSize;
      this.datasEnd = datas + dataCount;
      this.pinsEnd = pins + pinCount;
      this.scratch = scratch;
      this.datas = datas;
      this.pins = pins;
      this.writingScalars = false;
    }

    internal unsafe void Disable() => *(DataCollector*) ref this = new DataCollector();

    internal unsafe EventSource.EventData* Finish()
    {
      this.ScalarsEnd();
      return this.datas;
    }

    internal unsafe void AddScalar(void* value, int size)
    {
      byte* numPtr1 = (byte*) value;
      if (this.bufferNesting == 0)
      {
        byte* scratch = this.scratch;
        byte* numPtr2 = scratch + size;
        if (this.scratchEnd < numPtr2)
          throw new IndexOutOfRangeException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_AddScalarOutOfRange"));
        this.ScalarsBegin();
        this.scratch = numPtr2;
        for (int index = 0; index != size; ++index)
          scratch[index] = numPtr1[index];
      }
      else
      {
        int bufferPos = this.bufferPos;
        checked { this.bufferPos += size; }
        this.EnsureBuffer();
        int index = 0;
        while (index != size)
        {
          this.buffer[bufferPos] = numPtr1[index];
          ++index;
          ++bufferPos;
        }
      }
    }

    internal unsafe void AddBinary(string value, int size)
    {
      if (size > (int) ushort.MaxValue)
        size = 65534;
      if (this.bufferNesting != 0)
        this.EnsureBuffer(size + 2);
      this.AddScalar((void*) &size, 2);
      if (size == 0)
        return;
      if (this.bufferNesting == 0)
      {
        this.ScalarsEnd();
        this.PinArray((object) value, size);
      }
      else
      {
        int bufferPos = this.bufferPos;
        checked { this.bufferPos += size; }
        this.EnsureBuffer();
        fixed (char* chPtr = value)
          Marshal.Copy((IntPtr) (void*) chPtr, this.buffer, bufferPos, size);
      }
    }

    internal void AddBinary(Array value, int size) => this.AddArray(value, size, 1);

    internal unsafe void AddArray(Array value, int length, int itemSize)
    {
      if (length > (int) ushort.MaxValue)
        length = (int) ushort.MaxValue;
      int num = length * itemSize;
      if (this.bufferNesting != 0)
        this.EnsureBuffer(num + 2);
      this.AddScalar((void*) &length, 2);
      if (length == 0)
        return;
      if (this.bufferNesting == 0)
      {
        this.ScalarsEnd();
        this.PinArray((object) value, num);
      }
      else
      {
        int bufferPos = this.bufferPos;
        checked { this.bufferPos += num; }
        this.EnsureBuffer();
        Buffer.BlockCopy(value, 0, (Array) this.buffer, bufferPos, num);
      }
    }

    internal int BeginBufferedArray()
    {
      this.BeginBuffered();
      this.bufferPos += 2;
      return this.bufferPos;
    }

    internal void EndBufferedArray(int bookmark, int count)
    {
      this.EnsureBuffer();
      this.buffer[bookmark - 2] = (byte) count;
      this.buffer[bookmark - 1] = (byte) (count >> 8);
      this.EndBuffered();
    }

    internal void BeginBuffered()
    {
      this.ScalarsEnd();
      ++this.bufferNesting;
    }

    internal void EndBuffered()
    {
      --this.bufferNesting;
      if (this.bufferNesting != 0)
        return;
      this.EnsureBuffer();
      this.PinArray((object) this.buffer, this.bufferPos);
      this.buffer = (byte[]) null;
      this.bufferPos = 0;
    }

    private void EnsureBuffer()
    {
      int bufferPos = this.bufferPos;
      if (this.buffer != null && this.buffer.Length >= bufferPos)
        return;
      this.GrowBuffer(bufferPos);
    }

    private void EnsureBuffer(int additionalSize)
    {
      int required = this.bufferPos + additionalSize;
      if (this.buffer != null && this.buffer.Length >= required)
        return;
      this.GrowBuffer(required);
    }

    private void GrowBuffer(int required)
    {
      int newSize = this.buffer == null ? 64 : this.buffer.Length;
      do
      {
        newSize *= 2;
      }
      while (newSize < required);
      Array.Resize<byte>(ref this.buffer, newSize);
    }

    private unsafe void PinArray(object value, int size)
    {
      GCHandle* pins = this.pins;
      if (this.pinsEnd <= pins)
        throw new IndexOutOfRangeException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_PinArrayOutOfRange"));
      EventSource.EventData* datas = this.datas;
      if (this.datasEnd <= datas)
        throw new IndexOutOfRangeException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_DataDescriptorsOutOfRange"));
      this.pins = pins + 1;
      this.datas = datas + 1;
      *pins = GCHandle.Alloc(value, GCHandleType.Pinned);
      datas->m_Ptr = (long) (ulong) (UIntPtr) (void*) pins->AddrOfPinnedObject();
      datas->m_Size = size;
    }

    private unsafe void ScalarsBegin()
    {
      if (this.writingScalars)
        return;
      EventSource.EventData* datas = this.datas;
      if (this.datasEnd <= datas)
        throw new IndexOutOfRangeException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_DataDescriptorsOutOfRange"));
      datas->m_Ptr = (long) (ulong) (UIntPtr) (void*) this.scratch;
      this.writingScalars = true;
    }

    private unsafe void ScalarsEnd()
    {
      if (!this.writingScalars)
        return;
      EventSource.EventData* datas = this.datas;
      datas->m_Size = checked ((int) unchecked ((long) ((IntPtr) (this.scratch - checked ((UIntPtr) (ulong) datas->m_Ptr)) / 1)));
      this.datas = datas + 1;
      this.writingScalars = false;
    }
  }
}
