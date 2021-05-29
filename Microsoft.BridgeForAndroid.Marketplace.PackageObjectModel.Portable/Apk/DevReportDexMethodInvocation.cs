// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.DevReportDexMethodInvocation
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class DevReportDexMethodInvocation
  {
    private List<string> allParameters;

    public DevReportDexMethodInvocation(string className, string methodName, string parentMethod)
    {
      if (string.IsNullOrEmpty(className))
        throw new ArgumentException("className must not be null or empty.", nameof (className));
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentException("methodName must not be null or empty.", nameof (methodName));
      this.ClassName = className;
      this.MethodName = methodName;
      this.ParentMethod = parentMethod;
      this.allParameters = new List<string>();
    }

    public string MethodName { get; private set; }

    public string ParentMethod { get; private set; }

    public string ClassName { get; private set; }

    public IReadOnlyList<string> AllParameters => (IReadOnlyList<string>) this.allParameters;

    public void AddParameter(string parameterValue)
    {
      if (parameterValue == null)
        throw new ArgumentNullException(nameof (parameterValue));
      if (string.IsNullOrWhiteSpace(parameterValue))
        throw new ArgumentException("A valid parameter value must be provided.", nameof (parameterValue));
      this.allParameters.Add(parameterValue);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}({2}) ({3})", (object) this.ClassName, (object) this.MethodName, (object) string.Join(", ", (IEnumerable<string>) this.AllParameters), (object) this.ParentMethod);
  }
}
