// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Options
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Linq;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class Options
  {
    public const string DefaultApplicationKey = "axhQiYyQ";
    public const bool DefaultSynchronizeApiRequests = true;
    public const int DefaultApiRequestAttempts = 17;
    public const int DefaultApiRequestDelay = 100;
    public const float DefaultApiRequestDelayFactor = 1.5f;
    public const int DefaultBufferSize = 65536;
    public const int DefaultChunksPackSize = 1048576;
    public const long DefaultReportProgressChunkSize = 65536;

    public Options(
      string applicationKey = "axhQiYyQ",
      bool synchronizeApiRequests = true,
      Options.ComputeApiRequestRetryWaitDelayDelegate computeApiRequestRetryWaitDelay = null,
      int bufferSize = 65536,
      int chunksPackSize = 1048576,
      long reportProgressChunkSize = 65536)
    {
      this.ApplicationKey = applicationKey;
      this.SynchronizeApiRequests = synchronizeApiRequests;
      this.ComputeApiRequestRetryWaitDelay = computeApiRequestRetryWaitDelay ?? new Options.ComputeApiRequestRetryWaitDelayDelegate(this.ComputeDefaultApiRequestRetryWaitDelay);
      this.BufferSize = bufferSize;
      this.ChunksPackSize = chunksPackSize;
      this.ReportProgressChunkSize = reportProgressChunkSize >= (long) this.BufferSize ? reportProgressChunkSize : throw new ArgumentException(string.Format("ReportProgressChunkSize ({0}) cannot have a value lower than BufferSize ({1})", (object) reportProgressChunkSize, (object) bufferSize), nameof (reportProgressChunkSize));
    }

    public string ApplicationKey { get; }

    public bool SynchronizeApiRequests { get; }

    public Options.ComputeApiRequestRetryWaitDelayDelegate ComputeApiRequestRetryWaitDelay { get; }

    /// <summary>
    /// Size of the buffer used when downloading files
    /// This value has an impact on the progression.
    /// A lower value means more progression reports but a possible higher CPU usage
    /// </summary>
    public int BufferSize { get; }

    /// <summary>
    /// Upload is splitted in multiple fragments (useful for big uploads)
    /// The size of the fragments is defined by mega.nz and are the following:
    /// 0 / 128K / 384K / 768K / 1280K / 1920K / 2688K / 3584K / 4608K / ... (every 1024 KB) / EOF
    /// The upload method tries to upload multiple fragments at once.
    /// Fragments are merged until the total size reaches this value.
    /// The special value -1 merges all chunks in a single fragment and a single upload
    /// </summary>
    public int ChunksPackSize { get; internal set; }

    public long ReportProgressChunkSize { get; internal set; }

    private bool ComputeDefaultApiRequestRetryWaitDelay(int attempt, out TimeSpan delay)
    {
      if (attempt > 17)
      {
        delay = new TimeSpan();
        return false;
      }
      int num = Enumerable.Range(0, attempt).Aggregate<int, int>(0, (Func<int, int, int>) ((current, item) => current == 0 ? 100 : (int) ((double) current * 1.5)));
      delay = TimeSpan.FromMilliseconds((double) num);
      return true;
    }

    public delegate bool ComputeApiRequestRetryWaitDelayDelegate(int attempt, out TimeSpan delay);
  }
}
