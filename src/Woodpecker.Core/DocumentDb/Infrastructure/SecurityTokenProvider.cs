using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;


namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class SecurityTokenProvider : ISecurityTokenProvider
    {
        private readonly AuthenticationContext authenticationContext;
        private readonly ClientCredential clientCredential;

        public SecurityTokenProvider(string tenantId, string clientId, string clientSecret)
        {
            this.authenticationContext = new AuthenticationContext(tenantId);
            this.clientCredential = new ClientCredential(clientId, clientSecret);
        }
        public async Task<string> GetSecurityTokenAsync(string resourceId)
        {
            try
            {
                var result = await this.authenticationContext.AcquireTokenAsync(resourceId, clientCredential).ConfigureAwait(false);
                return result.AccessToken;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}