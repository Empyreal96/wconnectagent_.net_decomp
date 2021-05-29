// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.CalledTrackedServiceMethodDetails
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class CalledTrackedServiceMethodDetails
  {
    public CalledTrackedServiceMethodDetails(
      string serviceName,
      string serviceMethodPackageName,
      string serviceMethodCategory,
      string serviceMethodClassName,
      string serviceMethodName,
      string calledByMiddleware,
      ClassMemberStatus serviceMethodStatus,
      string methodNameCallingThisServiceMethod)
    {
      this.ServiceName = serviceName;
      this.ServiceMethodPackageName = serviceMethodPackageName;
      this.ServiceMethodCategory = serviceMethodCategory;
      this.ServiceMethodClassName = serviceMethodClassName;
      this.ServiceMethodName = serviceMethodName;
      this.CalledByMiddleware = calledByMiddleware;
      this.ServiceMethodStatus = serviceMethodStatus;
      this.MethodNameCallingThisServiceMethod = methodNameCallingThisServiceMethod;
    }

    public string ServiceName { get; private set; }

    public string ServiceMethodPackageName { get; private set; }

    public string ServiceMethodCategory { get; private set; }

    public string ServiceMethodClassName { get; private set; }

    public string ServiceMethodName { get; private set; }

    public string CalledByMiddleware { get; private set; }

    public ClassMemberStatus ServiceMethodStatus { get; private set; }

    public string MethodNameCallingThisServiceMethod { get; private set; }

    public override string ToString()
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", new object[3]
      {
        (object) this.ServiceMethodPackageName,
        (object) this.ServiceMethodClassName,
        (object) this.ServiceMethodName
      });
      if (string.IsNullOrWhiteSpace(this.CalledByMiddleware))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", new object[2]
        {
          (object) str,
          (object) this.CalledByMiddleware
        });
      return str;
    }
  }
}
