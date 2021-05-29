// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.DeveloperReportBase
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [DataContract]
  public abstract class DeveloperReportBase : IDeveloperReport
  {
    private const string ErrorMessagePrefix = "ER";
    private const string InfoMessagePrefix = "IN";
    private const string WarningMessagePrefix = "WA";
    private HashSet<Enum> alreadyReportedMessages = new HashSet<Enum>();

    public static string GetMessagePrefix(
      string builderPrefix,
      WorkerLogLevel logLevel,
      Enum messageId)
    {
      if (string.IsNullOrEmpty(nameof (builderPrefix)))
        throw new ArgumentException("buildPrefix cannot be null or empty.", nameof (builderPrefix));
      if (messageId == null)
        throw new ArgumentNullException(nameof (messageId));
      string str = "IN";
      switch (logLevel)
      {
        case WorkerLogLevel.Warning:
          str = "WA";
          break;
        case WorkerLogLevel.Error:
          str = "ER";
          break;
      }
      return str + builderPrefix + messageId.ToString("X").Substring(4);
    }

    public void AddReportMessage(IFeatureDetails featureDetails)
    {
      if (featureDetails == null)
        throw new ArgumentNullException(nameof (featureDetails));
      this.OnAddReportMessage(featureDetails.CreateFeatureLog());
      AggregateFeature aggregateFeature = featureDetails.CreateAggregateFeature();
      if (aggregateFeature != null)
        this.OnAddAggregateFeature(aggregateFeature.AggregateFeatureName, aggregateFeature.MessageVersion);
      this.alreadyReportedMessages.Add(featureDetails.Message);
    }

    public void AddNewReportMessage(IFeatureDetails featureDetails)
    {
      if (featureDetails == null)
        throw new ArgumentNullException(nameof (featureDetails));
      if (this.alreadyReportedMessages.Contains(featureDetails.Message))
        return;
      this.AddReportMessage(featureDetails);
    }

    protected abstract void OnAddReportMessage(IFeatureLog feature);

    protected abstract void OnAddAggregateFeature(string aggregateFeature, uint messageVersion);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => this.alreadyReportedMessages = new HashSet<Enum>();
  }
}
