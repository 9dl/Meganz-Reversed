// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.CreateNodeRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using System;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class CreateNodeRequest : RequestBase
  {
    private CreateNodeRequest(
      INode parentNode,
      NodeType type,
      string attributes,
      string encryptedKey,
      byte[] key,
      string completionHandle)
      : base("p")
    {
      this.ParentId = parentNode.Id;
      this.Nodes = new CreateNodeRequest.CreateNodeRequestData[1]
      {
        new CreateNodeRequest.CreateNodeRequestData()
        {
          Attributes = attributes,
          Key = encryptedKey,
          Type = type,
          CompletionHandle = completionHandle
        }
      };
      if (!(parentNode is INodeCrypto nodeCrypto))
        throw new ArgumentException("parentNode node must implement INodeCrypto");
      if (nodeCrypto.SharedKey == null)
        return;
      this.Share = new ShareData(parentNode.Id);
      this.Share.AddItem(completionHandle, key, nodeCrypto.SharedKey);
    }

    [JsonProperty("t")]
    public string ParentId { get; private set; }

    [JsonProperty("cr")]
    public ShareData Share { get; private set; }

    [JsonProperty("n")]
    public CreateNodeRequest.CreateNodeRequestData[] Nodes { get; private set; }

    public static CreateNodeRequest CreateFileNodeRequest(
      INode parentNode,
      string attributes,
      string encryptedkey,
      byte[] fileKey,
      string completionHandle)
    {
      return new CreateNodeRequest(parentNode, NodeType.File, attributes, encryptedkey, fileKey, completionHandle);
    }

    public static CreateNodeRequest CreateFolderNodeRequest(
      INode parentNode,
      string attributes,
      string encryptedkey,
      byte[] key)
    {
      return new CreateNodeRequest(parentNode, NodeType.Directory, attributes, encryptedkey, key, "xxxxxxxx");
    }

    internal class CreateNodeRequestData
    {
      [JsonProperty("h")]
      public string CompletionHandle { get; set; }

      [JsonProperty("t")]
      public NodeType Type { get; set; }

      [JsonProperty("a")]
      public string Attributes { get; set; }

      [JsonProperty("k")]
      public string Key { get; set; }
    }
  }
}
