// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Cryptography.CachedCryptoTransform
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Security.Cryptography;

#nullable disable
namespace CG.Web.MegaApiClient.Cryptography
{
  internal class CachedCryptoTransform : ICryptoTransform, IDisposable
  {
    private readonly Func<ICryptoTransform> _factory;
    private readonly bool _isKnownReusable;
    private ICryptoTransform _cachedInstance;

    public CachedCryptoTransform(Func<ICryptoTransform> factory, bool isKnownReusable)
    {
      this._factory = factory;
      this._isKnownReusable = isKnownReusable;
    }

    public void Dispose() => this._cachedInstance?.Dispose();

    public int TransformBlock(
      byte[] inputBuffer,
      int inputOffset,
      int inputCount,
      byte[] outputBuffer,
      int outputOffset)
    {
      return this.Forward<int>((Func<ICryptoTransform, int>) (x => x.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset)));
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
      return this._isKnownReusable && this._cachedInstance != null ? this._cachedInstance.TransformFinalBlock(inputBuffer, inputOffset, inputCount) : this.Forward<byte[]>((Func<ICryptoTransform, byte[]>) (x => x.TransformFinalBlock(inputBuffer, inputOffset, inputCount)));
    }

    public int InputBlockSize
    {
      get => this.Forward<int>((Func<ICryptoTransform, int>) (x => x.InputBlockSize));
    }

    public int OutputBlockSize
    {
      get => this.Forward<int>((Func<ICryptoTransform, int>) (x => x.OutputBlockSize));
    }

    public bool CanTransformMultipleBlocks
    {
      get => this.Forward<bool>((Func<ICryptoTransform, bool>) (x => x.CanTransformMultipleBlocks));
    }

    public bool CanReuseTransform
    {
      get => this.Forward<bool>((Func<ICryptoTransform, bool>) (x => x.CanReuseTransform));
    }

    private T Forward<T>(Func<ICryptoTransform, T> action)
    {
      ICryptoTransform cryptoTransform = this._cachedInstance ?? this._factory();
      try
      {
        return action(cryptoTransform);
      }
      finally
      {
        if (!this._isKnownReusable && !cryptoTransform.CanReuseTransform)
          cryptoTransform.Dispose();
        else
          this._cachedInstance = cryptoTransform;
      }
    }
  }
}
