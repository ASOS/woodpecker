using System.Threading.Tasks;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public interface ISecurityTokenProvider
    {
        Task<string> GetSecurityTokenAsync();
    }
}