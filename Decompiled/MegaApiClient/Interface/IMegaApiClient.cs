// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.IMegaApiClient
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public interface IMegaApiClient
  {
    event EventHandler<ApiRequestFailedEventArgs> ApiRequestFailed;

    bool IsLoggedIn { get; }

    CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(string email, string password);

    CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(
      string email,
      string password,
      string mfaKey);

    CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(CG.Web.MegaApiClient.MegaApiClient.AuthInfos authInfos);

    void Login(CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken logonSessionToken);

    void Login();

    void LoginAnonymous();

    void Logout();

    string GetRecoveryKey();

    IAccountInformation GetAccountInformation();

    IEnumerable<ISession> GetSessionsHistory();

    IEnumerable<INode> GetNodes();

    IEnumerable<INode> GetNodes(INode parent);

    void Delete(INode node, bool moveToTrash = true);

    INode CreateFolder(string name, INode parent);

    Uri GetDownloadLink(INode node);

    void DownloadFile(INode node, string outputFile, CancellationToken? cancellationToken = null);

    void DownloadFile(Uri uri, string outputFile, CancellationToken? cancellationToken = null);

    Stream Download(INode node, CancellationToken? cancellationToken = null);

    Stream Download(Uri uri, CancellationToken? cancellationToken = null);

    INode GetNodeFromLink(Uri uri);

    IEnumerable<INode> GetNodesFromLink(Uri uri);

    INode UploadFile(string filename, INode parent, CancellationToken? cancellationToken = null);

    INode Upload(
      Stream stream,
      string name,
      INode parent,
      DateTime? modificationDate = null,
      CancellationToken? cancellationToken = null);

    INode Move(INode node, INode destinationParentNode);

    INode Rename(INode node, string newName);

    CG.Web.MegaApiClient.MegaApiClient.AuthInfos GenerateAuthInfos(
      string email,
      string password,
      string mfaKey = null);

    Stream DownloadFileAttribute(
      INode node,
      FileAttributeType fileAttributeType,
      CancellationToken? cancellationToken = null);

    Task<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken> LoginAsync(
      string email,
      string password,
      string mfaKey = null);

    Task<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken> LoginAsync(
      CG.Web.MegaApiClient.MegaApiClient.AuthInfos authInfos);

    Task LoginAsync(CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken logonSessionToken);

    Task LoginAsync();

    Task LoginAnonymousAsync();

    Task LogoutAsync();

    Task<string> GetRecoveryKeyAsync();

    Task<IAccountInformation> GetAccountInformationAsync();

    Task<IEnumerable<ISession>> GetSessionsHistoryAsync();

    Task<IEnumerable<INode>> GetNodesAsync();

    Task<IEnumerable<INode>> GetNodesAsync(INode parent);

    Task<INode> CreateFolderAsync(string name, INode parent);

    Task DeleteAsync(INode node, bool moveToTrash = true);

    Task<INode> MoveAsync(INode sourceNode, INode destinationParentNode);

    Task<INode> RenameAsync(INode sourceNode, string newName);

    Task<Uri> GetDownloadLinkAsync(INode node);

    Task<Stream> DownloadAsync(
      INode node,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null);

    Task<Stream> DownloadAsync(
      Uri uri,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null);

    Task DownloadFileAsync(
      INode node,
      string outputFile,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null);

    Task DownloadFileAsync(
      Uri uri,
      string outputFile,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null);

    Task<INode> UploadAsync(
      Stream stream,
      string name,
      INode parent,
      IProgress<double> progress = null,
      DateTime? modificationDate = null,
      CancellationToken? cancellationToken = null);

    Task<INode> UploadFileAsync(
      string filename,
      INode parent,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null);

    Task<INode> GetNodeFromLinkAsync(Uri uri);

    Task<IEnumerable<INode>> GetNodesFromLinkAsync(Uri uri);

    Task<CG.Web.MegaApiClient.MegaApiClient.AuthInfos> GenerateAuthInfosAsync(
      string email,
      string password);

    Task<CG.Web.MegaApiClient.MegaApiClient.AuthInfos> GenerateAuthInfosAsync(
      string email,
      string password,
      string mfaKey);

    Task<Stream> DownloadFileAttributeAsync(
      INode node,
      FileAttributeType fileAttributeType,
      CancellationToken? cancellationToken = null);
  }
}
