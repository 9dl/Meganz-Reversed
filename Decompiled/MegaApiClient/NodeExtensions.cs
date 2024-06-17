// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.NodeExtensions
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public static class NodeExtensions
  {
    public static long GetFolderSize(this INode node, IMegaApiClient client)
    {
      IEnumerable<INode> nodes = client.GetNodes();
      return node.GetFolderSize(nodes);
    }

    public static long GetFolderSize(this INode node, IEnumerable<INode> allNodes)
    {
      if (node.Type == NodeType.File)
        throw new InvalidOperationException("node is not a Directory");
      long folderSize1 = 0;
      foreach (INode node1 in allNodes.Where<INode>((Func<INode, bool>) (x => x.ParentId == node.Id)))
      {
        if (node1.Type == NodeType.File)
          folderSize1 += node1.Size;
        else if (node1.Type == NodeType.Directory)
        {
          long folderSize2 = node1.GetFolderSize(allNodes);
          folderSize1 += folderSize2;
        }
      }
      return folderSize1;
    }

    public static async Task<long> GetFolderSizeAsync(this INode node, IMegaApiClient client)
    {
      return await node.GetFolderSizeAsync(await client.GetNodesAsync());
    }

    public static async Task<long> GetFolderSizeAsync(this INode node, IEnumerable<INode> allNodes)
    {
      if (node.Type == NodeType.File)
        throw new InvalidOperationException("node is not a Directory");
      long folderSize = 0;
      foreach (INode node1 in allNodes.Where<INode>((Func<INode, bool>) (x => x.ParentId == node.Id)))
      {
        if (node1.Type == NodeType.File)
          folderSize += node1.Size;
        else if (node1.Type == NodeType.Directory)
          folderSize += await node1.GetFolderSizeAsync(allNodes);
      }
      return folderSize;
    }
  }
}
