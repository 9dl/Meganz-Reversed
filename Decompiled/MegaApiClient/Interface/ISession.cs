// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.ISession
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Net;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public interface ISession
  {
    string Client { get; }

    IPAddress IpAddress { get; }

    string Country { get; }

    DateTime LoginTime { get; }

    DateTime LastSeenTime { get; }

    SessionStatus Status { get; }

    string SessionId { get; }
  }
}
