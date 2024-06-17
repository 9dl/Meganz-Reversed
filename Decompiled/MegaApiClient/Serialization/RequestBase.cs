﻿// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.RequestBase
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using System.Collections.Generic;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal abstract class RequestBase
  {
    protected RequestBase(string action)
    {
      this.Action = action;
      this.QueryArguments = new Dictionary<string, string>();
    }

    [JsonProperty("a")]
    public string Action { get; private set; }

    [JsonIgnore]
    public Dictionary<string, string> QueryArguments { get; }
  }
}
