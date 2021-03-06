// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.TraceLoggingDataCollector
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Security;

namespace Microsoft.Diagnostics.Tracing
{
  [SecuritySafeCritical]
  internal class TraceLoggingDataCollector
  {
    internal static readonly TraceLoggingDataCollector Instance = new TraceLoggingDataCollector();

    private TraceLoggingDataCollector()
    {
    }

    public int BeginBufferedArray() => DataCollector.ThreadInstance.BeginBufferedArray();

    public void EndBufferedArray(int bookmark, int count) => DataCollector.ThreadInstance.EndBufferedArray(bookmark, count);

    public TraceLoggingDataCollector AddGroup() => this;

    public unsafe void AddScalar(bool value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 1);

    public unsafe void AddScalar(sbyte value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 1);

    public unsafe void AddScalar(byte value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 1);

    public unsafe void AddScalar(short value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 2);

    public unsafe void AddScalar(ushort value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 2);

    public unsafe void AddScalar(int value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 4);

    public unsafe void AddScalar(uint value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 4);

    public unsafe void AddScalar(long value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 8);

    public unsafe void AddScalar(ulong value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 8);

    public unsafe void AddScalar(IntPtr value) => DataCollector.ThreadInstance.AddScalar((void*) &value, IntPtr.Size);

    public unsafe void AddScalar(UIntPtr value) => DataCollector.ThreadInstance.AddScalar((void*) &value, UIntPtr.Size);

    public unsafe void AddScalar(float value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 4);

    public unsafe void AddScalar(double value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 8);

    public unsafe void AddScalar(char value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 2);

    public unsafe void AddScalar(Guid value) => DataCollector.ThreadInstance.AddScalar((void*) &value, 16);

    public void AddBinary(string value) => DataCollector.ThreadInstance.AddBinary(value, value == null ? 0 : value.Length * 2);

    public void AddBinary(byte[] value) => DataCollector.ThreadInstance.AddBinary((Array) value, value == null ? 0 : value.Length);

    public void AddArray(bool[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 1);

    public void AddArray(sbyte[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 1);

    public void AddArray(short[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 2);

    public void AddArray(ushort[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 2);

    public void AddArray(int[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 4);

    public void AddArray(uint[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 4);

    public void AddArray(long[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 8);

    public void AddArray(ulong[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 8);

    public void AddArray(IntPtr[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, IntPtr.Size);

    public void AddArray(UIntPtr[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, UIntPtr.Size);

    public void AddArray(float[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 4);

    public void AddArray(double[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 8);

    public void AddArray(char[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 2);

    public void AddArray(Guid[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 16);

    public void AddCustom(byte[] value) => DataCollector.ThreadInstance.AddArray((Array) value, value == null ? 0 : value.Length, 1);
  }
}
