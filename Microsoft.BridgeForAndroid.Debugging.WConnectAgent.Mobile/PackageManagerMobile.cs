// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.PackageManagerMobile
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A3725DA-8D01-4D08-90D4-BC0331796EE5
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile.dll

using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  public class PackageManagerMobile : IPackageManager
  {
    private const int RetryAttempts = 4;
    private const int InstallRetryAttemptSleep = 1500;
    private const uint ErrorInstallFailed = 2147958009;
    private readonly string currentUserSid = PackageManagerMobile.GetCurrentUserSidAsString();
    private PackageManager pacman = new PackageManager();

    public IList<AppxPackage> FindPackages()
    {
      IList<AppxPackage> appxPackageList = (IList<AppxPackage>) new List<AppxPackage>();
      foreach (Package package in this.pacman.FindPackagesForUser(this.currentUserSid))
        appxPackageList.Add(new AppxPackage(package.Id.Name, package.Id.FullName, package.Id.Publisher, PackageManagerMobile.ReadPackageLocation(package)));
      return appxPackageList;
    }

    public async Task<PackageDeploymentResult> InstallAppFromFolderLayoutAsync(
      Uri manifestUri,
      IEnumerable<Uri> dependencyPackageUris)
    {
      if (manifestUri == (Uri) null)
        throw new ArgumentNullException(nameof (manifestUri));
      int retryAttempt = 0;
      PackageDeploymentResult lastPackageDeploymentResult = (PackageDeploymentResult) null;
      for (; retryAttempt <= 4; ++retryAttempt)
      {
        if (retryAttempt > 0)
        {
          LoggerCore.Log("Sleeping {0} millisecond(s)...", (object) 1500);
          await Task.Delay(1500);
          LoggerCore.Log("Woke up!");
          EtwLogger.Instance.AppxInstallReattempt();
        }
        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = this.pacman.RegisterPackageAsync(manifestUri, (IEnumerable<Uri>) null, (DeploymentOptions) 3);
        AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completedHandler1 = (AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>) null;
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        PackageManagerMobile.\u003C\u003Ec__DisplayClass2 cDisplayClass2 = new PackageManagerMobile.\u003C\u003Ec__DisplayClass2();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass2.deploymentCompletedEvent = new ManualResetEvent(false);
        try
        {
          IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operationWithProgress = deploymentOperation;
          AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completed = operationWithProgress.Completed;
          if (completedHandler1 == null)
          {
            // ISSUE: method pointer
            completedHandler1 = new AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>((object) cDisplayClass2, __methodptr(\u003CInstallAppFromFolderLayoutAsync\u003Eb__0));
          }
          AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completedHandler2 = completedHandler1;
          operationWithProgress.put_Completed((AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>) Delegate.Combine((Delegate) completed, (Delegate) completedHandler2));
          // ISSUE: reference to a compiler-generated field
          cDisplayClass2.deploymentCompletedEvent.WaitOne();
        }
        finally
        {
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass2.deploymentCompletedEvent != null)
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass2.deploymentCompletedEvent.Dispose();
          }
        }
        DeploymentResult deploymentResult = deploymentOperation.GetResults();
        lastPackageDeploymentResult = new PackageDeploymentResult(((IAsyncInfo) deploymentOperation).ErrorCode, deploymentResult.ExtendedErrorCode);
        if (lastPackageDeploymentResult.Error != null && lastPackageDeploymentResult.Error.HResult == -2147009287)
          LoggerCore.Log(LoggerCore.LogLevels.Info, "Pacman returned ERROR_INSTALL_FAILED");
        else
          break;
      }
      if (retryAttempt >= 5)
        LoggerCore.Log(LoggerCore.LogLevels.Info, "Installation retry attempts exhausted.");
      return lastPackageDeploymentResult;
    }

    public async Task<AppUninstallResult> UninstallAppsAsync(
      AppxPackage package)
    {
      if (package == null)
        throw new ArgumentNullException(nameof (package));
      return await this.UninstallAppsAsync(package.PackageName, package.PackagePublisher);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design.")]
    private static string ReadPackageLocation(Package package)
    {
      string str = (string) null;
      try
      {
        str = package.InstalledLocation.Path;
      }
      catch (Exception ex)
      {
        LoggerCore.Log(ex);
      }
      return str;
    }

    private static string GetCurrentUserSidAsString()
    {
      IntPtr num1 = IntPtr.Zero;
      IntPtr TokenHandle = IntPtr.Zero;
      IntPtr num2 = IntPtr.Zero;
      try
      {
        num1 = NativeMethods.GetCurrentProcess();
        if (!NativeMethods.OpenProcessToken(num1, 8, out TokenHandle))
        {
          LoggerCore.Log(LoggerCore.LogLevels.Error, "OpenProcessToken Failed");
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        int reqLength = -1;
        if (!NativeMethods.GetTokenInformation(TokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, 0, out reqLength))
        {
          if (reqLength > 0)
          {
            LoggerCore.Log("GetTokenInformation first call got the token length: {0}.", (object) reqLength);
          }
          else
          {
            LoggerCore.Log(LoggerCore.LogLevels.Error, "GetTokenInformation first call failed, error code = {0}.", (object) Marshal.GetLastWin32Error());
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
          }
        }
        num2 = Marshal.AllocHGlobal(reqLength);
        if (!NativeMethods.GetTokenInformation(TokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS.TokenUser, num2, reqLength, out reqLength))
        {
          LoggerCore.Log(LoggerCore.LogLevels.Error, "GetTokenInformation second call Failed");
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        string pStringSid;
        if (!NativeMethods.ConvertSidToStringSid(Marshal.PtrToStructure<NativeMethods.TOKEN_USER>(num2).User.Sid, out pStringSid))
        {
          LoggerCore.Log(LoggerCore.LogLevels.Error, "ConvertSidToStringSid Failed");
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        return pStringSid;
      }
      finally
      {
        if (num2 != IntPtr.Zero)
          Marshal.FreeHGlobal(num2);
        if (TokenHandle != IntPtr.Zero)
          NativeMethods.CloseHandle(TokenHandle);
        if (num1 != IntPtr.Zero)
          NativeMethods.CloseHandle(num1);
      }
    }

    private async Task<AppUninstallResult> UninstallAppsAsync(
      string packageName,
      string packagePublisher)
    {
      if (string.IsNullOrWhiteSpace(packageName))
        throw new ArgumentException("Package name cannot be null or whitespace.", nameof (packageName));
      if (string.IsNullOrWhiteSpace(packagePublisher))
        throw new ArgumentException("Package publisher cannot be null or whitespace.", nameof (packagePublisher));
      IEnumerable<Package> packages = this.pacman.FindPackagesForUser(this.currentUserSid);
      int foundCount = 0;
      foreach (Package package in packages)
      {
        if (string.Compare(package.Id.Name, packageName, StringComparison.Ordinal) == 0 && string.Compare(package.Id.Publisher, packagePublisher, StringComparison.Ordinal) == 0)
        {
          ++foundCount;
          PackageDeploymentResult result = await this.UninstallAppsAsync(package.Id.FullName);
          if (result.Error != null)
            return AppUninstallResult.Error;
        }
      }
      return foundCount > 0 ? AppUninstallResult.Success : AppUninstallResult.NotFound;
    }

    private async Task<PackageDeploymentResult> UninstallAppsAsync(
      string fullName)
    {
      LoggerCore.Log("Uninstalling " + fullName);
      IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operationWithProgress1 = this.pacman.RemovePackageAsync(fullName);
      AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completedHandler1 = (AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>) null;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      PackageManagerMobile.\u003C\u003Ec__DisplayClass18 cDisplayClass18 = new PackageManagerMobile.\u003C\u003Ec__DisplayClass18();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass18.deploymentCompletedEvent = new ManualResetEvent(false);
      try
      {
        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operationWithProgress2 = operationWithProgress1;
        AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completed = operationWithProgress2.Completed;
        if (completedHandler1 == null)
        {
          // ISSUE: method pointer
          completedHandler1 = new AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>((object) cDisplayClass18, __methodptr(\u003CUninstallAppsAsync\u003Eb__16));
        }
        AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> completedHandler2 = completedHandler1;
        operationWithProgress2.put_Completed((AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>) Delegate.Combine((Delegate) completed, (Delegate) completedHandler2));
        // ISSUE: reference to a compiler-generated field
        cDisplayClass18.deploymentCompletedEvent.WaitOne();
      }
      finally
      {
        // ISSUE: reference to a compiler-generated field
        if (cDisplayClass18.deploymentCompletedEvent != null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass18.deploymentCompletedEvent.Dispose();
        }
      }
      DeploymentResult results = operationWithProgress1.GetResults();
      if (((IAsyncInfo) operationWithProgress1).ErrorCode == null)
      {
        LoggerCore.Log("Uninstall succeeded");
      }
      else
      {
        LoggerCore.Log("Uninstall failed");
        LoggerCore.Log(LoggerCore.LogLevels.Error, ((IAsyncInfo) operationWithProgress1).ErrorCode);
        if (!string.IsNullOrWhiteSpace(results.ErrorText))
          LoggerCore.Log(LoggerCore.LogLevels.Error, results.ErrorText);
        if (results.ExtendedErrorCode != null)
          LoggerCore.Log(LoggerCore.LogLevels.Error, results.ExtendedErrorCode);
      }
      return new PackageDeploymentResult(((IAsyncInfo) operationWithProgress1).ErrorCode, results.ExtendedErrorCode);
    }
  }
}
