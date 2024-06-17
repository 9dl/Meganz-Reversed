// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.MegaAesCtrStreamCrypter
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using System;
using System.IO;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal class MegaAesCtrStreamCrypter : MegaAesCtrStream
  {
    public MegaAesCtrStreamCrypter(Stream stream)
      : base(stream, stream.Length, MegaAesCtrStream.Mode.Crypt, Crypto.CreateAesKey(), Crypto.CreateAesKey().CopySubArray<byte>(8))
    {
    }

    public byte[] FileKey => this.FileKey;

    public byte[] Iv => this.Iv;

    public byte[] MetaMac
    {
      get
      {
        if (this._position != this.StreamLength)
          throw new NotSupportedException("Stream must be fully read to obtain computed FileMac");
        return this.MetaMac;
      }
    }
  }
}
