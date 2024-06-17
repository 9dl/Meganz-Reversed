// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.MegaApiClient
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using CG.Web.MegaApiClient.Cryptography;
using CG.Web.MegaApiClient.Serialization;
using Medo.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace CG.Web.MegaApiClient
{
  public class MegaApiClient : IMegaApiClient
  {
    private static readonly Uri s_baseApiUri = new Uri("https://g.api.mega.co.nz/cs");
    private static readonly Uri s_baseUri = new Uri("https://mega.nz");
    private readonly Options _options;
    private readonly IWebClient _webClient;
    private readonly object _apiRequestLocker = new object();
    private Node _trashNode;
    private string _sessionId;
    private byte[] _masterKey;
    private uint _sequenceIndex = (uint) ((double) uint.MaxValue * new Random().NextDouble());
    private bool _authenticatedLogin;

    /// <summary>
    /// Instantiate a new <see cref="T:CG.Web.MegaApiClient.MegaApiClient" /> object with default <see cref="T:CG.Web.MegaApiClient.Options" /> and default <see cref="T:CG.Web.MegaApiClient.IWebClient" />
    /// </summary>
    public MegaApiClient()
      : this(new Options(), (IWebClient) new WebClient())
    {
    }

    /// <summary>
    /// Instantiate a new <see cref="T:CG.Web.MegaApiClient.MegaApiClient" /> object with custom <see cref="T:CG.Web.MegaApiClient.Options" /> and default <see cref="T:CG.Web.MegaApiClient.IWebClient" />
    /// </summary>
    public MegaApiClient(Options options)
      : this(options, (IWebClient) new WebClient())
    {
    }

    /// <summary>
    /// Instantiate a new <see cref="T:CG.Web.MegaApiClient.MegaApiClient" /> object with default <see cref="T:CG.Web.MegaApiClient.Options" /> and custom <see cref="T:CG.Web.MegaApiClient.IWebClient" />
    /// </summary>
    public MegaApiClient(IWebClient webClient)
      : this(new Options(), webClient)
    {
    }

    /// <summary>
    /// Instantiate a new <see cref="T:CG.Web.MegaApiClient.MegaApiClient" /> object with custom <see cref="T:CG.Web.MegaApiClient.Options" /> and custom <see cref="T:CG.Web.MegaApiClient.IWebClient" />
    /// </summary>
    public MegaApiClient(Options options, IWebClient webClient)
    {
      this._options = options ?? throw new ArgumentNullException(nameof (options));
      this._webClient = webClient ?? throw new ArgumentNullException(nameof (webClient));
      this._webClient.BufferSize = options.BufferSize;
    }

    /// <summary>
    /// Generate authentication informations and store them in a serializable object to allow persistence
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="password">password</param>
    /// <param name="mfaKey"></param>
    /// <returns><see cref="T:CG.Web.MegaApiClient.MegaApiClient.AuthInfos" /> object containing encrypted data</returns>
    /// <exception cref="T:System.ArgumentNullException">email or password is null</exception>
    public CG.Web.MegaApiClient.MegaApiClient.AuthInfos GenerateAuthInfos(
      string email,
      string password,
      string mfaKey = null)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof (email));
      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException(nameof (password));
      PreLoginResponse preLoginResponse = this.Request<PreLoginResponse>((RequestBase) new PreLoginRequest(email));
      if (preLoginResponse.Version == 2 && !string.IsNullOrEmpty(preLoginResponse.Salt))
      {
        byte[] salt = preLoginResponse.Salt.FromBase64();
        byte[] bytesPassword = password.ToBytesPassword();
        byte[] source = new byte[32];
        using (HMACSHA512 algorithm = new HMACSHA512())
          source = new Pbkdf2((HMAC) algorithm, bytesPassword, salt, 100000).GetBytes(source.Length);
        return !string.IsNullOrEmpty(mfaKey) ? new CG.Web.MegaApiClient.MegaApiClient.AuthInfos(email, ((IEnumerable<byte>) source).Skip<byte>(16).ToArray<byte>().ToBase64(), ((IEnumerable<byte>) source).Take<byte>(16).ToArray<byte>(), mfaKey) : new CG.Web.MegaApiClient.MegaApiClient.AuthInfos(email, ((IEnumerable<byte>) source).Skip<byte>(16).ToArray<byte>().ToBase64(), ((IEnumerable<byte>) source).Take<byte>(16).ToArray<byte>());
      }
      if (preLoginResponse.Version != 1)
        throw new NotSupportedException("Version of account not supported");
      byte[] passwordAesKey = CG.Web.MegaApiClient.MegaApiClient.PrepareKey(password.ToBytesPassword());
      string hash = CG.Web.MegaApiClient.MegaApiClient.GenerateHash(email.ToLowerInvariant(), passwordAesKey);
      return !string.IsNullOrEmpty(mfaKey) ? new CG.Web.MegaApiClient.MegaApiClient.AuthInfos(email, hash, passwordAesKey, mfaKey) : new CG.Web.MegaApiClient.MegaApiClient.AuthInfos(email, hash, passwordAesKey);
    }

    public event EventHandler<ApiRequestFailedEventArgs> ApiRequestFailed;

    public bool IsLoggedIn => this._sessionId != null;

    /// <summary>
    /// Login to Mega.co.nz service using email/password credentials
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="password">password</param>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Service is not available or credentials are invalid</exception>
    /// <exception cref="T:System.ArgumentNullException">email or password is null</exception>
    /// <exception cref="T:System.NotSupportedException">Already logged in</exception>
    public CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(string email, string password)
    {
      return this.Login(this.GenerateAuthInfos(email, password, (string) null));
    }

    /// <summary>
    /// Login to Mega.co.nz service using email/password credentials
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="password">password</param>
    /// <param name="mfaKey"></param>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Service is not available or credentials are invalid</exception>
    /// <exception cref="T:System.ArgumentNullException">email or password is null</exception>
    /// <exception cref="T:System.NotSupportedException">Already logged in</exception>
    public CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(
      string email,
      string password,
      string mfaKey)
    {
      return this.Login(this.GenerateAuthInfos(email, password, mfaKey));
    }

    /// <summary>Login to Mega.co.nz service using hashed credentials</summary>
    /// <param name="authInfos">Authentication informations generated by <see cref="M:CG.Web.MegaApiClient.MegaApiClient.GenerateAuthInfos(System.String,System.String,System.String)" /> method</param>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Service is not available or authInfos is invalid</exception>
    /// <exception cref="T:System.ArgumentNullException">authInfos is null</exception>
    /// <exception cref="T:System.NotSupportedException">Already logged in</exception>
    public CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken Login(
      CG.Web.MegaApiClient.MegaApiClient.AuthInfos authInfos)
    {
      if (authInfos == null)
        throw new ArgumentNullException(nameof (authInfos));
      this.EnsureLoggedOut();
      this._authenticatedLogin = true;
      LoginResponse loginResponse = this.Request<LoginResponse>(string.IsNullOrEmpty(authInfos.MFAKey) ? (RequestBase) new LoginRequest(authInfos.Email, authInfos.Hash) : (RequestBase) new LoginRequest(authInfos.Email, authInfos.Hash, authInfos.MFAKey));
      this._masterKey = Crypto.DecryptKey(loginResponse.MasterKey.FromBase64(), authInfos.PasswordAesKey);
      BigInteger[] privateKeyComponents = Crypto.GetRsaPrivateKeyComponents(loginResponse.PrivateKey.FromBase64(), this._masterKey);
      this._sessionId = ((IEnumerable<byte>) Crypto.RsaDecrypt(loginResponse.SessionId.FromBase64().FromMPINumber(), privateKeyComponents[0], privateKeyComponents[1], privateKeyComponents[2])).Take<byte>(43).ToArray<byte>().ToBase64();
      return new CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken(this._sessionId, this._masterKey);
    }

    public void Login(CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken logonSessionToken)
    {
      this.EnsureLoggedOut();
      this._authenticatedLogin = true;
      this._sessionId = logonSessionToken.SessionId;
      this._masterKey = logonSessionToken.MasterKey;
    }

    /// <summary>Login anonymously to Mega.co.nz service</summary>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Throws if service is not available</exception>
    public void Login() => this.LoginAnonymous();

    /// <summary>Login anonymously to Mega.co.nz service</summary>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Throws if service is not available</exception>
    public void LoginAnonymous()
    {
      this.EnsureLoggedOut();
      this._authenticatedLogin = false;
      Random random = new Random();
      this._masterKey = new byte[16];
      random.NextBytes(this._masterKey);
      byte[] numArray1 = new byte[16];
      random.NextBytes(numArray1);
      byte[] numArray2 = new byte[16];
      random.NextBytes(numArray2);
      byte[] data = Crypto.EncryptAes(this._masterKey, numArray1);
      byte[] sourceArray = Crypto.EncryptAes(numArray2, this._masterKey);
      byte[] numArray3 = new byte[32];
      Array.Copy((Array) numArray2, 0, (Array) numArray3, 0, 16);
      Array.Copy((Array) sourceArray, 0, (Array) numArray3, 16, sourceArray.Length);
      this._sessionId = this.Request<LoginResponse>((RequestBase) new LoginRequest(this.Request((RequestBase) new AnonymousLoginRequest(data.ToBase64(), numArray3.ToBase64())), (string) null)).TemporarySessionId;
    }

    /// <summary>Logout from Mega.co.nz service</summary>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    public void Logout()
    {
      this.EnsureLoggedIn();
      if (this._authenticatedLogin)
        this.Request((RequestBase) new LogoutRequest());
      this._masterKey = (byte[]) null;
      this._sessionId = (string) null;
    }

    /// <summary>Retrieve recovery key</summary>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    public string GetRecoveryKey()
    {
      this.EnsureLoggedIn();
      if (!this._authenticatedLogin)
        throw new NotSupportedException("Anonymous login is not supported");
      return this._masterKey.ToBase64();
    }

    /// <summary>Retrieve account (quota) information</summary>
    /// <returns>An object containing account information</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    public IAccountInformation GetAccountInformation()
    {
      this.EnsureLoggedIn();
      return (IAccountInformation) this.Request<AccountInformationResponse>((RequestBase) new AccountInformationRequest());
    }

    /// <summary>Retrieve session history</summary>
    /// <returns>A collection of sessions</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    public IEnumerable<ISession> GetSessionsHistory()
    {
      this.EnsureLoggedIn();
      return (IEnumerable<ISession>) this.Request<SessionHistoryResponse>((RequestBase) new SessionHistoryRequest());
    }

    /// <summary>Retrieve all filesystem nodes</summary>
    /// <returns>Flat representation of all the filesystem nodes</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    public IEnumerable<INode> GetNodes()
    {
      this.EnsureLoggedIn();
      Node[] nodes = this.Request<GetNodesResponse>((RequestBase) new GetNodesRequest(), this._masterKey).Nodes;
      if (this._trashNode == null)
        this._trashNode = ((IEnumerable<Node>) nodes).First<Node>((Func<Node, bool>) (n => n.Type == NodeType.Trash));
      return ((IEnumerable<Node>) nodes).Distinct<Node>().OfType<INode>();
    }

    /// <summary>Retrieve children nodes of a parent node</summary>
    /// <returns>Flat representation of children nodes</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">Parent node is null</exception>
    public IEnumerable<INode> GetNodes(INode parent)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      return this.GetNodes().Where<INode>((Func<INode, bool>) (n => n.ParentId == parent.Id));
    }

    /// <summary>Delete a node from the filesytem</summary>
    /// <remarks>
    /// You can only delete <see cref="F:CG.Web.MegaApiClient.NodeType.Directory" /> or <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> node
    /// </remarks>
    /// <param name="node">Node to delete</param>
    /// <param name="moveToTrash">Moved to trash if true, Permanently deleted if false</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not a directory or a file</exception>
    public void Delete(INode node, bool moveToTrash = true)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Type != NodeType.Directory && node.Type != NodeType.File)
        throw new ArgumentException("Invalid node type");
      this.EnsureLoggedIn();
      if (moveToTrash)
        this.Move(node, (INode) this._trashNode);
      else
        this.Request((RequestBase) new DeleteRequest(node));
    }

    /// <summary>Create a folder on the filesytem</summary>
    /// <param name="name">Folder name</param>
    /// <param name="parent">Parent node to attach created folder</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">name or parent is null</exception>
    /// <exception cref="T:System.ArgumentException">parent is not valid (all types are allowed expect <see cref="F:CG.Web.MegaApiClient.NodeType.File" />)</exception>
    public INode CreateFolder(string name, INode parent)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (parent.Type == NodeType.File)
        throw new ArgumentException("Invalid parent node");
      this.EnsureLoggedIn();
      byte[] aesKey = Crypto.CreateAesKey();
      byte[] data1 = Crypto.EncryptAttributes(new Attributes(name), aesKey);
      byte[] data2 = Crypto.EncryptAes(aesKey, this._masterKey);
      return (INode) this.Request<GetNodesResponse>((RequestBase) CreateNodeRequest.CreateFolderNodeRequest(parent, data1.ToBase64(), data2.ToBase64(), aesKey), this._masterKey).Nodes[0];
    }

    /// <summary>Retrieve an url to download specified node</summary>
    /// <param name="node">Node to retrieve the download link (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> or <see cref="F:CG.Web.MegaApiClient.NodeType.Directory" /> can be downloaded)</param>
    /// <returns>Download link to retrieve the node with associated key</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not valid (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> or <see cref="F:CG.Web.MegaApiClient.NodeType.Directory" /> can be downloaded)</exception>
    public Uri GetDownloadLink(INode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Type != NodeType.File && node.Type != NodeType.Directory)
        throw new ArgumentException("Invalid node");
      this.EnsureLoggedIn();
      if (node.Type == NodeType.Directory)
      {
        this.Request((RequestBase) new ShareNodeRequest(node, this._masterKey, this.GetNodes()));
        node = this.GetNodes().First<INode>((Func<INode, bool>) (x => x.Equals(node)));
      }
      string str = node is INodeCrypto nodeCrypto ? this.Request<string>((RequestBase) new GetDownloadLinkRequest(node)) : throw new ArgumentException("node must implement INodeCrypto");
      return new Uri(CG.Web.MegaApiClient.MegaApiClient.s_baseUri, string.Format("/{0}/{1}#{2}", node.Type == NodeType.Directory ? (object) "folder" : (object) "file", (object) str, node.Type == NodeType.Directory ? (object) nodeCrypto.SharedKey.ToBase64() : (object) nodeCrypto.FullKey.ToBase64()));
    }

    /// <summary>
    /// Download a specified node and save it to the specified file
    /// </summary>
    /// <param name="node">Node to download (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</param>
    /// <param name="outputFile">File to save the node to</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node or outputFile is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not valid (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.DownloadException">Checksum is invalid. Downloaded data are corrupted</exception>
    public void DownloadFile(INode node, string outputFile, CancellationToken? cancellationToken = null)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (string.IsNullOrEmpty(outputFile))
        throw new ArgumentNullException(nameof (outputFile));
      using (Stream stream = this.Download(node, cancellationToken))
        this.SaveStream(stream, outputFile);
    }

    /// <summary>
    /// Download a specified Uri from Mega and save it to the specified file
    /// </summary>
    /// <param name="uri">Uri to download</param>
    /// <param name="outputFile">File to save the Uri to</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">uri or outputFile is null</exception>
    /// <exception cref="T:System.ArgumentException">Uri is not valid (id and key are required)</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.DownloadException">Checksum is invalid. Downloaded data are corrupted</exception>
    public void DownloadFile(Uri uri, string outputFile, CancellationToken? cancellationToken = null)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      if (string.IsNullOrEmpty(outputFile))
        throw new ArgumentNullException(nameof (outputFile));
      using (Stream stream = this.Download(uri, cancellationToken))
        this.SaveStream(stream, outputFile);
    }

    /// <summary>
    /// Retrieve a Stream to download and decrypt the specified node
    /// </summary>
    /// <param name="node">Node to download (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node or outputFile is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not valid (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.DownloadException">Checksum is invalid. Downloaded data are corrupted</exception>
    public Stream Download(INode node, CancellationToken? cancellationToken = null)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Type != NodeType.File)
        throw new ArgumentException("Invalid node");
      if (!(node is INodeCrypto nodeCrypto))
        throw new ArgumentException("node must implement INodeCrypto");
      this.EnsureLoggedIn();
      DownloadUrlResponse downloadUrlResponse = this.Request<DownloadUrlResponse>(!(node is PublicNode publicNode) || publicNode.ParentId != null ? (RequestBase) new DownloadUrlRequest(node) : (RequestBase) new DownloadUrlRequestFromId(node.Id));
      Stream stream = (Stream) new MegaAesCtrStreamDecrypter((Stream) new BufferedStream(this._webClient.GetRequestRaw(new Uri(downloadUrlResponse.Url))), downloadUrlResponse.Size, nodeCrypto.Key, nodeCrypto.Iv, nodeCrypto.MetaMac);
      if (cancellationToken.HasValue)
        stream = (Stream) new CancellableStream(stream, cancellationToken.Value);
      return stream;
    }

    /// <summary>
    /// Retrieve a Stream to download and decrypt the specified Uri
    /// </summary>
    /// <param name="uri">Uri to download</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">uri is null</exception>
    /// <exception cref="T:System.ArgumentException">Uri is not valid (id and key are required)</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.DownloadException">Checksum is invalid. Downloaded data are corrupted</exception>
    public Stream Download(Uri uri, CancellationToken? cancellationToken = null)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      this.EnsureLoggedIn();
      string id;
      byte[] iv;
      byte[] metaMac;
      byte[] key;
      this.GetPartsFromUri(uri, out id, out iv, out metaMac, out key);
      DownloadUrlResponse downloadUrlResponse = this.Request<DownloadUrlResponse>((RequestBase) new DownloadUrlRequestFromId(id));
      Stream stream = (Stream) new MegaAesCtrStreamDecrypter((Stream) new BufferedStream(this._webClient.GetRequestRaw(new Uri(downloadUrlResponse.Url))), downloadUrlResponse.Size, key, iv, metaMac);
      if (cancellationToken.HasValue)
        stream = (Stream) new CancellableStream(stream, cancellationToken.Value);
      return stream;
    }

    /// <summary>
    /// Retrieve public properties of a file from a specified Uri
    /// </summary>
    /// <param name="uri">Uri to retrive properties</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">uri is null</exception>
    /// <exception cref="T:System.ArgumentException">Uri is not valid (id and key are required)</exception>
    public INode GetNodeFromLink(Uri uri)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      this.EnsureLoggedIn();
      string id;
      byte[] iv;
      byte[] metaMac;
      byte[] key;
      this.GetPartsFromUri(uri, out id, out iv, out metaMac, out key);
      DownloadUrlResponse downloadResponse = this.Request<DownloadUrlResponse>((RequestBase) new DownloadUrlRequestFromId(id));
      return (INode) new PublicNode(new Node(id, downloadResponse, key, iv, metaMac), (string) null);
    }

    /// <summary>Retrieve list of nodes from a specified Uri</summary>
    /// <param name="uri">Uri</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">uri is null</exception>
    /// <exception cref="T:System.ArgumentException">Uri is not valid (id and key are required)</exception>
    public IEnumerable<INode> GetNodesFromLink(Uri uri)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      this.EnsureLoggedIn();
      byte[] key;
      string shareId;
      this.GetPartsFromUri(uri, out shareId, out byte[] _, out byte[] _, out key);
      return ((IEnumerable<Node>) this.Request<GetNodesResponse>((RequestBase) new GetNodesRequest(shareId), key).Nodes).Select<Node, PublicNode>((Func<Node, PublicNode>) (x => new PublicNode(x, shareId))).OfType<INode>();
    }

    /// <summary>
    /// Upload a file on Mega.co.nz and attach created node to selected parent
    /// </summary>
    /// <param name="filename">File to upload</param>
    /// <param name="parent">Node to attach the uploaded file (all types except <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <returns>Created node</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">filename or parent is null</exception>
    /// <exception cref="T:System.IO.FileNotFoundException">filename is not found</exception>
    /// <exception cref="T:System.ArgumentException">parent is not valid (all types except <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</exception>
    public INode UploadFile(string filename, INode parent, CancellationToken? cancellationToken = null)
    {
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException(nameof (filename));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (!File.Exists(filename))
        throw new FileNotFoundException(filename);
      this.EnsureLoggedIn();
      DateTime lastWriteTime = File.GetLastWriteTime(filename);
      using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        return this.Upload((Stream) fileStream, Path.GetFileName(filename), parent, new DateTime?(lastWriteTime), cancellationToken);
    }

    /// <summary>
    /// Upload a stream on Mega.co.nz and attach created node to selected parent
    /// </summary>
    /// <param name="stream">Data to upload</param>
    /// <param name="name">Created node name</param>
    /// <param name="modificationDate">Custom modification date stored in the Node attributes</param>
    /// <param name="parent">Node to attach the uploaded file (all types except <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <returns>Created node</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">stream or name or parent is null</exception>
    /// <exception cref="T:System.ArgumentException">parent is not valid (all types except <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</exception>
    public INode Upload(
      Stream stream,
      string name,
      INode parent,
      DateTime? modificationDate = null,
      CancellationToken? cancellationToken = null)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (parent.Type == NodeType.File)
        throw new ArgumentException("Invalid parent node");
      if (parent is PublicNode)
        throw new ApiException(ApiResultCode.AccessDenied);
      this.EnsureLoggedIn();
      if (cancellationToken.HasValue)
        stream = (Stream) new CancellableStream(stream, cancellationToken.Value);
      string str = string.Empty;
      int attemptNum = 0;
      TimeSpan delay;
      while (this._options.ComputeApiRequestRetryWaitDelay(++attemptNum, out delay))
      {
        UploadUrlResponse uploadUrlResponse = this.Request<UploadUrlResponse>((RequestBase) new UploadUrlRequest(stream.Length));
        ApiResultCode apiResultCode = ApiResultCode.Ok;
        using (MegaAesCtrStreamCrypter ctrStreamCrypter = new MegaAesCtrStreamCrypter(stream))
        {
          long num = 0;
          int[] array = this.ComputeChunksSizesToUpload(ctrStreamCrypter.ChunksPositions, ctrStreamCrypter.Length).ToArray<int>();
          Uri url = (Uri) null;
          for (int index = 0; index < array.Length; ++index)
          {
            str = string.Empty;
            int count = array[index];
            byte[] buffer = new byte[count];
            ctrStreamCrypter.Read(buffer, 0, count);
            using (MemoryStream dataStream = new MemoryStream(buffer))
            {
              url = new Uri(uploadUrlResponse.Url + "/" + num.ToString());
              num += (long) count;
              try
              {
                str = this._webClient.PostRequestRaw(url, (Stream) dataStream);
                if (string.IsNullOrEmpty(str))
                  apiResultCode = ApiResultCode.Ok;
                else if (str.FromBase64().Length != 27)
                {
                  long result;
                  if (long.TryParse(str, out result))
                  {
                    apiResultCode = (ApiResultCode) result;
                    break;
                  }
                }
              }
              catch (Exception ex)
              {
                apiResultCode = ApiResultCode.RequestFailedRetry;
                EventHandler<ApiRequestFailedEventArgs> apiRequestFailed = this.ApiRequestFailed;
                if (apiRequestFailed != null)
                {
                  apiRequestFailed((object) this, new ApiRequestFailedEventArgs(url, attemptNum, delay, apiResultCode, ex));
                  break;
                }
                break;
              }
            }
          }
          if (apiResultCode != ApiResultCode.Ok)
          {
            EventHandler<ApiRequestFailedEventArgs> apiRequestFailed = this.ApiRequestFailed;
            if (apiRequestFailed != null)
              apiRequestFailed((object) this, new ApiRequestFailedEventArgs(url, attemptNum, delay, apiResultCode, str));
            if (apiResultCode != ApiResultCode.RequestFailedRetry && apiResultCode != ApiResultCode.RequestFailedPermanetly && apiResultCode != ApiResultCode.TooManyRequests)
              throw new ApiException(apiResultCode);
            this.Wait(delay);
            stream.Seek(0L, SeekOrigin.Begin);
          }
          else
          {
            byte[] data1 = Crypto.EncryptAttributes(new Attributes(name, stream, modificationDate), ctrStreamCrypter.FileKey);
            byte[] numArray = new byte[32];
            for (int index = 0; index < 8; ++index)
            {
              numArray[index] = (byte) ((uint) ctrStreamCrypter.FileKey[index] ^ (uint) ctrStreamCrypter.Iv[index]);
              numArray[index + 16] = ctrStreamCrypter.Iv[index];
            }
            for (int index = 8; index < 16; ++index)
            {
              numArray[index] = (byte) ((uint) ctrStreamCrypter.FileKey[index] ^ (uint) ctrStreamCrypter.MetaMac[index - 8]);
              numArray[index + 16] = ctrStreamCrypter.MetaMac[index - 8];
            }
            byte[] data2 = Crypto.EncryptKey(numArray, this._masterKey);
            return (INode) this.Request<GetNodesResponse>((RequestBase) CreateNodeRequest.CreateFileNodeRequest(parent, data1.ToBase64(), data2.ToBase64(), numArray, str), this._masterKey).Nodes[0];
          }
        }
      }
      throw new UploadException(str);
    }

    /// <summary>Change node parent</summary>
    /// <param name="node">Node to move</param>
    /// <param name="destinationParentNode">New parent</param>
    /// <returns>Moved node</returns>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node or destinationParentNode is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not valid (only <see cref="F:CG.Web.MegaApiClient.NodeType.Directory" /> and <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</exception>
    /// <exception cref="T:System.ArgumentException">parent is not valid (all types except <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> are supported)</exception>
    public INode Move(INode node, INode destinationParentNode)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (destinationParentNode == null)
        throw new ArgumentNullException(nameof (destinationParentNode));
      if (node.Type != NodeType.Directory && node.Type != NodeType.File)
        throw new ArgumentException("Invalid node type");
      if (destinationParentNode.Type == NodeType.File)
        throw new ArgumentException("Invalid destination parent node");
      this.EnsureLoggedIn();
      this.Request((RequestBase) new MoveRequest(node, destinationParentNode));
      return this.GetNodes().First<INode>((Func<INode, bool>) (n => n.Equals(node)));
    }

    public INode Rename(INode node, string newName)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Type != NodeType.Directory && node.Type != NodeType.File)
        throw new ArgumentException("Invalid node type");
      if (string.IsNullOrEmpty(newName))
        throw new ArgumentNullException(nameof (newName));
      if (!(node is INodeCrypto nodeCrypto))
        throw new ArgumentException("node must implement INodeCrypto");
      this.EnsureLoggedIn();
      byte[] data = Crypto.EncryptAttributes(new Attributes(newName, ((Node) node).Attributes), nodeCrypto.Key);
      this.Request((RequestBase) new RenameRequest(node, data.ToBase64()));
      return this.GetNodes().First<INode>((Func<INode, bool>) (n => n.Equals(node)));
    }

    /// <summary>
    /// Download thumbnail from file attributes (or return null if thumbnail is not available)
    /// </summary>
    /// <param name="node">Node to download the thumbnail from (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</param>
    /// <param name="fileAttributeType">File attribute type to retrieve</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the action</param>
    /// <exception cref="T:System.NotSupportedException">Not logged in</exception>
    /// <exception cref="T:CG.Web.MegaApiClient.ApiException">Mega.co.nz service reports an error</exception>
    /// <exception cref="T:System.ArgumentNullException">node or outputFile is null</exception>
    /// <exception cref="T:System.ArgumentException">node is not valid (only <see cref="F:CG.Web.MegaApiClient.NodeType.File" /> can be downloaded)</exception>
    /// <exception cref="T:System.InvalidOperationException">file attribute data is invalid</exception>
    public Stream DownloadFileAttribute(
      INode node,
      FileAttributeType fileAttributeType,
      CancellationToken? cancellationToken = null)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Type != NodeType.File)
        throw new ArgumentException("Invalid node");
      if (!(node is INodeCrypto nodeCrypto))
        throw new ArgumentException("node must implement INodeCrypto");
      this.EnsureLoggedIn();
      IFileAttribute fileAttribute = ((IEnumerable<IFileAttribute>) node.FileAttributes).FirstOrDefault<IFileAttribute>((Func<IFileAttribute, bool>) (_ => _.Type == fileAttributeType));
      if (fileAttribute == null)
        return (Stream) null;
      DownloadFileAttributeResponse attributeResponse = this.Request<DownloadFileAttributeResponse>((RequestBase) new DownloadFileAttributeRequest(fileAttribute.Handle));
      byte[] numArray1 = fileAttribute.Handle.FromBase64();
      using (Stream stream1 = this._webClient.PostRequestRawAsStream(new Uri(attributeResponse.Url + "/0"), (Stream) new MemoryStream(numArray1)))
      {
        using (MemoryStream destination = new MemoryStream())
        {
          stream1.CopyTo((Stream) destination);
          destination.Position = 0L;
          byte[] array = destination.ToArray();
          byte[] numArray2 = array.CopySubArray<byte>(8);
          if (!((IEnumerable<byte>) numArray2).SequenceEqual<byte>((IEnumerable<byte>) numArray1))
            throw new InvalidOperationException("File attribute handle mismatch (" + fileAttribute.Handle + " requested but " + numArray2.ToBase64() + " received)");
          uint uint32 = BitConverter.ToUInt32(array.CopySubArray<byte>(4, 8), 0);
          if ((long) uint32 != (long) (array.Length - 12))
            throw new InvalidOperationException(string.Format("File attribute size mismatch ({0} expected but {1} received)", (object) uint32, (object) (array.Length - 12)));
          Stream stream2 = (Stream) new MemoryStream(Crypto.DecryptAes(array.CopySubArray<byte>(array.Length - 12, 12), nodeCrypto.Key));
          if (cancellationToken.HasValue)
            stream2 = (Stream) new CancellableStream(stream2, cancellationToken.Value);
          return stream2;
        }
      }
    }

    private static string GenerateHash(string email, byte[] passwordAesKey)
    {
      byte[] bytes = email.ToBytes();
      byte[] numArray1 = new byte[16];
      for (int index = 0; index < bytes.Length; ++index)
        numArray1[index % 16] ^= bytes[index];
      using (ICryptoTransform aesEncryptor = Crypto.CreateAesEncryptor(passwordAesKey))
      {
        for (int index = 0; index < 16384; ++index)
          numArray1 = Crypto.EncryptAes(numArray1, aesEncryptor);
      }
      byte[] numArray2 = new byte[8];
      Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, 4);
      Array.Copy((Array) numArray1, 8, (Array) numArray2, 4, 4);
      return numArray2.ToBase64();
    }

    private static byte[] PrepareKey(byte[] data)
    {
      byte[] data1 = new byte[16]
      {
        (byte) 147,
        (byte) 196,
        (byte) 103,
        (byte) 227,
        (byte) 125,
        (byte) 176,
        (byte) 199,
        (byte) 164,
        (byte) 209,
        (byte) 190,
        (byte) 63,
        (byte) 129,
        (byte) 1,
        (byte) 82,
        (byte) 203,
        (byte) 86
      };
      for (int index = 0; index < 65536; ++index)
      {
        for (int offset = 0; offset < data.Length; offset += 16)
        {
          byte[] key = data.CopySubArray<byte>(16, offset);
          data1 = Crypto.EncryptAes(data1, key);
        }
      }
      return data1;
    }

    private string Request(RequestBase request) => this.Request<string>(request);

    private TResponse Request<TResponse>(RequestBase request, byte[] key = null) where TResponse : class
    {
      if (!this._options.SynchronizeApiRequests)
        return this.RequestCore<TResponse>(request, key);
      lock (this._apiRequestLocker)
        return this.RequestCore<TResponse>(request, key);
    }

    private TResponse RequestCore<TResponse>(RequestBase request, byte[] key) where TResponse : class
    {
      string jsonData = JsonConvert.SerializeObject((object) new object[1]
      {
        (object) request
      });
      Uri url = this.GenerateUrl(request.QueryArguments);
      object obj = (object) null;
      int attemptNum = 0;
      TimeSpan delay;
      while (this._options.ComputeApiRequestRetryWaitDelay(++attemptNum, out delay))
      {
        string responseJson = this._webClient.PostRequestJson(url, jsonData);
        if (!string.IsNullOrEmpty(responseJson) && (obj = JsonConvert.DeserializeObject(responseJson)) != null)
        {
          switch (obj)
          {
            case long _:
              break;
            case JArray jarray:
              if (jarray[0].Type != JTokenType.Integer)
                goto label_12;
              else
                break;
            default:
              goto label_12;
          }
        }
        ApiResultCode apiResultCode = obj == null ? ApiResultCode.RequestFailedRetry : (obj is long ? (ApiResultCode) Enum.ToObject(typeof (ApiResultCode), obj) : (ApiResultCode) ((JArray) obj)[0].Value<int>());
        if (apiResultCode != ApiResultCode.Ok)
        {
          EventHandler<ApiRequestFailedEventArgs> apiRequestFailed = this.ApiRequestFailed;
          if (apiRequestFailed != null)
            apiRequestFailed((object) this, new ApiRequestFailedEventArgs(url, attemptNum, delay, apiResultCode, responseJson));
        }
        if (apiResultCode == ApiResultCode.RequestFailedRetry)
        {
          this.Wait(delay);
        }
        else
        {
          if (apiResultCode != ApiResultCode.Ok)
            throw new ApiException(apiResultCode);
          break;
        }
      }
label_12:
      string str = ((JArray) obj)[0].ToString();
      if (typeof (TResponse) == typeof (string))
        return str as TResponse;
      return JsonConvert.DeserializeObject<TResponse>(str, (JsonConverter) new GetNodesResponseConverter(key));
    }

    private void Wait(TimeSpan retryDelay) => Task.Delay(retryDelay).Wait();

    private Uri GenerateUrl(Dictionary<string, string> queryArguments)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) queryArguments)
      {
        ["id"] = (this._sequenceIndex++ % uint.MaxValue).ToString((IFormatProvider) CultureInfo.InvariantCulture),
        ["ak"] = this._options.ApplicationKey
      };
      if (!string.IsNullOrEmpty(this._sessionId))
        dictionary["sid"] = this._sessionId;
      UriBuilder uriBuilder = new UriBuilder(CG.Web.MegaApiClient.MegaApiClient.s_baseApiUri);
      string str1 = "";
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        str1 = str1 + keyValuePair.Key + "=" + keyValuePair.Value + "&";
      string str2 = str1.Substring(0, str1.Length - 1);
      uriBuilder.Query = str2;
      return uriBuilder.Uri;
    }

    private void SaveStream(Stream stream, string outputFile)
    {
      using (FileStream destination = new FileStream(outputFile, FileMode.CreateNew, FileAccess.Write))
        stream.CopyTo((Stream) destination, this._options.BufferSize);
    }

    private void EnsureLoggedIn()
    {
      if (this._sessionId == null)
        throw new NotSupportedException("Not logged in");
    }

    private void EnsureLoggedOut()
    {
      if (this._sessionId != null)
        throw new NotSupportedException("Already logged in");
    }

    private void GetPartsFromUri(
      Uri uri,
      out string id,
      out byte[] iv,
      out byte[] metaMac,
      out byte[] key)
    {
      byte[] decryptedKey;
      bool isFolder;
      if (!this.TryGetPartsFromUri(uri, out id, out decryptedKey, out isFolder) && !this.TryGetPartsFromLegacyUri(uri, out id, out decryptedKey, out isFolder))
        throw new ArgumentException(string.Format("Invalid uri. Unable to extract Id and Key from the uri {0}", (object) uri));
      if (isFolder)
      {
        iv = (byte[]) null;
        metaMac = (byte[]) null;
        key = decryptedKey;
      }
      else
        Crypto.GetPartsFromDecryptedKey(decryptedKey, out iv, out metaMac, out key);
    }

    private bool TryGetPartsFromUri(
      Uri uri,
      out string id,
      out byte[] decryptedKey,
      out bool isFolder)
    {
      Match match = new Regex("/(?<type>(file|folder))/(?<id>[^#]+)#(?<key>[^$/]+)").Match(uri.PathAndQuery + uri.Fragment);
      if (match.Success)
      {
        id = match.Groups[nameof (id)].Value;
        decryptedKey = match.Groups["key"].Value.FromBase64();
        isFolder = match.Groups["type"].Value == "folder";
        return true;
      }
      id = (string) null;
      decryptedKey = (byte[]) null;
      isFolder = false;
      return false;
    }

    private bool TryGetPartsFromLegacyUri(
      Uri uri,
      out string id,
      out byte[] decryptedKey,
      out bool isFolder)
    {
      Match match = new Regex("#(?<type>F?)!(?<id>[^!]+)!(?<key>[^$!\\?]+)").Match(uri.Fragment);
      if (match.Success)
      {
        id = match.Groups[nameof (id)].Value;
        decryptedKey = match.Groups["key"].Value.FromBase64();
        isFolder = match.Groups["type"].Value == "F";
        return true;
      }
      id = (string) null;
      decryptedKey = (byte[]) null;
      isFolder = false;
      return false;
    }

    private IEnumerable<int> ComputeChunksSizesToUpload(long[] chunksPositions, long streamLength)
    {
      for (int i = 0; i < chunksPositions.Length; ++i)
      {
        long chunksPosition = chunksPositions[i];
        long num;
        for (num = i == chunksPositions.Length - 1 ? streamLength : chunksPositions[i + 1]; ((int) (num - chunksPosition) < this._options.ChunksPackSize || this._options.ChunksPackSize == -1) && i < chunksPositions.Length - 1; num = i == chunksPositions.Length - 1 ? streamLength : chunksPositions[i + 1])
          ++i;
        yield return (int) (num - chunksPosition);
      }
    }

    public Task<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken> LoginAsync(
      string email,
      string password,
      string mfaKey = null)
    {
      return Task.Run<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken>((Func<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken>) (() => this.Login(email, password, mfaKey)));
    }

    public Task<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken> LoginAsync(
      CG.Web.MegaApiClient.MegaApiClient.AuthInfos authInfos)
    {
      return Task.Run<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken>((Func<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken>) (() => this.Login(authInfos)));
    }

    public Task LoginAsync(CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken logonSessionToken)
    {
      return Task.Run((Action) (() => this.Login(logonSessionToken)));
    }

    public Task LoginAsync() => Task.Run((Action) (() => this.Login()));

    public Task LoginAnonymousAsync() => Task.Run((Action) (() => this.LoginAnonymous()));

    public Task LogoutAsync() => Task.Run((Action) (() => this.Logout()));

    public Task<string> GetRecoveryKeyAsync() => Task.FromResult<string>(this.GetRecoveryKey());

    public Task<IAccountInformation> GetAccountInformationAsync()
    {
      return Task.Run<IAccountInformation>((Func<IAccountInformation>) (() => this.GetAccountInformation()));
    }

    public Task<IEnumerable<ISession>> GetSessionsHistoryAsync()
    {
      return Task.Run<IEnumerable<ISession>>((Func<IEnumerable<ISession>>) (() => this.GetSessionsHistory()));
    }

    public Task<IEnumerable<INode>> GetNodesAsync()
    {
      return Task.Run<IEnumerable<INode>>((Func<IEnumerable<INode>>) (() => this.GetNodes()));
    }

    public Task<IEnumerable<INode>> GetNodesAsync(INode parent)
    {
      return Task.Run<IEnumerable<INode>>((Func<IEnumerable<INode>>) (() => this.GetNodes(parent)));
    }

    public Task<INode> CreateFolderAsync(string name, INode parent)
    {
      return Task.Run<INode>((Func<INode>) (() => this.CreateFolder(name, parent)));
    }

    public Task DeleteAsync(INode node, bool moveToTrash = true)
    {
      return Task.Run((Action) (() => this.Delete(node, moveToTrash)));
    }

    public Task<INode> MoveAsync(INode sourceNode, INode destinationParentNode)
    {
      return Task.Run<INode>((Func<INode>) (() => this.Move(sourceNode, destinationParentNode)));
    }

    public Task<INode> RenameAsync(INode sourceNode, string newName)
    {
      return Task.Run<INode>((Func<INode>) (() => this.Rename(sourceNode, newName)));
    }

    public Task<Uri> GetDownloadLinkAsync(INode node)
    {
      return Task.Run<Uri>((Func<Uri>) (() => this.GetDownloadLink(node)));
    }

    public Task<Stream> DownloadAsync(
      INode node,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run<Stream>((Func<Stream>) (() => (Stream) new ProgressionStream(this.Download(node, cancellationToken), progress, this._options.ReportProgressChunkSize)), cancellationToken.GetValueOrDefault());
    }

    public Task<Stream> DownloadAsync(
      Uri uri,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run<Stream>((Func<Stream>) (() => (Stream) new ProgressionStream(this.Download(uri, cancellationToken), progress, this._options.ReportProgressChunkSize)), cancellationToken.GetValueOrDefault());
    }

    public Task DownloadFileAsync(
      INode node,
      string outputFile,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run((Action) (() =>
      {
        using (Stream stream = (Stream) new ProgressionStream(this.Download(node, cancellationToken), progress, this._options.ReportProgressChunkSize))
          this.SaveStream(stream, outputFile);
      }), cancellationToken.GetValueOrDefault());
    }

    public Task DownloadFileAsync(
      Uri uri,
      string outputFile,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run((Action) (() =>
      {
        if (string.IsNullOrEmpty(outputFile))
          throw new ArgumentNullException(nameof (outputFile));
        using (Stream stream = (Stream) new ProgressionStream(this.Download(uri, cancellationToken), progress, this._options.ReportProgressChunkSize))
          this.SaveStream(stream, outputFile);
      }), cancellationToken.GetValueOrDefault());
    }

    public Task<INode> UploadAsync(
      Stream stream,
      string name,
      INode parent,
      IProgress<double> progress = null,
      DateTime? modificationDate = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run<INode>((Func<INode>) (() =>
      {
        if (stream == null)
          throw new ArgumentNullException(nameof (stream));
        using (Stream stream1 = (Stream) new ProgressionStream(stream, progress, this._options.ReportProgressChunkSize))
          return this.Upload(stream1, name, parent, modificationDate, cancellationToken);
      }), cancellationToken.GetValueOrDefault());
    }

    public Task<INode> UploadFileAsync(
      string filename,
      INode parent,
      IProgress<double> progress = null,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run<INode>((Func<INode>) (() =>
      {
        DateTime lastWriteTime = File.GetLastWriteTime(filename);
        using (Stream stream = (Stream) new ProgressionStream((Stream) new FileStream(filename, FileMode.Open, FileAccess.Read), progress, this._options.ReportProgressChunkSize))
          return this.Upload(stream, Path.GetFileName(filename), parent, new DateTime?(lastWriteTime), cancellationToken);
      }), cancellationToken.GetValueOrDefault());
    }

    public Task<INode> GetNodeFromLinkAsync(Uri uri)
    {
      return Task.Run<INode>((Func<INode>) (() => this.GetNodeFromLink(uri)));
    }

    public Task<IEnumerable<INode>> GetNodesFromLinkAsync(Uri uri)
    {
      return Task.Run<IEnumerable<INode>>((Func<IEnumerable<INode>>) (() => this.GetNodesFromLink(uri)));
    }

    public Task<CG.Web.MegaApiClient.MegaApiClient.AuthInfos> GenerateAuthInfosAsync(
      string email,
      string password)
    {
      return Task.Run<CG.Web.MegaApiClient.MegaApiClient.AuthInfos>((Func<CG.Web.MegaApiClient.MegaApiClient.AuthInfos>) (() => this.GenerateAuthInfos(email, password, (string) null)));
    }

    public Task<CG.Web.MegaApiClient.MegaApiClient.AuthInfos> GenerateAuthInfosAsync(
      string email,
      string password,
      string mfaKey)
    {
      return Task.Run<CG.Web.MegaApiClient.MegaApiClient.AuthInfos>((Func<CG.Web.MegaApiClient.MegaApiClient.AuthInfos>) (() => this.GenerateAuthInfos(email, password, mfaKey)));
    }

    public Task<Stream> DownloadFileAttributeAsync(
      INode node,
      FileAttributeType fileAttributeType,
      CancellationToken? cancellationToken = null)
    {
      return Task.Run<Stream>((Func<Stream>) (() => this.DownloadFileAttribute(node, fileAttributeType, cancellationToken)));
    }

    public class AuthInfos
    {
      public AuthInfos(string email, string hash, byte[] passwordAesKey, string mfaKey = null)
      {
        this.Email = email;
        this.Hash = hash;
        this.PasswordAesKey = passwordAesKey;
        this.MFAKey = mfaKey;
      }

      [JsonProperty]
      public string Email { get; private set; }

      [JsonProperty]
      public string Hash { get; private set; }

      [JsonProperty]
      public byte[] PasswordAesKey { get; private set; }

      [JsonProperty]
      public string MFAKey { get; private set; }
    }

    public class LogonSessionToken : IEquatable<CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken>
    {
      [JsonProperty]
      public string SessionId { get; }

      [JsonProperty]
      public byte[] MasterKey { get; }

      private LogonSessionToken()
      {
      }

      public LogonSessionToken(string sessionId, byte[] masterKey)
      {
        this.SessionId = sessionId;
        this.MasterKey = masterKey;
      }

      public bool Equals(CG.Web.MegaApiClient.MegaApiClient.LogonSessionToken other)
      {
        return other != null && this.SessionId != null && other.SessionId != null && string.CompareOrdinal(this.SessionId, other.SessionId) == 0 && this.MasterKey != null && other.MasterKey != null && ((IEnumerable<byte>) this.MasterKey).SequenceEqual<byte>((IEnumerable<byte>) other.MasterKey);
      }
    }
  }
}
