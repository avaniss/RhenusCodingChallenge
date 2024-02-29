namespace Application.Interfaces;
public interface ISecurityTokenGenerator
{
    public (string, DateTime) GenerateToken(IDictionary<string, string> claims);
}
