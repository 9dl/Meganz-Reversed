# Mega.nz C# SDK Reversed
- Translate this into any desired language to create a rate limit free Mega.nz API.
- Please use responsibly and do not abuse this functionality.
  
# API
```cs
    private static readonly Uri s_baseApiUri = new Uri("https://g.api.mega.co.nz/cs");
    private static readonly Uri s_baseUri = new Uri("https://mega.nz");
```

# Login
```cs
    /// <summary>
    /// Login to Mega.co.nz service using email/password credentials
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="password">password</param>
    /// <exception cref="ApiException">Service is not available or credentials are invalid</exception>
    /// <exception cref="ArgumentNullException">email or password is null</exception>
    /// <exception cref="NotSupportedException">Already logged in</exception>
    public LogonSessionToken Login(string email, string password)
    {
      return Login(GenerateAuthInfos(email, password));
    }
```

# GenerateAuthInfos
```cs
    public AuthInfos GenerateAuthInfos(string email, string password, string mfaKey = null)
    {
      if (string.IsNullOrEmpty(email))
      {
        throw new ArgumentNullException("email");
      }

      if (string.IsNullOrEmpty(password))
      {
        throw new ArgumentNullException("password");
      }

      // Prelogin to retrieve account version
      var preLoginRequest = new PreLoginRequest(email);
      var preLoginResponse = Request<PreLoginResponse>(preLoginRequest);

      if (preLoginResponse.Version == 2 && !string.IsNullOrEmpty(preLoginResponse.Salt))
      {
        // Mega uses a new way to hash password based on a salt sent by Mega during prelogin
        var saltBytes = preLoginResponse.Salt.FromBase64();
        var passwordBytes = password.ToBytesPassword();
        const int Iterations = 100000;

        var derivedKeyBytes = new byte[32];
        using (var hmac = new HMACSHA512())
        {
          var pbkdf2 = new Pbkdf2(hmac, passwordBytes, saltBytes, Iterations);
          derivedKeyBytes = pbkdf2.GetBytes(derivedKeyBytes.Length);
        }

        // Derived key contains master key (0-16) and password hash (16-32)
        if (!string.IsNullOrEmpty(mfaKey))
        {
          return new AuthInfos(
            email,
            derivedKeyBytes.Skip(16).ToArray().ToBase64(),
            derivedKeyBytes.Take(16).ToArray(),
            mfaKey);
        }

        return new AuthInfos(
          email,
          derivedKeyBytes.Skip(16).ToArray().ToBase64(),
          derivedKeyBytes.Take(16).ToArray());
      }
      else if (preLoginResponse.Version == 1)
      {
        // Retrieve password as UTF8 byte array
        var passwordBytes = password.ToBytesPassword();

        // Encrypt password to use password as key for the hash
        var passwordAesKey = PrepareKey(passwordBytes);

        // Hash email and password to decrypt master key on Mega servers
        var hash = GenerateHash(email.ToLowerInvariant(), passwordAesKey);
        if (!string.IsNullOrEmpty(mfaKey))
        {
          return new AuthInfos(email, hash, passwordAesKey, mfaKey);
        }

        return new AuthInfos(email, hash, passwordAesKey);
      }
      else
      {
        throw new NotSupportedException("Version of account not supported");
      }
    }
```

## PrepareKey, AuthInfos, PreLoginRequest can easily be found in the SDK 
