// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.SharedKey
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using System.Diagnostics;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  [DebuggerDisplay("Id: {Id} / Key: {Key}")]
  internal class SharedKey
  {
    public SharedKey(string id, string key)
    {
      this.Id = id;
      this.Key = key;
    }

    [JsonProperty("h")]
    public string Id { get; private set; }

    [JsonProperty("k")]
    public string Key { get; private set; }
  }
}
