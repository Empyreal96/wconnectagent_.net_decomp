// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Shareable.SystemInformation
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Shareable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 41C560E5-BFB9-47F5-9B09-E868E74DAE2C
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Shareable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Utils.Shareable
{
  public class SystemInformation : ISystemInformation
  {
    public SystemArchitecture Architecture
    {
      get
      {
        string upper = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToUpper(CultureInfo.InvariantCulture);
        if (upper.Contains("ARM"))
          return SystemArchitecture.Arm;
        if (upper.Contains("X86"))
          return SystemArchitecture.X86;
        return upper.Contains("AMD64") ? SystemArchitecture.X64 : SystemArchitecture.Other;
      }
    }
  }
}
