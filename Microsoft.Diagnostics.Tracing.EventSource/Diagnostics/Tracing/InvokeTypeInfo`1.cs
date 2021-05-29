// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.InvokeTypeInfo`1
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class InvokeTypeInfo<ContainerType> : TraceLoggingTypeInfo<ContainerType>
  {
    private readonly PropertyAnalysis[] properties;
    private readonly PropertyAccessor<ContainerType>[] accessors;

    public InvokeTypeInfo(TypeAnalysis typeAnalysis)
      : base(typeAnalysis.name, typeAnalysis.level, typeAnalysis.opcode, typeAnalysis.keywords, typeAnalysis.tags)
    {
      if (typeAnalysis.properties.Length == 0)
        return;
      this.properties = typeAnalysis.properties;
      this.accessors = new PropertyAccessor<ContainerType>[this.properties.Length];
      for (int index = 0; index < this.accessors.Length; ++index)
        this.accessors[index] = PropertyAccessor<ContainerType>.Create(this.properties[index]);
    }

    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      TraceLoggingMetadataCollector collector1 = collector.AddGroup(name);
      if (this.properties == null)
        return;
      foreach (PropertyAnalysis property in this.properties)
      {
        EventFieldFormat format1 = EventFieldFormat.Default;
        EventFieldAttribute fieldAttribute = property.fieldAttribute;
        if (fieldAttribute != null)
        {
          collector1.Tags = fieldAttribute.Tags;
          format1 = fieldAttribute.Format;
        }
        property.typeInfo.WriteMetadata(collector1, property.name, format1);
      }
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref ContainerType value)
    {
      if (this.accessors == null)
        return;
      foreach (PropertyAccessor<ContainerType> accessor in this.accessors)
        accessor.Write(collector, ref value);
    }

    public override object GetData(object value)
    {
      if (this.properties == null)
        return (object) null;
      List<string> payloadNames = new List<string>();
      List<object> payloadValues = new List<object>();
      for (int index = 0; index < this.properties.Length; ++index)
      {
        object data = this.accessors[index].GetData((ContainerType) value);
        payloadNames.Add(this.properties[index].name);
        payloadValues.Add(this.properties[index].typeInfo.GetData(data));
      }
      return (object) new EventPayload(payloadNames, payloadValues);
    }

    public override void WriteObjectData(TraceLoggingDataCollector collector, object valueObj)
    {
      if (this.accessors == null)
        return;
      ContainerType containerType = valueObj == null ? default (ContainerType) : (ContainerType) valueObj;
      this.WriteData(collector, ref containerType);
    }
  }
}
