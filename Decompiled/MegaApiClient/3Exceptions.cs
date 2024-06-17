// Decompiled with JetBrains decompiler
// Type: CG.Web.MegaApiClient.ApiResultCode
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

#nullable disable
namespace CG.Web.MegaApiClient
{
  public enum ApiResultCode
  {
    /// <summary>Login requires Two-Factor Authentication</summary>
    TwoFactorAuthenticationError = -26, // 0xFFFFFFE6
    /// <summary>
    /// API_EAPPKEY (-22): Invalid application key; request not processed
    /// </summary>
    InvalidOrMissingApplicationKey = -22, // 0xFFFFFFEA
    /// <summary>API_EREAD (-21): Read failed</summary>
    FileCouldNotBeReadFrom = -21, // 0xFFFFFFEB
    /// <summary>API_EWRITE (-20): Write failed</summary>
    FileCouldNotBeWrittenTo = -20, // 0xFFFFFFEC
    /// <summary>
    /// API_ETOOMANYCONNECTIONS (-19): Too many connections on this resource
    /// </summary>
    TooManyConnectionsOnThisResource = -19, // 0xFFFFFFED
    /// <summary>
    /// API_ETEMPUNAVAIL (-18): Resource temporarily not available, please try again later
    /// </summary>
    ResourceTemporarilyNotAvailable = -18, // 0xFFFFFFEE
    /// <summary>API_EOVERQUOTA (-17): Request over quota</summary>
    QuotaExceeded = -17, // 0xFFFFFFEF
    /// <summary>API_EBLOCKED (-16): User blocked</summary>
    ResourceAdministrativelyBlocked = -16, // 0xFFFFFFF0
    /// <summary>
    /// API_ESID (-15): Invalid or expired user session, please relogin
    /// </summary>
    BadSessionId = -15, // 0xFFFFFFF1
    /// <summary>
    /// API_EKEY (-14): A decryption operation failed (never returned by the API)
    /// </summary>
    CryptographicError = -14, // 0xFFFFFFF2
    /// <summary>
    /// API_EINCOMPLETE (-13): Trying to access an incomplete resource
    /// </summary>
    RequestIncomplete = -13, // 0xFFFFFFF3
    /// <summary>
    /// API_EEXIST (-12): Trying to create an object that already exists
    /// </summary>
    ResourceAlreadyExists = -12, // 0xFFFFFFF4
    /// <summary>
    /// API_EACCESS (-11): Access violation (e.g., trying to write to a read-only share)
    /// </summary>
    AccessDenied = -11, // 0xFFFFFFF5
    /// <summary>API_ECIRCULAR (-10): Circular linkage attempted</summary>
    CircularLinkage = -10, // 0xFFFFFFF6
    /// <summary>
    /// API_EOENT (-9): Object (typically, node or user) not found
    /// </summary>
    ResourceNotExists = -9, // 0xFFFFFFF7
    /// <summary>
    /// API_EEXPIRED (-8): The upload target URL you are trying to access has expired. Please request a fresh one.
    /// </summary>
    ResourceExpired = -8, // 0xFFFFFFF8
    /// <summary>
    /// API_ERANGE (-7): The upload file packet is out of range or not starting and ending on a chunk boundary.
    /// </summary>
    ResourceAccessOutOfRange = -7, // 0xFFFFFFF9
    /// <summary>
    /// API_ETOOMANY (-6): Too many concurrent IP addresses are accessing this upload target URL.
    /// </summary>
    ToManyRequestsForThisResource = -6, // 0xFFFFFFFA
    /// <summary>
    /// API_EFAILED (-5): The upload failed. Please restart it from scratch.
    /// </summary>
    RequestFailedPermanetly = -5, // 0xFFFFFFFB
    /// <summary>
    /// API_ERATELIMIT (-4): You have exceeded your command weight per time quota. Please wait a few seconds, then try again (this should never happen in sane real-life applications).
    /// </summary>
    TooManyRequests = -4, // 0xFFFFFFFC
    /// <summary>
    /// API_EAGAIN (-3) (always at the request level): A temporary congestion or server malfunction prevented your request from being processed. No data was altered. Retry. Retries must be spaced with exponential backoff.
    /// </summary>
    RequestFailedRetry = -3, // 0xFFFFFFFD
    /// <summary>
    /// API_EARGS (-2): You have passed invalid arguments to this command.
    /// </summary>
    BadArguments = -2, // 0xFFFFFFFE
    /// <summary>
    /// API_EINTERNAL (-1): An internal error has occurred. Please submit a bug report, detailing the exact circumstances in which this error occurred.
    /// </summary>
    InternalError = -1, // 0xFFFFFFFF
    /// <summary>API_OK (0): Success</summary>
    Ok = 0,
  }
}
