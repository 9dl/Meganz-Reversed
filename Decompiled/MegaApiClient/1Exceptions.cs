﻿// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.DownloadException
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class DownloadException : Exception
  {
    public DownloadException()
      : base("Invalid file checksum")
    {
    }
  }
}
