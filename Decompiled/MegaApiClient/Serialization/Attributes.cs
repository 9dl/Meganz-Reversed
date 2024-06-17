// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.Attributes
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using DamienG.Security.Cryptography;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class Attributes
  {
    private const int CrcArrayLength = 4;
    private const int CrcSize = 16;
    private const int FingerprintMaxSize = 25;
    private const int MaxFull = 8192;
    private const uint CryptoPPCRC32Polynomial = 3988292384;

    [JsonConstructor]
    private Attributes()
    {
    }

    public Attributes(string name) => this.Name = name;

    public Attributes(string name, Attributes originalAttributes)
    {
      this.Name = name;
      this.SerializedFingerprint = originalAttributes.SerializedFingerprint;
    }

    public Attributes(string name, Stream stream, DateTime? modificationDate = null)
    {
      this.Name = name;
      if (!modificationDate.HasValue)
        return;
      byte[] array = new byte[25];
      Buffer.BlockCopy((Array) this.ComputeCrc(stream), 0, (Array) array, 0, 16);
      byte[] bytes = modificationDate.Value.ToEpoch().SerializeToBytes();
      Buffer.BlockCopy((Array) bytes, 0, (Array) array, 16, bytes.Length);
      Array.Resize<byte>(ref array, array.Length - 9 + bytes.Length);
      this.SerializedFingerprint = array.ToBase64();
    }

    [JsonProperty("n")]
    public string Name { get; set; }

    [JsonProperty("c", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SerializedFingerprint { get; set; }

    [JsonIgnore]
    public DateTime? ModificationDate { get; private set; }

    [System.Runtime.Serialization.OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
      if (this.SerializedFingerprint == null)
        return;
      byte[] data = this.SerializedFingerprint.FromBase64();
      this.ModificationDate = new DateTime?(data.DeserializeToLong(16, data.Length - 16).ToDateTime());
    }

    private uint[] ComputeCrc(Stream stream)
    {
      stream.Seek(0L, SeekOrigin.Begin);
      uint[] dst = new uint[4];
      byte[] numArray1 = new byte[16];
      uint num1 = 0;
      if (stream.Length <= 16L)
      {
        if (stream.Read(numArray1, 0, (int) stream.Length) != 0)
          Buffer.BlockCopy((Array) numArray1, 0, (Array) dst, 0, numArray1.Length);
      }
      else if (stream.Length <= 8192L)
      {
        byte[] buffer = new byte[stream.Length];
        int offset1 = 0;
        do
          ;
        while ((long) (offset1 += stream.Read(buffer, offset1, (int) stream.Length - offset1)) < stream.Length);
        for (int index = 0; index < dst.Length; ++index)
        {
          int offset2 = (int) ((long) index * stream.Length / (long) dst.Length);
          int num2 = (int) ((long) (index + 1) * stream.Length / (long) dst.Length);
          using (Crc32 crc32 = new Crc32(3988292384U, uint.MaxValue))
            num1 = BitConverter.ToUInt32(crc32.ComputeHash(buffer, offset2, num2 - offset2), 0);
          dst[index] = num1;
        }
      }
      else
      {
        byte[] buffer = new byte[64];
        uint num3 = (uint) (8192 / (buffer.Length * 4));
        long num4 = 0;
        for (uint index1 = 0; index1 < 4U; ++index1)
        {
          byte[] numArray2 = (byte[]) null;
          uint seed = uint.MaxValue;
          for (uint index2 = 0; index2 < num3; ++index2)
          {
            long num5 = (stream.Length - (long) buffer.Length) * (long) (index1 * num3 + index2) / (long) (uint) (4 * (int) num3 - 1);
            stream.Seek(num5 - num4, SeekOrigin.Current);
            long num6 = num4 + (num5 - num4);
            int count = stream.Read(buffer, 0, buffer.Length);
            num4 = num6 + (long) count;
            using (Crc32 crc32 = new Crc32(3988292384U, seed))
            {
              numArray2 = crc32.ComputeHash(buffer, 0, count);
              byte[] numArray3 = new byte[numArray2.Length];
              numArray2.CopyTo((Array) numArray3, 0);
              if (BitConverter.IsLittleEndian)
                Array.Reverse((Array) numArray3);
              seed = BitConverter.ToUInt32(numArray3, 0);
              seed = ~seed;
            }
          }
          uint uint32 = BitConverter.ToUInt32(numArray2, 0);
          dst[(int) index1] = uint32;
        }
      }
      return dst;
    }
  }
}
