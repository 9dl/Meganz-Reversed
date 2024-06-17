// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Node
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using CG.Web.MegaApiClient.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

#nullable disable
namespace CG.Web.MegaApiClient
{
  [DebuggerDisplay("Node - Type: {Type} - Name: {Name} - Id: {Id}")]
  internal class Node : INode, IEquatable<INode>, INodeCrypto
  {
    private static readonly Regex s_fileAttributeRegex = new Regex("(?<id>\\d+):(?<type>\\d+)\\*(?<handle>[a-zA-Z0-9-_]+)");
    private byte[] _masterKey;
    private readonly List<CG.Web.MegaApiClient.Serialization.SharedKey> _sharedKeys;

    public Node(byte[] masterKey, ref List<CG.Web.MegaApiClient.Serialization.SharedKey> sharedKeys)
    {
      this._masterKey = masterKey;
      this._sharedKeys = sharedKeys;
    }

    internal Node(
      string id,
      DownloadUrlResponse downloadResponse,
      byte[] key,
      byte[] iv,
      byte[] metaMac)
    {
      this.Id = id;
      this.Attributes = Crypto.DecryptAttributes(downloadResponse.SerializedAttributes.FromBase64(), key);
      this.Size = downloadResponse.Size;
      this.Type = NodeType.File;
      this.FileAttributes = Node.DeserializeFileAttributes(downloadResponse.SerializedFileAttributes);
      this.Key = key;
      this.Iv = iv;
      this.MetaMac = metaMac;
    }

    [JsonIgnore]
    public string Name => this.Attributes?.Name;

    [JsonProperty("s")]
    public long Size { get; private set; }

    [JsonProperty("t")]
    public NodeType Type { get; private set; }

    [JsonProperty("h")]
    public string Id { get; private set; }

    [JsonIgnore]
    public DateTime? ModificationDate => this.Attributes?.ModificationDate;

    [JsonIgnore]
    public string Fingerprint => this.Attributes?.SerializedFingerprint;

    [JsonIgnore]
    public Attributes Attributes { get; private set; }

    [JsonProperty("p")]
    public string ParentId { get; private set; }

    [JsonIgnore]
    public DateTime? CreationDate { get; private set; }

    [JsonProperty("u")]
    public string Owner { get; private set; }

    [JsonIgnore]
    public IFileAttribute[] FileAttributes { get; private set; }

    [JsonProperty("su")]
    internal string SharingId { get; private set; }

    [JsonProperty("sk")]
    internal string SharingKey { get; private set; }

    [JsonIgnore]
    internal bool EmptyKey { get; private set; }

    [JsonIgnore]
    public byte[] Key { get; private set; }

    [JsonIgnore]
    public byte[] SharedKey { get; private set; }

    [JsonIgnore]
    public byte[] Iv { get; private set; }

    [JsonIgnore]
    public byte[] MetaMac { get; private set; }

    [JsonIgnore]
    public byte[] FullKey { get; private set; }

    [JsonProperty("ts")]
    private long SerializedCreationDate { get; set; }

    [JsonProperty("a")]
    private string SerializedAttributes { get; set; }

    [JsonProperty("k")]
    internal string SerializedKey { get; private set; }

    [JsonProperty("fa")]
    private string SerializedFileAttributes { get; set; }

    [System.Runtime.Serialization.OnDeserialized]
    public void OnDeserialized(StreamingContext ctx)
    {
      if (this.SharingKey != null && !this._sharedKeys.Any<CG.Web.MegaApiClient.Serialization.SharedKey>((Func<CG.Web.MegaApiClient.Serialization.SharedKey, bool>) (x => x.Id == this.Id)))
        this._sharedKeys.Add(new CG.Web.MegaApiClient.Serialization.SharedKey(this.Id, this.SharingKey));
      this.CreationDate = new DateTime?(this.SerializedCreationDate.ToDateTime());
      if (this.Type != NodeType.File && this.Type != NodeType.Directory)
        return;
      if (string.IsNullOrEmpty(this.SerializedKey))
      {
        this.EmptyKey = true;
      }
      else
      {
        string str = this.SerializedKey.Split('/')[0];
        int length = str.IndexOf(":", StringComparison.Ordinal);
        byte[] data = str.Substring(length + 1).FromBase64();
        if (this._sharedKeys != null)
        {
          string handle = str.Substring(0, length);
          CG.Web.MegaApiClient.Serialization.SharedKey sharedKey = this._sharedKeys.FirstOrDefault<CG.Web.MegaApiClient.Serialization.SharedKey>((Func<CG.Web.MegaApiClient.Serialization.SharedKey, bool>) (x => x.Id == handle));
          if (sharedKey != null)
          {
            this._masterKey = Crypto.DecryptKey(sharedKey.Key.FromBase64(), this._masterKey);
            this.SharedKey = this.Type != NodeType.Directory ? Crypto.DecryptKey(data, this._masterKey) : this._masterKey;
          }
        }
        if (data.Length != 16 && data.Length != 32)
          return;
        this.FullKey = Crypto.DecryptKey(data, this._masterKey);
        if (this.Type == NodeType.File)
        {
          byte[] iv;
          byte[] metaMac;
          byte[] fileKey;
          Crypto.GetPartsFromDecryptedKey(this.FullKey, out iv, out metaMac, out fileKey);
          this.Iv = iv;
          this.MetaMac = metaMac;
          this.Key = fileKey;
        }
        else
          this.Key = this.FullKey;
        this.Attributes = Crypto.DecryptAttributes(this.SerializedAttributes.FromBase64(), this.Key);
        this.FileAttributes = Node.DeserializeFileAttributes(this.SerializedFileAttributes);
      }
    }

    public bool Equals(INode other) => other != null && this.Id == other.Id;

    public override int GetHashCode() => this.Id.GetHashCode();

    public override bool Equals(object obj) => this.Equals(obj as INode);

    private static IFileAttribute[] DeserializeFileAttributes(string serializedFileAttributes)
    {
      if (serializedFileAttributes == null)
        return new IFileAttribute[0];
      return ((IEnumerable<string>) serializedFileAttributes.Split('/')).Select<string, Match>((Func<string, Match>) (_ => Node.s_fileAttributeRegex.Match(_))).Where<Match>((Func<Match, bool>) (_ => _.Success)).Select<Match, FileAttribute>((Func<Match, FileAttribute>) (_ => new FileAttribute(int.Parse(_.Groups["id"].Value), (FileAttributeType) Enum.Parse(typeof (FileAttributeType), _.Groups["type"].Value), _.Groups["handle"].Value))).Cast<IFileAttribute>().ToArray<IFileAttribute>();
    }
  }
}
