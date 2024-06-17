// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.DownloadUrlRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class DownloadUrlRequest : RequestBase
  {
    public DownloadUrlRequest(INode node)
      : base("g")
    {
      this.Id = node.Id;
      if (!(node is PublicNode publicNode))
        return;
      this.QueryArguments["n"] = publicNode.ShareId;
    }

    [JsonProperty("g")]
    public int G => 1;

    [JsonProperty("n")]
    public string Id { get; private set; }
  }
}
