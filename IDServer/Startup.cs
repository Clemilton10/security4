using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IDServer
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddIdentityServer()
				.AddInMemoryClients(IdentityConfiguration.Clients)
				.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
				.AddInMemoryApiResources(IdentityConfiguration.ApiResources)
				.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
				.AddTestUsers(IdentityConfiguration.TestUsers)
				.AddDeveloperSigningCredential();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseIdentityServer();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Hello World!");
				});
			});
		}
	}
}
