// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder.ResourcesDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder
{
  public sealed class ResourcesDecoder : StreamDecoder
  {
    private TableChunk tableChunk;
    private IDictionary<uint, ApkResource> apkResources;

    public ResourcesDecoder(string apkResourcesFilePath)
      : base(apkResourcesFilePath)
    {
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("\nResource Groups Count: ");
      if (this.apkResources != null)
      {
        stringBuilder.Append(this.apkResources.Count);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\n Entries: \n");
        foreach (KeyValuePair<uint, ApkResource> apkResource in (IEnumerable<KeyValuePair<uint, ApkResource>>) this.apkResources)
          stringBuilder.Append((object) apkResource);
      }
      else
      {
        LoggerCore.Log("Resources have not been decoded yet.");
        stringBuilder.Append("0");
      }
      return stringBuilder.ToString();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Internal log message", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Acceptable for tasks")]
    public async Task<IDictionary<uint, ApkResource>> RetrieveApkResourcesAsync() => await Task.Run<IDictionary<uint, ApkResource>>((Func<IDictionary<uint, ApkResource>>) (() =>
    {
      if (this.apkResources == null)
      {
        LoggerCore.Log("Retrieving apk Resource groups");
        this.apkResources = ResourcesHelper.GetResourceGroups(this.RetrieveTableChunk());
      }
      return this.apkResources;
    })).ConfigureAwait(false);

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Internal log message", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)")]
    internal TableChunk RetrieveTableChunk()
    {
      if (this.tableChunk == null)
      {
        LoggerCore.Log("Decoding resources file as table chunk");
        this.tableChunk = ChunkDecoder.Decode((StreamDecoder) this) as TableChunk;
      }
      return this.tableChunk;
    }
  }
}
