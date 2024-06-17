// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.WebClient
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Authentication;
using System.Text;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class WebClient : IWebClient
  {
    private const int DefaultResponseTimeout = -1;
    private static readonly HttpClient s_sharedHttpClient = WebClient.CreateHttpClient(-1, WebClient.GenerateUserAgent());
    private readonly HttpClient _httpClient;

    public WebClient(int responseTimeout = -1, ProductInfoHeaderValue userAgent = null)
    {
      if (responseTimeout == -1 && userAgent == null)
        this._httpClient = WebClient.s_sharedHttpClient;
      else
        this._httpClient = WebClient.CreateHttpClient(responseTimeout, userAgent ?? WebClient.GenerateUserAgent());
    }

    public int BufferSize { get; set; } = 65536;

    public string PostRequestJson(Uri url, string jsonData)
    {
      using (MemoryStream dataStream = new MemoryStream(jsonData.ToBytes()))
      {
        using (Stream stream = this.PostRequest(url, (Stream) dataStream, "application/json"))
          return this.StreamToString(stream);
      }
    }

    public string PostRequestRaw(Uri url, Stream dataStream)
    {
      using (Stream stream = this.PostRequest(url, dataStream, "application/json"))
        return this.StreamToString(stream);
    }

    public Stream PostRequestRawAsStream(Uri url, Stream dataStream)
    {
      return this.PostRequest(url, dataStream, "application/octet-stream");
    }

    public Stream GetRequestRaw(Uri url) => this._httpClient.GetStreamAsync(url).Result;

    private Stream PostRequest(Uri url, Stream dataStream, string contentType)
    {
      using (StreamContent streamContent = new StreamContent(dataStream, this.BufferSize))
      {
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        HttpResponseMessage result = this._httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
        {
          Content = (HttpContent) streamContent
        }, HttpCompletionOption.ResponseHeadersRead).Result;
        if (!result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.InternalServerError && result.ReasonPhrase == "Server Too Busy")
          return (Stream) new MemoryStream(Encoding.UTF8.GetBytes(-3L.ToString()));
        result.EnsureSuccessStatusCode();
        return result.Content.ReadAsStreamAsync().Result;
      }
    }

    private string StreamToString(Stream stream)
    {
      using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
        return streamReader.ReadToEnd();
    }

    private static HttpClient CreateHttpClient(int timeout, ProductInfoHeaderValue userAgent)
    {
      return new HttpClient((HttpMessageHandler) new HttpClientHandler()
      {
        SslProtocols = SslProtocols.Tls12,
        AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
      }, true)
      {
        Timeout = TimeSpan.FromMilliseconds((double) timeout),
        DefaultRequestHeaders = {
          UserAgent = {
            userAgent
          }
        }
      };
    }

    private static ProductInfoHeaderValue GenerateUserAgent()
    {
      AssemblyName name = typeof (WebClient).GetTypeInfo().Assembly.GetName();
      return new ProductInfoHeaderValue(name.Name, name.Version.ToString(2));
    }
  }
}
