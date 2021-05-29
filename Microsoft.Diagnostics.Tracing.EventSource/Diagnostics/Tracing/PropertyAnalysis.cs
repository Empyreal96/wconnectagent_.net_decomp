// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.PropertyAnalysis
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System.Reflection;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class PropertyAnalysis
  {
    internal readonly string name;
    internal readonly MethodInfo getterInfo;
    internal readonly TraceLoggingTypeInfo typeInfo;
    internal readonly EventFieldAttribute fieldAttribute;

    public PropertyAnalysis(
      string name,
      MethodInfo getterInfo,
      TraceLoggingTypeInfo typeInfo,
      EventFieldAttribute fieldAttribute)
    {
      this.name = name;
      this.getterInfo = getterInfo;
      this.typeInfo = typeInfo;
      this.fieldAttribute = fieldAttribute;
    }
  }
}
