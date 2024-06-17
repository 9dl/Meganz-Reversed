// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.BufferedStream
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.IO;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal class BufferedStream : Stream
  {
    private const int BufferSize = 65536;
    private readonly Stream _innerStream;
    private readonly byte[] _streamBuffer = new byte[65536];
    private int _streamBufferDataStartIndex;
    private int _streamBufferDataCount;

    public BufferedStream(Stream innerStream) => this._innerStream = innerStream;

    public byte[] Buffer => this._streamBuffer;

    public int BufferOffset => this._streamBufferDataStartIndex;

    public int AvailableCount => this._streamBufferDataCount;

    public override void Flush() => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = 0;
      do
      {
        int length = Math.Min(this._streamBufferDataCount, count);
        if (length != 0)
        {
          Array.Copy((Array) this._streamBuffer, this._streamBufferDataStartIndex, (Array) buffer, offset, length);
          offset += length;
          count -= length;
          this._streamBufferDataStartIndex += length;
          this._streamBufferDataCount -= length;
          num += length;
        }
        if (count != 0)
        {
          this._streamBufferDataStartIndex = 0;
          this._streamBufferDataCount = 0;
          this.FillBuffer();
        }
        else
          break;
      }
      while (this._streamBufferDataCount != 0);
      return num;
    }

    public void FillBuffer()
    {
      while (true)
      {
        int offset = this._streamBufferDataStartIndex + this._streamBufferDataCount;
        int count = this._streamBuffer.Length - offset;
        if (count != 0)
        {
          int num = this._innerStream.Read(this._streamBuffer, offset, count);
          if (num != 0)
            this._streamBufferDataCount += num;
          else
            goto label_4;
        }
        else
          break;
      }
      return;
label_4:;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
  }
}
