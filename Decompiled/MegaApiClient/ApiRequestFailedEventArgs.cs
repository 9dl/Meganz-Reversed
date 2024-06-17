// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.ApiRequestFailedEventArgs
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class ApiRequestFailedEventArgs : EventArgs
  {
    public ApiRequestFailedEventArgs(
      Uri url,
      int attemptNum,
      TimeSpan retryDelay,
      ApiResultCode apiResult,
      string responseJson)
      : this(url, attemptNum, retryDelay, apiResult, responseJson, (Exception) null)
    {
    }

    public ApiRequestFailedEventArgs(
      Uri url,
      int attemptNum,
      TimeSpan retryDelay,
      ApiResultCode apiResult,
      Exception exception)
      : this(url, attemptNum, retryDelay, apiResult, (string) null, exception)
    {
    }

    private ApiRequestFailedEventArgs(
      Uri url,
      int attemptNum,
      TimeSpan retryDelay,
      ApiResultCode apiResult,
      string responseJson,
      Exception exception)
    {
      this.ApiUrl = url;
      this.AttemptNum = attemptNum;
      this.RetryDelay = retryDelay;
      this.ApiResult = apiResult;
      this.ResponseJson = responseJson;
      this.Exception = exception;
    }

    public Uri ApiUrl { get; }

    public ApiResultCode ApiResult { get; }

    public string ResponseJson { get; }

    public int AttemptNum { get; }

    public TimeSpan RetryDelay { get; }

    public Exception Exception { get; }
  }
}
