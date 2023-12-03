class Program
{
	static async Task Main()
	{
		string apiUrl = "http://localhost:5020/weatherforecast";
		using (var httpClient = new HttpClient())
		{
			var response = await httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Resposta: {content}");
			}
			else
			{
				Console.WriteLine($"Erro na solicitação: {response.StatusCode}");
			}
		}
	}
}