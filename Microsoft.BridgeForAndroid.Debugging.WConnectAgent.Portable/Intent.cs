// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.Intent
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class Intent
  {
    public const string ActionDelete = "android.intent.action.DELETE";

    public bool IsExplicitIntent => this.PackageName != null && this.ActivityName != null;

    public bool IsUnsupportedIntent => !this.IsExplicitIntent;

    public string Action { get; set; }

    public string Category { get; set; }

    public string PackageName { get; set; }

    public string ActivityName { get; set; }

    public Uri DataUri { get; set; }

    public bool HasDataFlag => this.DataUri != (Uri) null;
  }
}
