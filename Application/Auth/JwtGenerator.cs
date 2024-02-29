using Application.Interfaces;
using FastEndpoints.Security;

namespace Application.Auth;
public class JwtGenerator : ISecurityTokenGenerator
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _validityInHours;

    public JwtGenerator(string key, string issuer, string audience, int validityInHours)
    {
        this._key = key;
        this._issuer = issuer;
        this._audience = audience;
        this._validityInHours = validityInHours;
    }

    public (string, DateTime) GenerateToken(IDictionary<string, string> claims)
    {
        (string claimType, string claimValue)[] claimSet = claims.Select(c => (c.Key, c.Value)).ToArray();
        var expiry = DateTime.UtcNow.AddHours(this._validityInHours);
        return (JWTBearer.CreateToken(
                signingKey: this._key,
                issuer: this._issuer,
                audience: this._audience,
                expireAt: expiry,
                claims: claimSet), expiry);
    }
}
