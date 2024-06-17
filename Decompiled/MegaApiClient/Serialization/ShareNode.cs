// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.ShareNodeRequest
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class ShareNodeRequest : RequestBase
  {
    public ShareNodeRequest(INode node, byte[] masterKey, IEnumerable<INode> nodes)
      : base("s2")
    {
      this.Id = node.Id;
      this.Options = new object[1]
      {
        (object) new{ r = 0, u = "EXP" }
      };
      INodeCrypto nodeCrypto = (INodeCrypto) node;
      byte[] numArray = nodeCrypto.SharedKey ?? Crypto.CreateAesKey();
      this.SharedKey = Crypto.EncryptKey(numArray, masterKey).ToBase64();
      if (nodeCrypto.SharedKey == null)
      {
        this.Share = new ShareData(node.Id);
        this.Share.AddItem(node.Id, nodeCrypto.FullKey, numArray);
        foreach (INode recursiveChild in this.GetRecursiveChildren(nodes.ToArray<INode>(), node))
          this.Share.AddItem(recursiveChild.Id, ((INodeCrypto) recursiveChild).FullKey, numArray);
      }
      this.HandleAuth = Crypto.EncryptKey((node.Id + node.Id).ToBytes(), masterKey).ToBase64();
    }

    private IEnumerable<INode> GetRecursiveChildren(INode[] nodes, INode parent)
    {
      using (IEnumerator<INode> enumerator = ((IEnumerable<INode>) nodes).Where<INode>((Func<INode, bool>) (x => x.Type == NodeType.Directory || x.Type == NodeType.File)).GetEnumerator())
      {
label_8:
        if (enumerator.MoveNext())
        {
          INode current = enumerator.Current;
          string parentId = current.Id;
          do
          {
            parentId = ((IEnumerable<INode>) nodes).FirstOrDefault<INode>((Func<INode, bool>) (x => x.Id == parentId))?.ParentId;
            if (parentId == parent.Id)
            {
              yield return current;
              break;
            }
          }
          while (parentId != null);
          goto label_8;
        }
      }
    }

    [JsonProperty("n")]
    public string Id { get; private set; }

    [JsonProperty("ha")]
    public string HandleAuth { get; private set; }

    [JsonProperty("s")]
    public object[] Options { get; private set; }

    [JsonProperty("cr")]
    public ShareData Share { get; private set; }

    [JsonProperty("ok")]
    public string SharedKey { get; private set; }
  }
}
