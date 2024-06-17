// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.Serialization.SessionHistoryResponse
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

#nullable disable
namespace CG.Web.MegaApiClient.Serialization
{
  [JsonConverter(typeof (SessionHistoryResponse.SessionHistoryConverter))]
  internal class SessionHistoryResponse : Collection<ISession>
  {
    internal class SessionHistoryConverter : JsonConverter
    {
      public override bool CanConvert(Type objectType)
      {
        return typeof (SessionHistoryResponse.SessionHistoryConverter.Session) == objectType;
      }

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        if (reader.TokenType == JsonToken.Null)
          return (object) null;
        SessionHistoryResponse sessionHistoryResponse = new SessionHistoryResponse();
        foreach (JArray jArray in JArray.Load(reader).OfType<JArray>())
          sessionHistoryResponse.Add((ISession) new SessionHistoryResponse.SessionHistoryConverter.Session(jArray));
        return (object) sessionHistoryResponse;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        throw new NotSupportedException();
      }

      private class Session : ISession
      {
        public Session(JArray jArray)
        {
          try
          {
            this.LoginTime = jArray.Value<long>((object) 0).ToDateTime();
            this.LastSeenTime = jArray.Value<long>((object) 1).ToDateTime();
            this.Client = jArray.Value<string>((object) 2);
            this.IpAddress = IPAddress.Parse(jArray.Value<string>((object) 3));
            this.Country = jArray.Value<string>((object) 4);
            this.SessionId = jArray.Value<string>((object) 6);
            jArray.Value<long>((object) 7);
            if (jArray.Value<long>((object) 5) == 1L)
              this.Status |= SessionStatus.Current;
            if (jArray.Value<long>((object) 7) == 1L)
              this.Status |= SessionStatus.Active;
            if (this.Status != SessionStatus.Undefined)
              return;
            this.Status = SessionStatus.Expired;
          }
          catch (Exception ex)
          {
            this.Client = "Deserialization error: " + ex.Message;
          }
        }

        public string Client { get; private set; }

        public IPAddress IpAddress { get; private set; }

        public string Country { get; private set; }

        public DateTime LoginTime { get; private set; }

        public DateTime LastSeenTime { get; private set; }

        public SessionStatus Status { get; private set; }

        public string SessionId { get; private set; }
      }
    }
  }
}
