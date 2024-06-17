// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.INode
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public interface INode : IEquatable<INode>
  {
    string Id { get; }

    NodeType Type { get; }

    string Name { get; }

    long Size { get; }

    DateTime? ModificationDate { get; }

    string Fingerprint { get; }

    string ParentId { get; }

    DateTime? CreationDate { get; }

    string Owner { get; }

    IFileAttribute[] FileAttributes { get; }
  }
}
