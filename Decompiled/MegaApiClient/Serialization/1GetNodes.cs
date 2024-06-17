// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.GetNodesResponse
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class GetNodesResponse
  {
    private readonly byte[] _masterKey;
    private List<SharedKey> _sharedKeys;

    public GetNodesResponse(byte[] masterKey) => this._masterKey = masterKey;

    public CG.Web.MegaApiClient.Node[] Nodes { get; private set; }

    public CG.Web.MegaApiClient.Node[] UnsupportedNodes { get; private set; }

    [JsonProperty("f")]
    public JRaw NodesSerialized { get; private set; }

    [JsonProperty("ok")]
    public List<SharedKey> SharedKeys
    {
      get => this._sharedKeys;
      private set => this._sharedKeys = value;
    }

    [System.Runtime.Serialization.OnDeserialized]
    public void OnDeserialized(StreamingContext ctx)
    {
      CG.Web.MegaApiClient.Node[] source = JsonConvert.DeserializeObject<CG.Web.MegaApiClient.Node[]>(this.NodesSerialized.ToString(), (JsonConverter) new NodeConverter(this._masterKey, ref this._sharedKeys));
      this.UnsupportedNodes = ((IEnumerable<CG.Web.MegaApiClient.Node>) source).Where<CG.Web.MegaApiClient.Node>((Func<CG.Web.MegaApiClient.Node, bool>) (x => x.EmptyKey)).ToArray<CG.Web.MegaApiClient.Node>();
      this.Nodes = ((IEnumerable<CG.Web.MegaApiClient.Node>) source).Where<CG.Web.MegaApiClient.Node>((Func<CG.Web.MegaApiClient.Node, bool>) (x => !x.EmptyKey)).ToArray<CG.Web.MegaApiClient.Node>();
    }
  }
}
