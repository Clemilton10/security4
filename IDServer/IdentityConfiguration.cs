using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IDServer
{
	public class IdentityConfiguration
	{
		// Usuários de teste
		public static List<TestUser> TestUsers =>
			new List<TestUser>
			{
				new TestUser
				{
					SubjectId = "1144",
					Username = "mukesh",
					Password = "mukesh",
					Claims =
					{
						new Claim(JwtClaimTypes.Name, "Mukesh Murugan"),
						new Claim(JwtClaimTypes.GivenName, "Mukesh"),
						new Claim(JwtClaimTypes.FamilyName, "Murugan"),
						new Claim(JwtClaimTypes.WebSite, "http://codewithmukesh.com"),
					}
				}
			};

		// Recursos de identidade
		public static IEnumerable<IdentityResource> IdentityResources =>
			new IdentityResource[]
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			};

		// Escopos de API
		public static IEnumerable<ApiScope> ApiScopes =>
			new ApiScope[]
			{
				new ApiScope("myApi.read"),
				new ApiScope("myApi.write"),
			};

		// Recursos da API
		public static IEnumerable<ApiResource> ApiResources =>
			new ApiResource[]
			{
				new ApiResource("myApi")
				{
					Scopes = new List<string>{ "myApi.read","myApi.write" },
					ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
				}
			};

		// Clientes
		public static IEnumerable<Client> Clients =>
			new Client[]
			{
				new Client
				{
					ClientId = "cwm.client",
					ClientName = "Client Credentials Client",
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					ClientSecrets = { new Secret("secret".Sha256()) },
					AllowedScopes = { "myApi.read" }
				},
			};
	}
}