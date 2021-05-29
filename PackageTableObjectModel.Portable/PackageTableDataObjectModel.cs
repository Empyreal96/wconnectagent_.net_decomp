// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageTableObjectModel.Portable.PackageTableDataObjectModel
// Assembly: PackageTableObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E5ED006-7CC7-4295-9681-03F709C9E411
// Assembly location: .\\AowDebugger\Agent\PackageTableObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.PackageTableObjectModel.Portable
{
  public sealed class PackageTableDataObjectModel
  {
    private const int TimeoutInMilliseconds = 5000;
    private const string BadgingArguments = " d badging ";
    private string packageTableData;
    private string pathToApkFile;
    private string pathToAaptTool;
    private object uniqueLock = new object();

    public PackageTableDataObjectModel(string pathToApkFile, string pathToAaptTool)
    {
      this.pathToApkFile = pathToApkFile;
      this.pathToAaptTool = pathToAaptTool;
    }

    private void GetPackageTableData()
    {
      using (IProcessRunner processRunner = PortableUtilsServiceLocator.ProcessRunnerFactory.Create())
      {
        processRunner.ExePath = this.pathToAaptTool;
        processRunner.Arguments = " d badging " + this.pathToApkFile;
        processRunner.StandardErrorEncoding = Encoding.UTF8;
        processRunner.StandardOutputEncoding = Encoding.UTF8;
        if (!processRunner.RunAndWait(5000))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Aapt tool took longer than {0} seconds to complete.", new object[1]
          {
            (object) 5
          }));
        StringBuilder stringBuilder1 = new StringBuilder();
        StringBuilder stringBuilder2 = new StringBuilder();
        if (processRunner.SupportsStandardOutputRedirection)
        {
          foreach (string str in (IEnumerable<string>) processRunner.StandardOutput)
            stringBuilder1.AppendLine(str);
        }
        if (processRunner.SupportsStandardErrorRedirection)
        {
          foreach (string str in (IEnumerable<string>) processRunner.StandardError)
            stringBuilder2.AppendLine(str);
        }
        int? exitCode = processRunner.ExitCode;
        if ((exitCode.GetValueOrDefault() != 0 ? 1 : (!exitCode.HasValue ? 1 : 0)) != 0)
          stringBuilder1.AppendLine(stringBuilder2.ToString());
        this.packageTableData = stringBuilder1.ToString();
      }
    }

    public string PackageTableDataAsString
    {
      get
      {
        lock (this.uniqueLock)
        {
          if (this.packageTableData == null)
            this.GetPackageTableData();
        }
        return this.packageTableData;
      }
      private set => this.packageTableData = value;
    }
  }
}
