// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.DownloadUrlRequestFromId
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class DownloadUrlRequestFromId : RequestBase
  {
    public DownloadUrlRequestFromId(string id)
      : base("g")
    {
      this.Id = id;
    }

    [JsonProperty("g")]
    public int G => 1;

    [JsonProperty("p")]
    public string Id { get; private set; }
  }
}
