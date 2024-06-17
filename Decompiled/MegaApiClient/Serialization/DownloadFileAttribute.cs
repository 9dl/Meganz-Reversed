// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.DownloadFileAttributeRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class DownloadFileAttributeRequest : RequestBase
  {
    public DownloadFileAttributeRequest(string fileAttributeHandle)
      : base("ufa")
    {
      this.Id = fileAttributeHandle;
    }

    [JsonProperty("ssl")]
    public int Ssl => 2;

    [JsonProperty("r")]
    public int R => 1;

    [JsonProperty("fah")]
    public string Id { get; private set; }
  }
}
