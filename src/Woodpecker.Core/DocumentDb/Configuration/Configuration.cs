namespace Woodpecker.Core.DocumentDb.Configuration
{
    public class Configuration : IConfiguration
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResourceId { get; set; }
    }
}
