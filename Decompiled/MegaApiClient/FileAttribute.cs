// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.FileAttribute
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal class FileAttribute : IFileAttribute
  {
    public FileAttribute(int id, FileAttributeType type, string handle)
    {
      this.Id = id;
      this.Type = type;
      this.Handle = handle;
    }

    public int Id { get; }

    public FileAttributeType Type { get; }

    public string Handle { get; }
  }
}
