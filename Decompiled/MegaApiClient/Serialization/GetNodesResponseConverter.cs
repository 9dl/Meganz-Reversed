// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.GetNodesResponseConverter
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  internal class GetNodesResponseConverter : JsonConverter
  {
    private readonly byte[] _masterKey;

    public GetNodesResponseConverter(byte[] masterKey) => this._masterKey = masterKey;

    public override bool CanConvert(Type objectType) => typeof (GetNodesResponse) == objectType;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      GetNodesResponse target = new GetNodesResponse(this._masterKey);
      JsonReader reader1 = jobject.CreateReader();
      reader1.Culture = reader.Culture;
      reader1.DateFormatString = reader.DateFormatString;
      reader1.DateParseHandling = reader.DateParseHandling;
      reader1.DateTimeZoneHandling = reader.DateTimeZoneHandling;
      reader1.FloatParseHandling = reader.FloatParseHandling;
      reader1.MaxDepth = reader.MaxDepth;
      reader1.SupportMultipleContent = reader.SupportMultipleContent;
      serializer.Populate(reader1, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }
  }
}
