// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.AccountInformationRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class AccountInformationRequest : RequestBase
  {
    public AccountInformationRequest()
      : base("uq")
    {
    }

    [JsonProperty("strg")]
    public int Storage => 1;

    [JsonProperty("xfer")]
    public int Transfer => 0;

    [JsonProperty("pro")]
    public int AccountType => 0;
  }
}
