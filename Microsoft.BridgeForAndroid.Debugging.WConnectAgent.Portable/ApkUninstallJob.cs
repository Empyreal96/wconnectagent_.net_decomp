// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkUninstallJob
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ApkUninstallJob : ShellChannelJob
  {
    private IFactory factory;
    private ShellPmUninstallParam uninstallParameters;

    public ApkUninstallJob(IFactory factory, ShellPmUninstallParam uninstallParams)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (uninstallParams == null)
        throw new ArgumentNullException(nameof (uninstallParams));
      this.factory = factory;
      this.uninstallParameters = uninstallParams;
      this.IsWithinInteractiveShell = uninstallParams.FromInteractiveShell;
    }

    protected override async Task<string> OnExecuteShellCommand()
    {
      if (!this.uninstallParameters.IsPackageNameSpecified)
        return "Failure [PACKAGE_INVALID_NAME]";
      AndroidPackageUninstallService uninstallService = new AndroidPackageUninstallService(this.factory);
      AndroidPackageUninstallResult uninstallResult = await uninstallService.UninstallAndroidPackageAsync(this.uninstallParameters.PackageName);
      return AdbMessageStrings.FromAndroidUninstallResult(uninstallResult);
    }
  }
}
