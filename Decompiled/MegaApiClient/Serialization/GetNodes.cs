﻿// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.GetNodesRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class GetNodesRequest : RequestBase
  {
    public GetNodesRequest(string shareId = null)
      : base("f")
    {
      this.C = 1;
      if (shareId == null)
        return;
      this.QueryArguments["n"] = shareId;
      this.R = 1;
    }

    [JsonProperty("c")]
    public int C { get; private set; }

    [JsonProperty("r")]
    public int R { get; private set; }
  }
}
