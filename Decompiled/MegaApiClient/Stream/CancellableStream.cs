// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.CancellableStream
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.IO;
using System.Threading;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class CancellableStream : Stream
  {
    private Stream _stream;
    private readonly CancellationToken _cancellationToken;

    public CancellableStream(Stream stream, CancellationToken cancellationToken)
    {
      this._stream = stream ?? throw new ArgumentNullException(nameof (stream));
      this._cancellationToken = cancellationToken;
    }

    public override bool CanRead
    {
      get
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        return this._stream.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        return this._stream.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        return this._stream.CanWrite;
      }
    }

    public override void Flush()
    {
      this._cancellationToken.ThrowIfCancellationRequested();
      this._stream.Flush();
    }

    public override long Length
    {
      get
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        return this._stream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        return this._stream.Position;
      }
      set
      {
        this._cancellationToken.ThrowIfCancellationRequested();
        this._stream.Position = value;
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this._cancellationToken.ThrowIfCancellationRequested();
      return this._stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this._cancellationToken.ThrowIfCancellationRequested();
      return this._stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this._cancellationToken.ThrowIfCancellationRequested();
      this._stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._cancellationToken.ThrowIfCancellationRequested();
      this._stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._stream?.Dispose();
      this._stream = (Stream) null;
    }
  }
}
