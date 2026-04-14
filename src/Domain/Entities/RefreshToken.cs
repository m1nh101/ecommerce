namespace Domain.Entities;

public sealed class RefreshToken
{
  public string Token { get; private set; } = string.Empty;
  public string CredentialId { get; private set; } = string.Empty;
  public DateTime ExpiryTime { get; private set; }
  public bool IsRevoked { get; private set; }
  public string? ReplacedByToken { get; private set; }
  public DateTime CreatedAt { get; private set; }

  public bool IsExpired => DateTime.UtcNow >= ExpiryTime;

  public static RefreshToken Create(string token, string credentialId, DateTime expiryTime)
  {
    return new RefreshToken
    {
      Token = token,
      CredentialId = credentialId,
      ExpiryTime = expiryTime,
      CreatedAt = DateTime.UtcNow
    };
  }

  public void Revoke(string? replacedByToken = null)
  {
    IsRevoked = true;
    ReplacedByToken = replacedByToken;
  }
}
