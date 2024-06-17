// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.ShareDataConverter
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using Newtonsoft.Json;
using System;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class ShareDataConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (!(value is ShareData shareData))
        throw new ArgumentException("invalid data to serialize");
      writer.WriteStartArray();
      writer.WriteStartArray();
      writer.WriteValue(shareData.NodeId);
      writer.WriteEndArray();
      writer.WriteStartArray();
      foreach (ShareData.ShareDataItem shareDataItem in shareData.Items)
        writer.WriteValue(shareDataItem.NodeId);
      writer.WriteEndArray();
      writer.WriteStartArray();
      int num = 0;
      foreach (ShareData.ShareDataItem shareDataItem in shareData.Items)
      {
        writer.WriteValue(0);
        writer.WriteValue(num++);
        writer.WriteValue(Crypto.EncryptKey(shareDataItem.Data, shareDataItem.Key).ToBase64());
      }
      writer.WriteEndArray();
      writer.WriteEndArray();
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (ShareData);
  }
}
