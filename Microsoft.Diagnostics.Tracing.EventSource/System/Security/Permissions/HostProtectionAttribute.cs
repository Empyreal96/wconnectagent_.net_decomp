// Decompiled with JetBrains decompiler
// Type: System.Security.Permissions.HostProtectionAttribute
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace System.Security.Permissions
{
  internal class HostProtectionAttribute : Attribute
  {
    public bool MayLeakOnAbort { get; set; }
  }
}
