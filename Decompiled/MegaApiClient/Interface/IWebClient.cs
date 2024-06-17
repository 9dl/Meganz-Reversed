// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.IWebClient
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.IO;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public interface IWebClient
  {
    int BufferSize { get; set; }

    string PostRequestJson(Uri url, string jsonData);

    string PostRequestRaw(Uri url, Stream dataStream);

    Stream PostRequestRawAsStream(Uri url, Stream dataStream);

    Stream GetRequestRaw(Uri url);
  }
}
