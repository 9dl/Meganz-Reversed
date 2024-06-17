// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.PublicNode
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Diagnostics;

#nullable disable
namespace CG.Web.MegaApiClient
{
  [DebuggerDisplay("PublicNode - Type: {Type} - Name: {Name} - Id: {Id}")]
  internal class PublicNode : INode, IEquatable<INode>, INodeCrypto
  {
    private readonly Node _node;

    internal PublicNode(Node node, string shareId)
    {
      this._node = node;
      this.ShareId = shareId;
    }

    public string ShareId { get; }

    public bool Equals(INode other)
    {
      return this._node.Equals(other) && this.ShareId == (other is PublicNode publicNode ? publicNode.ShareId : (string) null);
    }

    public long Size => this._node.Size;

    public string Name => this._node.Name;

    public DateTime? ModificationDate => this._node.ModificationDate;

    public string Fingerprint => this._node.Fingerprint;

    public string Id => this._node.Id;

    public string ParentId => !this.IsShareRoot ? this._node.ParentId : (string) null;

    public string Owner => this._node.Owner;

    public NodeType Type
    {
      get
      {
        return !this.IsShareRoot || this._node.Type != NodeType.Directory ? this._node.Type : NodeType.Root;
      }
    }

    public DateTime? CreationDate => this._node.CreationDate;

    public byte[] Key => this._node.Key;

    public byte[] SharedKey => this._node.SharedKey;

    public byte[] Iv => this._node.Iv;

    public byte[] MetaMac => this._node.MetaMac;

    public byte[] FullKey => this._node.FullKey;

    public IFileAttribute[] FileAttributes => this._node.FileAttributes;

    private bool IsShareRoot
    {
      get
      {
        if (this._node.SerializedKey == null)
          return true;
        string str = this._node.SerializedKey.Split('/')[0];
        return str.Substring(0, str.IndexOf(":", StringComparison.Ordinal)) == this.Id;
      }
    }
  }
}
