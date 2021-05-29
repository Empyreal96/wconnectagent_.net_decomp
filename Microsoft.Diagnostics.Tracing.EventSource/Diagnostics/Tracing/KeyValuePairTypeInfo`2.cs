// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.KeyValuePairTypeInfo`2
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class KeyValuePairTypeInfo<K, V> : TraceLoggingTypeInfo<KeyValuePair<K, V>>
  {
    private readonly TraceLoggingTypeInfo<K> keyInfo;
    private readonly TraceLoggingTypeInfo<V> valueInfo;

    public KeyValuePairTypeInfo(List<Type> recursionCheck)
    {
      this.keyInfo = TraceLoggingTypeInfo<K>.GetInstance(recursionCheck);
      this.valueInfo = TraceLoggingTypeInfo<V>.GetInstance(recursionCheck);
    }

    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      TraceLoggingMetadataCollector collector1 = collector.AddGroup(name);
      this.keyInfo.WriteMetadata(collector1, "Key", EventFieldFormat.Default);
      this.valueInfo.WriteMetadata(collector1, "Value", format);
    }

    public override void WriteData(
      TraceLoggingDataCollector collector,
      ref KeyValuePair<K, V> value)
    {
      K key = value.Key;
      V v = value.Value;
      this.keyInfo.WriteData(collector, ref key);
      this.valueInfo.WriteData(collector, ref v);
    }

    public override object GetData(object value)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      KeyValuePair<K, V> keyValuePair = (KeyValuePair<K, V>) value;
      dictionary.Add("Key", this.keyInfo.GetData((object) keyValuePair.Key));
      dictionary.Add("Value", this.valueInfo.GetData((object) keyValuePair.Value));
      return (object) dictionary;
    }
  }
}
