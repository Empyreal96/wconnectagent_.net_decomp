// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkDetails
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ApkDetails : IPackageDetails
  {
    public ApkDetails(string apkFileName) => this.PackageName = !string.IsNullOrWhiteSpace(apkFileName) ? apkFileName : throw new ArgumentException("Must not be null or empty.", nameof (apkFileName));

    public string PackageName { get; private set; }

    public Task<Stream> RetrievePackageStreamAsync() => throw new NotImplementedException();
  }
}
