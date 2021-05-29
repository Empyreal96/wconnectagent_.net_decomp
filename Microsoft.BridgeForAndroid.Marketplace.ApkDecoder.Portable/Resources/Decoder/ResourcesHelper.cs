// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder.ResourcesHelper
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder
{
  internal sealed class ResourcesHelper
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "temporary")]
    public static IDictionary<uint, ApkResource> GetResourceGroups(
      TableChunk tableChunk)
    {
      ConcurrentDictionary<uint, ApkResource> resources = new ConcurrentDictionary<uint, ApkResource>();
      IReadOnlyList<string> stringPool = tableChunk.StringPoolChunk.Strings;
      Parallel.ForEach<PackageChunk>((IEnumerable<PackageChunk>) tableChunk.PackageChunkList, (Action<PackageChunk>) (packageChunk =>
      {
        LoggerCore.Log("Package Name: " + packageChunk.PackageName);
        uint packageId = packageChunk.PackageId;
        IReadOnlyList<string> typeNameStrings = packageChunk.TypeNameStringsChunk.Strings;
        Parallel.ForEach<TypeRecord>((IEnumerable<TypeRecord>) packageChunk.TypeRecords, (Action<TypeRecord, ParallelLoopState>) ((typeRecord, loopState) =>
        {
          if (typeRecord.TypeChunks.Count <= 0)
            return;
          uint id = typeRecord.TypeSpecChunk.Id;
          if (id <= 0U)
            return;
          ApkResourceType resourceType = ResourcesHelper.GetResourceType(typeNameStrings[(int) id - 1]);
          if ((long) typeRecord.TypeSpecChunk.EntryFlags.Count != (long) typeRecord.TypeSpecChunk.EntryCount)
            throw new ApkDecoderResourcesException("Invalid flag count. Expected: " + (object) typeRecord.TypeSpecChunk.EntryCount);
          for (uint resourceItemId = 0; resourceItemId < typeRecord.TypeSpecChunk.EntryCount; ++resourceItemId)
          {
            ConcurrentBag<ApkResourceValue> apkResourceValues = new ConcurrentBag<ApkResourceValue>();
            Parallel.ForEach<TypeChunk>((IEnumerable<TypeChunk>) typeRecord.TypeChunks, (Action<TypeChunk>) (typeChunk =>
            {
              ResourceItem resourceItem;
              if (!typeChunk.ResourceItems.TryGetValue(resourceItemId, out resourceItem))
                return;
              if (resourceItem == null)
                throw new ApkDecoderResourcesException("Resource Item can't be null");
              ApkResourceConfig apkResourceConfig = ResourcesHelper.CreateApkResourceConfig(typeRecord, resourceItemId, typeChunk);
              apkResourceValues.Add(ResourcesHelper.CreateApkResourceValue(resourceItem, resourceType, apkResourceConfig, stringPool) ?? throw new ApkDecoderResourcesException("Resource value cannot be null"));
            }));
            ApkResource apkResource = new ApkResource((IEnumerable<ApkResourceValue>) apkResourceValues, resourceType);
            resources.TryAdd(ResourcesHelper.GenerateResourceId(packageId, id, resourceItemId), apkResource);
          }
        }));
      }));
      return (IDictionary<uint, ApkResource>) resources;
    }

    public static string GetResourceData(
      ResourceValue resourceValue,
      IReadOnlyList<string> stringPool)
    {
      switch (resourceValue.Type)
      {
        case ResourceValueTypes.Reference:
          return "@res:" + resourceValue.Data.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture);
        case ResourceValueTypes.String:
          return stringPool[(int) resourceValue.Data];
        case ResourceValueTypes.FirstInt:
          return ((int) resourceValue.Data).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case ResourceValueTypes.IntHex:
          return resourceValue.Data.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case ResourceValueTypes.IntBoolean:
          return (resourceValue.Data != 0U).ToString();
        default:
          return "{ Type=" + (object) resourceValue.Type + ", Data=" + resourceValue.Data.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture) + "}";
      }
    }

    private static ApkResourceValue CreateApkResourceValue(
      ResourceItem resourceItem,
      ApkResourceType resourceType,
      ApkResourceConfig apkResourceConfig,
      IReadOnlyList<string> stringPool)
    {
      ApkResourceValue apkResourceValue;
      if (resourceItem.ResourceKey.IsComplexValue())
      {
        apkResourceValue = new ApkResourceValue(resourceType, apkResourceConfig, "{Complex Resources}");
      }
      else
      {
        string resourceData = ResourcesHelper.GetResourceData(resourceItem.SimpleValue, stringPool);
        apkResourceValue = new ApkResourceValue(resourceType, apkResourceConfig, resourceData);
      }
      return apkResourceValue;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We don't intend to localize log messages", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)")]
    private static bool CheckIfUnSupportedResourcesPresent(
      uint typeSpecEntryFlag,
      ResourceConfig config)
    {
      if ((((int) typeSpecEntryFlag & 128) != 0 || ((int) typeSpecEntryFlag & 8) != 0) && ((int) config.ScreenType & (int) ushort.MaxValue) != 0)
      {
        LoggerCore.Log("Resource contains qualifiers related to Orientation or touch-screen.");
        return true;
      }
      if ((((int) typeSpecEntryFlag & 1) != 0 || ((int) typeSpecEntryFlag & 2) != 0) && config.Imsi != 0U)
      {
        LoggerCore.Log("Resource contains qualifiers related to MNC or MCC");
        return true;
      }
      if ((((int) typeSpecEntryFlag & 16) != 0 || ((int) typeSpecEntryFlag & 32) != 0 || ((int) typeSpecEntryFlag & 64) != 0) && config.Input != 0U)
      {
        LoggerCore.Log("Resource contains qualifiers related to Keyboard or navigation.");
        return true;
      }
      if (((int) typeSpecEntryFlag & 4096) == 0 && ((int) typeSpecEntryFlag & 2048) == 0 && (((int) typeSpecEntryFlag & 16384) == 0 && ((int) typeSpecEntryFlag & 8192) == 0) || config.ScreenConfig == 0U)
        return false;
      LoggerCore.Log("Resource contains qualifiers related to Screen configuration.");
      return true;
    }

    private static ApkResourceConfig CreateApkResourceConfig(
      TypeRecord typeRecord,
      uint resourceItemId,
      TypeChunk typeChunk)
    {
      ApkResourceConfig apkResourceConfig = new ApkResourceConfig();
      apkResourceConfig.TypeSpecEntry = typeRecord.TypeSpecChunk.EntryFlags[(int) resourceItemId];
      if (((int) apkResourceConfig.TypeSpecEntry & 4) != 0)
        apkResourceConfig.Locale = typeChunk.Config.Locale;
      apkResourceConfig.Unsupported = ResourcesHelper.CheckIfUnSupportedResourcesPresent(apkResourceConfig.TypeSpecEntry, typeChunk.Config);
      return apkResourceConfig;
    }

    private static ApkResourceType GetResourceType(string resourceTypeString)
    {
      LoggerCore.Log("Trying to find resource type: {0} in list", (object) resourceTypeString);
      ApkResourceType apkResourceType;
      switch (resourceTypeString.ToUpperInvariant())
      {
        case "ANIM":
        case "ANIMATION":
        case "ANIMATOR":
          apkResourceType = ApkResourceType.Anim;
          break;
        case "COLOR":
          apkResourceType = ApkResourceType.Color;
          break;
        case "DRAWABLE":
        case "MIPMAP":
          apkResourceType = ApkResourceType.Drawable;
          break;
        case "STRING":
          apkResourceType = ApkResourceType.String;
          break;
        case "LAYOUT":
          apkResourceType = ApkResourceType.Layout;
          break;
        case "MENU":
          apkResourceType = ApkResourceType.Menu;
          break;
        case "STYLE":
          apkResourceType = ApkResourceType.Style;
          break;
        case "XML":
          apkResourceType = ApkResourceType.Xml;
          break;
        case "ATTR":
          apkResourceType = ApkResourceType.Attr;
          break;
        case "RAW":
          apkResourceType = ApkResourceType.Raw;
          break;
        case "ID":
          apkResourceType = ApkResourceType.Id;
          break;
        case "INTEGER":
          apkResourceType = ApkResourceType.Integer;
          break;
        case "DIMEN":
          apkResourceType = ApkResourceType.Dimen;
          break;
        case "BOOL":
          apkResourceType = ApkResourceType.Bool;
          break;
        case "ARRAY":
          apkResourceType = ApkResourceType.Array;
          break;
        case "APKEXPANSIONID":
          apkResourceType = ApkResourceType.ApkExpansionId;
          break;
        case "PLURALS":
          apkResourceType = ApkResourceType.Plurals;
          break;
        default:
          apkResourceType = ApkResourceType.Unknown;
          break;
      }
      return apkResourceType;
    }

    private static uint GenerateResourceId(uint packageId, uint typeId, uint itemId) => (uint) (((int) packageId & (int) byte.MaxValue) << 24 | ((int) typeId & (int) byte.MaxValue) << 16 | (int) itemId & (int) ushort.MaxValue);
  }
}
