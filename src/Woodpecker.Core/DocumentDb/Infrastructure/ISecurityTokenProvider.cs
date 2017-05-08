using System.Threading.Tasks;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface ISecurityTokenProvider
    {
        Task<string> GetSecurityTokenAsync();
    }
}