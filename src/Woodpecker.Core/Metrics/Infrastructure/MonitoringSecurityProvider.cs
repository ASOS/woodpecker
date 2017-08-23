using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public class MonitoringSecurityProvider : ISecurityTokenProvider
    {
        private const string AccessTokenResource = "https://management.core.windows.net/";
        private const string AuthorityFormat = "https://login.windows.net/{0}";

        private readonly AuthenticationContext authenticationContext;
        private readonly ClientCredential clientCredential;

        public MonitoringSecurityProvider(string tenantId, string clientId, string clientSecret)
        {
            var authority = string.Format(AuthorityFormat, tenantId);
            this.authenticationContext = new AuthenticationContext(authority);
            this.clientCredential = new ClientCredential(clientId, clientSecret);
        }
        public async Task<string> GetSecurityTokenAsync()
        {
            try
            {
                var result = await this.authenticationContext.AcquireTokenAsync(AccessTokenResource, clientCredential).ConfigureAwait(false);
                return result.AccessToken;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}