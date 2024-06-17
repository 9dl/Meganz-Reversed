// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.ShareData
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using System.Collections.Generic;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  [JsonConverter(typeof (ShareDataConverter))]
  internal class ShareData
  {
    private readonly IList<ShareData.ShareDataItem> _items;

    public ShareData(string nodeId)
    {
      this.NodeId = nodeId;
      this._items = (IList<ShareData.ShareDataItem>) new List<ShareData.ShareDataItem>();
    }

    public string NodeId { get; private set; }

    public IEnumerable<ShareData.ShareDataItem> Items
    {
      get => (IEnumerable<ShareData.ShareDataItem>) this._items;
    }

    public void AddItem(string nodeId, byte[] data, byte[] key)
    {
      this._items.Add(new ShareData.ShareDataItem()
      {
        NodeId = nodeId,
        Data = data,
        Key = key
      });
    }

    public class ShareDataItem
    {
      public string NodeId { get; set; }

      public byte[] Data { get; set; }

      public byte[] Key { get; set; }
    }
  }
}
