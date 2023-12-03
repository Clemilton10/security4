class Program
{
	static async Task Main()
	{
		string tokenEndpoint = "http://localhost:5010/connect/token";
		string clientId = "cwm.client";
		string clientSecret = "secret";
		string scope = "myApi.read";

		var content = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "client_credentials"),
			new KeyValuePair<string, string>("scope", scope),
			new KeyValuePair<string, string>("client_id", clientId),
			new KeyValuePair<string, string>("client_secret", clientSecret),
		});

		using (var httpClient = new HttpClient())
		{
			var response = await httpClient.PostAsync(tokenEndpoint, content);
			if (response.IsSuccessStatusCode)
			{
				var accessToken = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Token de Acesso: {accessToken}");
			}
			else
			{
				Console.WriteLine($"Erro na solicitação: {response.StatusCode}");
			}
		}
	}
}