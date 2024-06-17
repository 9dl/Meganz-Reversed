// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.MegaAesCtrStream
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal abstract class MegaAesCtrStream : Stream
  {
    protected readonly byte[] FileKey;
    protected readonly byte[] Iv;
    protected readonly long StreamLength;
    protected readonly byte[] MetaMac = new byte[8];
    protected long _position;
    private readonly Stream _stream;
    private readonly MegaAesCtrStream.Mode _mode;
    private readonly HashSet<long> _chunksPositionsCache;
    private readonly byte[] _counter = new byte[8];
    private readonly ICryptoTransform _encryptor;
    private long _currentCounter;
    private byte[] _currentChunkMac = new byte[16];
    private byte[] _fileMac = new byte[16];

    protected MegaAesCtrStream(
      Stream stream,
      long streamLength,
      MegaAesCtrStream.Mode mode,
      byte[] fileKey,
      byte[] iv)
    {
      if (fileKey == null || fileKey.Length != 16)
        throw new ArgumentException("Invalid fileKey");
      if (iv == null || iv.Length != 8)
        throw new ArgumentException("Invalid Iv");
      this._stream = stream ?? throw new ArgumentNullException(nameof (stream));
      this.StreamLength = streamLength;
      this._mode = mode;
      this.FileKey = fileKey;
      this.Iv = iv;
      this.ChunksPositions = this.GetChunksPositions(this.StreamLength).ToArray<long>();
      this._chunksPositionsCache = new HashSet<long>((IEnumerable<long>) this.ChunksPositions);
      this._encryptor = Crypto.CreateAesEncryptor(this.FileKey);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this._encryptor.Dispose();
    }

    public long[] ChunksPositions { get; }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => this.StreamLength;

    public override long Position
    {
      get => this._position;
      set
      {
        if (this._position != value)
          throw new NotSupportedException("Seek is not supported");
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this._position == this.StreamLength)
        return 0;
      if (this._position + (long) count < this.StreamLength && count < 16)
        throw new NotSupportedException("Invalid 'count' argument. Minimal read operation must be greater than 16 bytes (except for last read operation).");
      count = this._position + (long) count < this.StreamLength ? count - count % 16 : count;
      for (long position = this._position; position < Math.Min(this._position + (long) count, this.StreamLength); position += 16L)
      {
        if (this._chunksPositionsCache.Contains(position))
        {
          if (position != 0L)
            this.ComputeChunk(this._encryptor);
          for (int index = 0; index < 8; ++index)
          {
            this._currentChunkMac[index] = this.Iv[index];
            this._currentChunkMac[index + 8] = this.Iv[index];
          }
        }
        this.IncrementCounter();
        byte[] buffer1 = new byte[16];
        byte[] sourceArray = new byte[buffer1.Length];
        int offset1 = this._stream.Read(buffer1, 0, buffer1.Length);
        if (offset1 != buffer1.Length)
          offset1 += this._stream.Read(buffer1, offset1, buffer1.Length - offset1);
        byte[] numArray1 = new byte[16];
        Array.Copy((Array) this.Iv, (Array) numArray1, 8);
        Array.Copy((Array) this._counter, 0, (Array) numArray1, 8, 8);
        byte[] numArray2 = Crypto.EncryptAes(numArray1, this._encryptor);
        for (int index = 0; index < offset1; ++index)
        {
          sourceArray[index] = (byte) ((uint) numArray2[index] ^ (uint) buffer1[index]);
          this._currentChunkMac[index] ^= this._mode == MegaAesCtrStream.Mode.Crypt ? buffer1[index] : sourceArray[index];
        }
        Array.Copy((Array) sourceArray, 0, (Array) buffer, (int) ((long) offset + position - this._position), (int) Math.Min((long) sourceArray.Length, this.StreamLength - position));
        this._currentChunkMac = Crypto.EncryptAes(this._currentChunkMac, this._encryptor);
      }
      long num = Math.Min((long) count, this.StreamLength - this._position);
      this._position += num;
      if (this._position == this.StreamLength)
      {
        this.ComputeChunk(this._encryptor);
        for (int index = 0; index < 4; ++index)
        {
          this.MetaMac[index] = (byte) ((uint) this._fileMac[index] ^ (uint) this._fileMac[index + 4]);
          this.MetaMac[index + 4] = (byte) ((uint) this._fileMac[index + 8] ^ (uint) this._fileMac[index + 12]);
        }
        this.OnStreamRead();
      }
      return (int) num;
    }

    public override void Flush() => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    protected virtual void OnStreamRead()
    {
    }

    private void IncrementCounter()
    {
      if ((this._currentCounter & (long) byte.MaxValue) != (long) byte.MaxValue && (this._currentCounter & (long) byte.MaxValue) != 0L)
      {
        ++this._counter[7];
      }
      else
      {
        byte[] bytes = BitConverter.GetBytes(this._currentCounter);
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) bytes);
        Array.Copy((Array) bytes, (Array) this._counter, 8);
      }
      ++this._currentCounter;
    }

    private void ComputeChunk(ICryptoTransform encryptor)
    {
      for (int index = 0; index < 16; ++index)
        this._fileMac[index] ^= this._currentChunkMac[index];
      this._fileMac = Crypto.EncryptAes(this._fileMac, encryptor);
    }

    private IEnumerable<long> GetChunksPositions(long size)
    {
      yield return 0;
      long chunkStartPosition = 0;
      for (int idx = 1; idx <= 8 && chunkStartPosition < size - (long) (idx * 131072); ++idx)
      {
        chunkStartPosition += (long) (idx * 131072);
        yield return chunkStartPosition;
      }
      while (chunkStartPosition + 1048576L < size)
      {
        chunkStartPosition += 1048576L;
        yield return chunkStartPosition;
      }
    }

    protected enum Mode
    {
      Crypt,
      Decrypt,
    }
  }
}
