using IdentityModel.Client;
using System.Threading.Tasks;

namespace WebClient.Services
{
	public interface ITokenService
	{
		Task<TokenResponse> GetToken(string scope);
	}
}