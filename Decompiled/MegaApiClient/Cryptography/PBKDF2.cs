// Decompiled with JetBrains decompiler
// Type: Medo.Security.Cryptography.Pbkdf2
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace Medo.Security.Cryptography
{
  /// <summary>Generic PBKDF2 implementation.</summary>
  /// <example>This sample shows how to initialize class with SHA-256 HMAC.
  /// <code>
  /// using (var hmac = new HMACSHA256()) {
  ///     var df = new Pbkdf2(hmac, "password", "salt");
  ///     var bytes = df.GetBytes();
  /// }
  /// </code>
  /// </example>
  public class Pbkdf2
  {
    private readonly int BlockSize;
    private uint BlockIndex = 1;
    private byte[] BufferBytes;
    private int BufferStartIndex;
    private int BufferEndIndex;

    /// <summary>Creates new instance.</summary>
    /// <param name="algorithm">HMAC algorithm to use.</param>
    /// <param name="password">The password used to derive the key.</param>
    /// <param name="salt">The key salt used to derive the key.</param>
    /// <param name="iterations">The number of iterations for the operation.</param>
    /// <exception cref="T:System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
    public Pbkdf2(HMAC algorithm, byte[] password, byte[] salt, int iterations)
    {
      if (algorithm == null)
        throw new ArgumentNullException(nameof (algorithm), "Algorithm cannot be null.");
      if (salt == null)
        throw new ArgumentNullException(nameof (salt), "Salt cannot be null.");
      if (password == null)
        throw new ArgumentNullException(nameof (password), "Password cannot be null.");
      this.Algorithm = algorithm;
      this.Algorithm.Key = password;
      this.Salt = salt;
      this.IterationCount = iterations;
      this.BlockSize = this.Algorithm.HashSize / 8;
      this.BufferBytes = new byte[this.BlockSize];
    }

    /// <summary>Creates new instance.</summary>
    /// <param name="algorithm">HMAC algorithm to use.</param>
    /// <param name="password">The password used to derive the key.</param>
    /// <param name="salt">The key salt used to derive the key.</param>
    /// <exception cref="T:System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
    public Pbkdf2(HMAC algorithm, byte[] password, byte[] salt)
      : this(algorithm, password, salt, 1000)
    {
    }

    /// <summary>Creates new instance.</summary>
    /// <param name="algorithm">HMAC algorithm to use.</param>
    /// <param name="password">The password used to derive the key.</param>
    /// <param name="salt">The key salt used to derive the key.</param>
    /// <param name="iterations">The number of iterations for the operation.</param>
    /// <exception cref="T:System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
    public Pbkdf2(HMAC algorithm, string password, string salt, int iterations)
      : this(algorithm, Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), iterations)
    {
    }

    /// <summary>Creates new instance.</summary>
    /// <param name="algorithm">HMAC algorithm to use.</param>
    /// <param name="password">The password used to derive the key.</param>
    /// <param name="salt">The key salt used to derive the key.</param>
    /// <exception cref="T:System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
    public Pbkdf2(HMAC algorithm, string password, string salt)
      : this(algorithm, password, salt, 1000)
    {
    }

    /// <summary>Gets algorithm used for generating key.</summary>
    public HMAC Algorithm { get; private set; }

    /// <summary>Gets salt bytes.</summary>
    public byte[] Salt { get; private set; }

    /// <summary>Gets iteration count.</summary>
    public int IterationCount { get; private set; }

    /// <summary>
    /// Returns a pseudo-random key from a password, salt and iteration count.
    /// </summary>
    /// <param name="count">Number of bytes to return.</param>
    /// <returns>Byte array.</returns>
    public byte[] GetBytes(int count)
    {
      byte[] dst = new byte[count];
      int dstOffset = 0;
      int count1 = this.BufferEndIndex - this.BufferStartIndex;
      if (count1 > 0)
      {
        if (count < count1)
        {
          Buffer.BlockCopy((Array) this.BufferBytes, this.BufferStartIndex, (Array) dst, 0, count);
          this.BufferStartIndex += count;
          return dst;
        }
        Buffer.BlockCopy((Array) this.BufferBytes, this.BufferStartIndex, (Array) dst, 0, count1);
        this.BufferStartIndex = this.BufferEndIndex = 0;
        dstOffset += count1;
      }
      for (; dstOffset < count; dstOffset += this.BlockSize)
      {
        int count2 = count - dstOffset;
        this.BufferBytes = this.Func();
        if (count2 > this.BlockSize)
        {
          Buffer.BlockCopy((Array) this.BufferBytes, 0, (Array) dst, dstOffset, this.BlockSize);
        }
        else
        {
          Buffer.BlockCopy((Array) this.BufferBytes, 0, (Array) dst, dstOffset, count2);
          this.BufferStartIndex = count2;
          this.BufferEndIndex = this.BlockSize;
          return dst;
        }
      }
      return dst;
    }

    private byte[] Func()
    {
      byte[] numArray1 = new byte[this.Salt.Length + 4];
      Buffer.BlockCopy((Array) this.Salt, 0, (Array) numArray1, 0, this.Salt.Length);
      Buffer.BlockCopy((Array) Pbkdf2.GetBytesFromInt(this.BlockIndex), 0, (Array) numArray1, this.Salt.Length, 4);
      byte[] hash = this.Algorithm.ComputeHash(numArray1);
      byte[] numArray2 = hash;
      for (int index1 = 2; index1 <= this.IterationCount; ++index1)
      {
        hash = this.Algorithm.ComputeHash(hash, 0, hash.Length);
        for (int index2 = 0; index2 < this.BlockSize; ++index2)
          numArray2[index2] = (byte) ((uint) numArray2[index2] ^ (uint) hash[index2]);
      }
      if (this.BlockIndex == uint.MaxValue)
        throw new InvalidOperationException("Derived key too long.");
      ++this.BlockIndex;
      return numArray2;
    }

    private static byte[] GetBytesFromInt(uint i)
    {
      byte[] bytes = BitConverter.GetBytes(i);
      if (!BitConverter.IsLittleEndian)
        return bytes;
      return new byte[4]
      {
        bytes[3],
        bytes[2],
        bytes[1],
        bytes[0]
      };
    }
  }
}
