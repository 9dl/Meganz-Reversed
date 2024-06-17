// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.AccountInformationResponse
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class AccountInformationResponse : IAccountInformation
  {
    [JsonProperty("mstrg")]
    public long TotalQuota { get; private set; }

    [JsonProperty("cstrg")]
    public long UsedQuota { get; private set; }

    [JsonProperty("cstrgn")]
    private Dictionary<string, long[]> MetricsSerialized { get; set; }

    public IEnumerable<IStorageMetrics> Metrics { get; private set; }

    [System.Runtime.Serialization.OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
      this.Metrics = this.MetricsSerialized.Select<KeyValuePair<string, long[]>, IStorageMetrics>((Func<KeyValuePair<string, long[]>, IStorageMetrics>) (x => (IStorageMetrics) new AccountInformationResponse.StorageMetrics(x.Key, x.Value)));
    }

    private class StorageMetrics : IStorageMetrics
    {
      public StorageMetrics(string nodeId, long[] metrics)
      {
        this.NodeId = nodeId;
        this.BytesUsed = metrics[0];
        this.FilesCount = metrics[1];
        this.FoldersCount = metrics[2];
      }

      public string NodeId { get; }

      public long BytesUsed { get; }

      public long FilesCount { get; }

      public long FoldersCount { get; }
    }
  }
}
