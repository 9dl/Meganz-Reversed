// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Cryptography.Crypto
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Serialization;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

#nullable disable
namespace CG.Web.MegaApiClient.Cryptography
{
  internal class Crypto
  {
    private static readonly Aes s_aesCbc;
    private static readonly bool s_isKnownReusable;
    private static readonly byte[] s_defaultIv = new byte[16];

    static Crypto()
    {
      Crypto.s_aesCbc = (Aes) new AesManaged();
      Crypto.s_isKnownReusable = true;
      Crypto.s_aesCbc.Padding = PaddingMode.None;
      Crypto.s_aesCbc.Mode = CipherMode.CBC;
    }

    public static byte[] DecryptKey(byte[] data, byte[] key)
    {
      byte[] destinationArray = new byte[data.Length];
      for (int index = 0; index < data.Length; index += 16)
        Array.Copy((Array) Crypto.DecryptAes(data.CopySubArray<byte>(16, index), key), 0, (Array) destinationArray, index, 16);
      return destinationArray;
    }

    public static byte[] EncryptKey(byte[] data, byte[] key)
    {
      byte[] destinationArray = new byte[data.Length];
      using (ICryptoTransform aesEncryptor = Crypto.CreateAesEncryptor(key))
      {
        for (int index = 0; index < data.Length; index += 16)
          Array.Copy((Array) Crypto.EncryptAes(data.CopySubArray<byte>(16, index), aesEncryptor), 0, (Array) destinationArray, index, 16);
      }
      return destinationArray;
    }

    public static void GetPartsFromDecryptedKey(
      byte[] decryptedKey,
      out byte[] iv,
      out byte[] metaMac,
      out byte[] fileKey)
    {
      iv = new byte[8];
      metaMac = new byte[8];
      Array.Copy((Array) decryptedKey, 16, (Array) iv, 0, 8);
      Array.Copy((Array) decryptedKey, 24, (Array) metaMac, 0, 8);
      fileKey = new byte[16];
      for (int index = 0; index < 16; ++index)
        fileKey[index] = (byte) ((uint) decryptedKey[index] ^ (uint) decryptedKey[index + 16]);
    }

    public static byte[] DecryptAes(byte[] data, byte[] key)
    {
      using (ICryptoTransform decryptor = Crypto.s_aesCbc.CreateDecryptor(key, Crypto.s_defaultIv))
        return decryptor.TransformFinalBlock(data, 0, data.Length);
    }

    public static ICryptoTransform CreateAesEncryptor(byte[] key)
    {
      return (ICryptoTransform) new CachedCryptoTransform((Func<ICryptoTransform>) (() => Crypto.s_aesCbc.CreateEncryptor(key, Crypto.s_defaultIv)), Crypto.s_isKnownReusable);
    }

    public static byte[] EncryptAes(byte[] data, ICryptoTransform encryptor)
    {
      return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] EncryptAes(byte[] data, byte[] key)
    {
      using (ICryptoTransform aesEncryptor = Crypto.CreateAesEncryptor(key))
        return aesEncryptor.TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] CreateAesKey()
    {
      using (Aes aes = Aes.Create())
      {
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 128;
        aes.Padding = PaddingMode.None;
        aes.GenerateKey();
        return aes.Key;
      }
    }

    public static byte[] EncryptAttributes(Attributes attributes, byte[] nodeKey)
    {
      byte[] bytes = ("MEGA" + JsonConvert.SerializeObject((object) attributes, Formatting.None)).ToBytes();
      return Crypto.EncryptAes(bytes.CopySubArray<byte>(bytes.Length + 16 - bytes.Length % 16), nodeKey);
    }

    public static Attributes DecryptAttributes(byte[] attributes, byte[] nodeKey)
    {
      byte[] data = Crypto.DecryptAes(attributes, nodeKey);
      try
      {
        string str = data.ToUTF8String().Substring(4);
        int length = str.IndexOf(char.MinValue);
        if (length != -1)
          str = str.Substring(0, length);
        return JsonConvert.DeserializeObject<Attributes>(str);
      }
      catch (Exception ex)
      {
        return new Attributes(string.Format("Attribute deserialization failed: {0}", (object) ex.Message));
      }
    }

    public static BigInteger[] GetRsaPrivateKeyComponents(
      byte[] encodedRsaPrivateKey,
      byte[] masterKey)
    {
      encodedRsaPrivateKey = encodedRsaPrivateKey.CopySubArray<byte>(encodedRsaPrivateKey.Length + (16 - encodedRsaPrivateKey.Length % 16));
      byte[] numArray = Crypto.DecryptKey(encodedRsaPrivateKey, masterKey);
      BigInteger[] privateKeyComponents = new BigInteger[4];
      for (int index = 0; index < 4; ++index)
      {
        privateKeyComponents[index] = numArray.FromMPINumber();
        int num = ((int) numArray[0] * 256 + (int) numArray[1] + 7) / 8;
        numArray = numArray.CopySubArray<byte>(numArray.Length - num - 2, num + 2);
      }
      return privateKeyComponents;
    }

    public static byte[] RsaDecrypt(BigInteger data, BigInteger p, BigInteger q, BigInteger d)
    {
      return data.modPow(d, p * q).getBytes();
    }
  }
}
