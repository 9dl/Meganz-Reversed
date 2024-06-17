// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.MegaAesCtrStreamDecrypter
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal class MegaAesCtrStreamDecrypter : MegaAesCtrStream
  {
    private readonly byte[] _expectedMetaMac;

    public MegaAesCtrStreamDecrypter(
      Stream stream,
      long streamLength,
      byte[] fileKey,
      byte[] iv,
      byte[] expectedMetaMac)
      : base(stream, streamLength, MegaAesCtrStream.Mode.Decrypt, fileKey, iv)
    {
      this._expectedMetaMac = expectedMetaMac != null && expectedMetaMac.Length == 8 ? expectedMetaMac : throw new ArgumentException("Invalid expectedMetaMac");
    }

    protected override void OnStreamRead()
    {
      if (!((IEnumerable<byte>) this._expectedMetaMac).SequenceEqual<byte>((IEnumerable<byte>) this.MetaMac))
        throw new DownloadException();
    }
  }
}
