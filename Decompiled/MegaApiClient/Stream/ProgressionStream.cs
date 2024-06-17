// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.ProgressionStream
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.IO;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal class ProgressionStream : Stream
  {
    private readonly Stream _baseStream;
    private readonly IProgress<double> _progress;
    private readonly long _reportProgressChunkSize;
    private long _chunkSize;

    public ProgressionStream(
      Stream baseStream,
      IProgress<double> progress,
      long reportProgressChunkSize)
    {
      this._baseStream = baseStream;
      this._progress = progress ?? (IProgress<double>) new Progress<double>();
      this._reportProgressChunkSize = reportProgressChunkSize;
    }

    public override int Read(byte[] array, int offset, int count)
    {
      int count1 = this._baseStream.Read(array, offset, count);
      this.ReportProgress(count1);
      return count1;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._baseStream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this._chunkSize == 0L)
        return;
      this._progress.Report(100.0);
    }

    public override void Flush() => this._baseStream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this._baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value) => this._baseStream.SetLength(value);

    public override bool CanRead => this._baseStream.CanRead;

    public override bool CanSeek => this._baseStream.CanSeek;

    public override bool CanWrite => this._baseStream.CanWrite;

    public override long Length => this._baseStream.Length;

    public override long Position
    {
      get => this._baseStream.Position;
      set => this._baseStream.Position = value;
    }

    private void ReportProgress(int count)
    {
      this._chunkSize += (long) count;
      if (this._chunkSize < this._reportProgressChunkSize)
        return;
      this._chunkSize = 0L;
      this._progress.Report((double) this.Position / (double) this.Length * 100.0);
    }
  }
}
