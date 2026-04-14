using Domain.Entities;

namespace Application.Abstractions.Tokens;

public interface ITokenGenerator
{
  string GenerateAccessToken(string userId, IEnumerable<string> roles, TimeSpan tokenThreshold);
  RefreshToken GenerateRefreshToken(string credentialId, TimeSpan tokenThreshold);
}
