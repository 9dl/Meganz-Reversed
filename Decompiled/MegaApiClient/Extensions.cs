// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Extensions
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace CG.Web.MegaApiClient
{
  internal static class Extensions
  {
    private static readonly DateTime s_epochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public static string ToBase64(this byte[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(Convert.ToBase64String(data));
      stringBuilder.Replace('+', '-');
      stringBuilder.Replace('/', '_');
      stringBuilder.Replace("=", string.Empty);
      return stringBuilder.ToString();
    }

    public static byte[] FromBase64(this string data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(data);
      stringBuilder.Append(string.Empty.PadRight((4 - data.Length % 4) % 4, '='));
      stringBuilder.Replace('-', '+');
      stringBuilder.Replace('_', '/');
      stringBuilder.Replace(",", string.Empty);
      return Convert.FromBase64String(stringBuilder.ToString());
    }

    public static string ToUTF8String(this byte[] data) => Encoding.UTF8.GetString(data);

    public static byte[] ToBytes(this string data) => Encoding.UTF8.GetBytes(data);

    public static byte[] ToBytesPassword(this string data)
    {
      uint[] source = new uint[data.Length + 3 >> 2];
      for (int index = 0; index < data.Length; ++index)
        source[index >> 2] |= (uint) data[index] << 24 - (index & 3) * 8;
      return ((IEnumerable<uint>) source).SelectMany<uint, byte>((Func<uint, IEnumerable<byte>>) (x =>
      {
        byte[] bytes = BitConverter.GetBytes(x);
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) bytes);
        return (IEnumerable<byte>) bytes;
      })).ToArray<byte>();
    }

    public static T[] CopySubArray<T>(this T[] source, int length, int offset = 0)
    {
      T[] objArray = new T[length];
      while (--length >= 0)
      {
        if (source.Length > offset + length)
          objArray[length] = source[offset + length];
      }
      return objArray;
    }

    public static BigInteger FromMPINumber(this byte[] data)
    {
      byte[] numArray = new byte[((int) data[0] * 256 + (int) data[1] + 7) / 8];
      Array.Copy((Array) data, 2, (Array) numArray, 0, numArray.Length);
      return new BigInteger((IList<byte>) numArray);
    }

    public static DateTime ToDateTime(this long seconds)
    {
      DateTime dateTime = Extensions.s_epochStart;
      dateTime = dateTime.AddSeconds((double) seconds);
      return dateTime.ToLocalTime();
    }

    public static long ToEpoch(this DateTime datetime)
    {
      return (long) datetime.ToUniversalTime().Subtract(Extensions.s_epochStart).TotalSeconds;
    }

    public static long DeserializeToLong(this byte[] data, int index, int length)
    {
      byte num1 = data[index];
      long num2 = 0;
      if (num1 > (byte) 8 || (int) num1 >= length)
        throw new ArgumentException("Invalid value");
      while (num1 > (byte) 0)
        num2 = (num2 << 8) + (long) data[index + (int) num1--];
      return num2;
    }

    public static byte[] SerializeToBytes(this long data)
    {
      byte[] array = new byte[9];
      byte num = 0;
      for (; data != 0L; data >>= 8)
        array[(int) ++num] = (byte) data;
      array[0] = num;
      Array.Resize<byte>(ref array, (int) array[0] + 1);
      return array;
    }
  }
}
