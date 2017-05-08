namespace Woodpecker.Core.DocumentDb.Configuration
{
    public interface IConfiguration
    {
        string TenantId { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string ResourceId { get; set; }
    }
}
