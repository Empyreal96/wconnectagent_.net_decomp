// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.TraceLoggingTypeInfo
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  internal abstract class TraceLoggingTypeInfo
  {
    private readonly string name;
    private readonly EventKeywords keywords;
    private readonly EventLevel level = ~EventLevel.LogAlways;
    private readonly EventOpcode opcode = ~EventOpcode.Info;
    private readonly EventTags tags;
    private readonly Type dataType;

    internal TraceLoggingTypeInfo(Type dataType)
    {
      this.name = (object) dataType != null ? dataType.Name : throw new ArgumentNullException(nameof (dataType));
      this.dataType = dataType;
    }

    internal TraceLoggingTypeInfo(
      Type dataType,
      string name,
      EventLevel level,
      EventOpcode opcode,
      EventKeywords keywords,
      EventTags tags)
    {
      if ((object) dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      if (name == null)
        throw new ArgumentNullException("eventName");
      Statics.CheckName(name);
      this.name = name;
      this.keywords = keywords;
      this.level = level;
      this.opcode = opcode;
      this.tags = tags;
      this.dataType = dataType;
    }

    public string Name => this.name;

    public EventLevel Level => this.level;

    public EventOpcode Opcode => this.opcode;

    public EventKeywords Keywords => this.keywords;

    public EventTags Tags => this.tags;

    internal Type DataType => this.dataType;

    public abstract void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format);

    public abstract void WriteObjectData(TraceLoggingDataCollector collector, object value);

    public virtual object GetData(object value) => value;
  }
}
