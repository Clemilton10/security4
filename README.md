[Home](./README.md) | [VS Code Snippets](./snippets.md)

## IdentityServer4 no ASP.NET Core – Guia definitivo para iniciantes

[https://codewithmukesh.com/blog/identityserver4-in-aspnet-core/](https://codewithmukesh.com/blog/identityserver4-in-aspnet-core/)

Neste artigo, começaremos a aprender sobre o IdentityServer4 no ASP.NET Core e maneiras de integrá-lo para criar soluções seguras. Estaremos criando uma Solução de Trabalho do zero, levando você através de vários conceitos e implementações deste incrível OpenID Framework. Esta é a Parte 1 do IdentityServer4 na série Core ASP.NET. Você pode encontrar o código-fonte completo da implementação [aqui](https://github.com/iammukeshm/GettingStartedWithIdentityServer4.ASPNETCore).

## O que é o IdentityServer4?

[IdentityServer4](https://duendesoftware.com/) é um OpenID Connect e OAuth 2.0 GRATUITO e Open Source Framework para ASP.NET Core. Em outras palavras, é um Provedor de Autenticação para suas Soluções. É uma estrutura que é construída sobre o OpenID Connect e OAuth 2.0 para ASP.NET Core. A ideia principal é centralizar o provedor de autenticação. Digamos que você tenha 5 APIS / Microsserviços. Você realmente não precisa definir as lógicas de autenticação em cada aplicativo. Em vez disso, com o IdentityServer4, você pode centralizar o Controle de Acesso para que todas as APIs sejam protegidas pelo Central IdentityServer.

Outro recurso interessante é quando um cliente (Aplicativo Web) deseja acessar uma API segura, o IdentityServer4 gera tokens de acesso perfeitamente para tornar isso possível. Falaremos sobre isso mais adiante no artigo.

## Conceito do Identity Server

A ideia é bem simples e direta. Os usuários usam os clientes (digamos ASP.NET MVC Core) para acessar os dados. Os usuários serão autenticados pelo IdentityServer para usar o cliente. Depois que os usuários são autenticados para usar o Cliente, o cliente envia uma solicitação para o Recurso de API. Lembre-se de que os Recursos de Cliente e API são protegidos por uma única entidade, o IdentityServer. O cliente solicita um token de acesso com o qual ele pode acessar as respostas da API. Desta forma, estamos centralizando o Mecanismo de Autenticação em um único servidor. Bem interessante, sim?

Aqui está um fluxo conforme descrito pela documentação do IdentityServer.

![alt](./imgs/001.webp)

## Responsabilidades do IdentityServer4

O Identity Server é uma solução de segurança tudo em um para seus projetos. Aqui estão suas principais características e responsabilidades.

- Proteja seus recursos
- Autenticar usuários usando um armazenamento de conta local ou por meio de um provedor de identidade externo
- Fornecer gerenciamento de sessão e logon único
- gerenciar e autenticar clientes
- Emitir tokens de identidade e acesso para clientes
- validar tokens

## Modelos do IdentityServer4

Há algumas maneiras de ativar projetos IdentityServer4. O mais usado é o Templates. Esta é mais uma solução de início rápido em que você instala os modelos do IdentityServer4 usando sua CLI e seleciona um modelo que cria automaticamente um projeto implementado para você.

> PS – NÃO usaremos essa abordagem em nosso artigo, pois ela esconde a maior parte da complexidade e você acaba não sabendo o que de fato acontece nos bastidores. Vamos implementar o servidor do zero. Uma vez que você está familiarizado com o que está funcionando, você está pronto para usar esses modelos.

Abra o Powershell / Prompt de Comando em um diretório de trabalho e execute o seguinte comando que instala os modelos do IdentityServer4 globalmente para você.

```sh
# Modelos do IdentityServer4
dotnet new -i identityserver4.templates
```

![alt](./imgs/002.webp)

Você pode ver os modelos IdentityServer4 instalados. Agora, para criar um novo projeto com base em um modelo, execute o seguinte.

```sh
dotnet new is4inmem --force
```

Isso cria uma implementação de IdentityServer4 no ASP.NET Core usando usuário na memória e configurações. Mas haverá muito código que não precisaremos / entenderemos para o nosso propósito de aprendizagem. Assim, vamos criar tudo do zero para que entendamos cada parte da implementação do IdentityServer4.

## O que iremos Construir?

1. Criar um projeto de host IdentityServer4 com usuários na memória e armazenamentos (para fins de teste)
2. Criar uma API ASP.NET Core (Este é o recurso a ser protegido pelo IdentityServer4)
3. Criar um cliente Web que consome o ponto de acesso

## Introdução ao IdentityServer4 no ASP.NET Core

Vamos começar criando uma Solução em Branco na [Comunidade do Visual Studio 2019](https://codewithmukesh.com/blog/install-visual-studio-2019-community/).

```sh
# Solução em Branco
dotnet new sln -n security
```

![alt](./imgs/003.webp)

Agora, na solução em branco, adicione um novo ASP.NET Core Empty Project. Verifique se você selecionou o Modelo Vazio. Este é o projeto que hospedará o IdentityServer real.

```sh
# ASP.NET Core Empty Project
dotnet new web -n IDServer -f net3.1
dotnet sln add IDServer
```

📄 Properties/launchSettings.json

```json
{
  "profiles": {
    "IDServer": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5010",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

![alt](./imgs/004.webp)

![alt](./imgs/005.webp)

## Instalando o pacote IdentityServer4 em ASP.NET projeto principal

Para o projeto recém-criado, vamos instalar o pacote IdentityServer4. Execute o seguinte comando no Console do Gerenciador de Pacotes.

```powershell
cd IDServer
Install-Package IdentityServer4 -Version 4.1.1
```

```sh
cd IDServer
dotnet add package IdentityServer4 --version 4.1.1
```

## Adicionando configuração na memória

Adicionaremos toda a Configuração em nosso código para fins de demonstração. Observe que esse não será o caso quando você integrar o IdentityServer4 na produção. Esta é uma maneira mais fácil de entender cada componente. Para a raiz do projeto IdentityServer, adicione uma nova classe e nomeie-a IdentityConfiguration.cs

📄 IdentityConfiguration.cs

```csharp
public class IdentityConfiguration
{
}
```

## Usuários de teste

Vamos adicionar um usuário de teste ao nosso arquivo de configuração. Para fins de demonstração, definiremos os dados do usuário no código. Em outro artigo, aprenderemos como integrar o Entity Framework e o ASP.NET Core Identity para gerenciar usuários em um banco de dados. Mas, por enquanto, vamos manter as coisas simples e entender os contextos.

Adicione o seguinte à classe IdentityConfiguration. Esse trecho retorna um TestUser com algumas declarações JWT específicas.

📄 IdentityConfiguration.cs

```csharp
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
```

## Recursos de identidade

Recursos de identidade são dados como userId, e-mail, um número de telefone que é algo exclusivo de uma identidade/usuário específico. No trecho abaixo, adicionaremos o OpenId e os Recursos de Perfil. Copie esse código para sua classe IdentityConfiguration.

📄 IdentityConfiguration.cs

```csharp
public static IEnumerable<IdentityResource> IdentityResources =>
new IdentityResource[]
{
	new IdentityResources.OpenId(),
	new IdentityResources.Profile(),
};
```

## Escopos de API

Como mencionado anteriormente, nossa principal intenção é proteger uma API (que ainda não construímos). Então, essa API pode ter escopos. Escopos no contexto de, o que o usuário autorizado pode fazer. Por exemplo, podemos ter 2 escopos por enquanto – Leitura, Gravação. Vamos nomear nossa API como myAPI. Copie o código abaixo para `IdentityConfiguration.cs`

📄 IdentityConfiguration.cs

```csharp
public static IEnumerable<ApiScope> ApiScopes =>
new ApiScope[]
{
	new ApiScope("myApi.read"),
	new ApiScope("myApi.write"),
};
```

## Recursos da API

Agora, vamos definir a API em si. Vamos dar-lhe um nome myApi e mencionar os escopos suportados também, juntamente com o segredo. Certifique-se de fazer hash deste código secreto. Esse código com hash será salvo internamente no IdentityServer.

📄 IdentityConfiguration.cs

```csharp
public static IEnumerable<ApiResource> ApiResources =>
new ApiResource[]
{
	new ApiResource("myApi")
	{
		Scopes = new List<string>{ "myApi.read","myApi.write" },
		ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
	}
};
```

## Clientes

Finalmente, temos que definir a quem será concedido acesso ao nosso recurso protegido que, no nosso caso, é `myApi`. Forneça um nome de cliente e ID apropriados. Aqui estamos definindo o GrantType como ClientCredentials.

📄 IdentityConfiguration.cs

```csharp
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
```

## Registrando o IdentityServer4 no ASP.NET Core

Vamos registrar o IdentityServer4 em ASP.NET Core DI Container. Abra Startup.cs e adicione o seguinte ao método ConfigureServices. Aqui estaremos usando todos os recursos estáticos, clientes e usuários que definimos em nossa classe IdentityConfiguration.

📄 Startup.cs

```csharp
services
	.AddIdentityServer()
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddTestUsers(IdentityConfiguration.TestUsers)
    .AddDeveloperSigningCredential();
```

## Armazenamentos de configuração na memória

Como mencionado anteriormente, faremos um código rígido das configurações do Identity Server para manter as coisas simples de entender. Há alguns armazenamentos na memória a serem configurados. Essas configurações são codificadas no projeto HOST e são carregadas apenas uma vez quando o aplicativo é iniciado. Isso é usado principalmente para as fases de desenvolvimento e prototipagem. Dizendo que essa abordagem também pode ser válida para cenários de produção se a configuração raramente mudar com o tempo,

## Assinando credenciais

Basicamente, o IdentityServer precisa de certificados para verificar seu uso. Mas, novamente, para fins de desenvolvimento e como não temos nenhum certificado conosco, usamos a extensão AddDeveloperSigningCredential(). [Você pode ler mais sobre isso aqui](#).

Finalmente, no método Configure, adicione a seguinte linha para adicionar o IdentityServer Middleware.

📄 Startup.cs

```csharp
app.UseRouting();
app.UseIdentityServer();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});
```

## Executando o IdentityServer4

Depois de configurar o IdentityServer4, vamos compilá-lo e executá-lo.

> Certifique-se de anotar a postagem na qual seu IdentityServer é executado. Para mim é 5010. Você pode definir que é uma porta personalizada modificando o launchsettings.json encontrado na pasta Properties do seu projeto ASP.NET Core.

## Documento de descoberta do OpenID

O documento de descoberta do OpenID Connect está disponível para todos os provedores OpenID em `/.well-known/openid-configuration`. Este documento contém a definição do seu IdentityServer, como o ponto de extremidade do token (o ponto de extremidade para o qual você POSTA para recuperar tokens de acesso), escopos com suporte, a URL do IdentityServer em execução e assim por diante.

Para saber mais sobre essa padronização, [leia aqui](https://openid.net/specs/openid-connect-discovery-1_0.html).

```sh
# Verifique a porta em 📄 appsettings.json
# "sslPort": 5010
http://localhost:5010/.well-known/openid-configuration
```

![alt](./imgs/006.webp)

## Buscando Tokens de Acesso com o POSTMAN

No Documento de Descoberta, você pode saber sobre o ponto de extremidade configurado para recuperar o token de acesso. Abra o `POSTMAN` e envie uma solicitação `POST` para o ponto de extremidade do token de acesso. Certifique-se de que tem os parâmetros abaixo no corpo do pedido.

```console
POST: http://localhost:5010/connect/token
	Body
		x-www-form-urlencoded
			grant_type		client_credentials
			scope			myApi.read
			client_id		cwm.client
			client_secret	secret
```

![alt](./imgs/007.webp)

Uma vez autorizado com êxito, IdentityServer4 retorna você com um token de acesso que é válido por 3600 segundos ou 1 hora.

Observe que passamos parâmetros como grant_type, escopo do uso, ID do cliente e segredo.

## Entendendo o Token

Agora que temos um token de acesso válido. vamos até [jwt.io](https://jwt.io/) decodificar o Access Token. Como outro ponto, qualquer JWTokens pode ser decodificado, portanto, certifique-se de nunca adicionar dados confidenciais como senha, etc em seus tokens.

![alt](./imgs/008.webp)

Você pode ver que todos os dados que definimos estão disponíveis em nosso Access Token. O conceito, como mencionamos anteriormente, é que usaremos esse token para acessar a API protegida pelo Identity Server.

## Protegendo uma WebAPI do ASP.NET Core com o IdentityServer4

Nesta seção, aprenderemos como proteger uma WebAPI do ASP.NET Core com o IdentityServer4 e acessar um endpoint protegido usando um token de acesso.

Adicione um novo projeto WebAPI do ASP.NET Core à solução. Observe que, idealmente, teríamos que manter o Servidor de Identidade em uma Solução separada. Mas, para fins de demonstração, vamos juntar tudo em uma única solução.

```sh
# WebAPI do ASP.NET Core
dotnet new webapi -n WebApi -f net3.1
```

📄 Properties/launchSettings.json

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "WebApi": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "weatherforecast",
      "applicationUrl": "http://localhost:5020",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Além disso, verifique se enquanto você executa a solução, o primeiro projeto a ser executado é o IdentityServer4. Para habilitar projetos de inicialização múltipla, clique com o botão direito do mouse na solução e clique em propriedades.

![alt](./imgs/009.webp)

Vamos executar a Solução. No navegador da WebAPI, navegue até /weatherforecast. Este é o controlador padrão que acompanha o ASP.NET Core. Usaremos esse Endpoint e o protegeremos com o IdentityServer4.

![alt](./imgs/010.webp)

Agora, volte para o WebAPI Project e instale o seguinte pacote via Console do Gerenciador de Pacotes.

```powershell
Install-Package IdentityServer4.AccessTokenValidation -Version 3.0.1
```

```sh
dotnet add package IdentityServer4.AccessTokenValidation --version 3.0.1
```

Vamos começar a adicionar o Middleware de Autenticação ao Pipeline. Abra Startup.cs do projeto WebAPI e adicione o seguinte ao método ConfigureService.

📄 Startup.cs

```csharp
services
	.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.ApiName = "myApi";
        options.Authority = "http://localhost:5010";
		// Permite sem https
		options.RequireHttpsMetadata = false;
    });
```

A linha 4 determina o nome do recurso WebAPI. Lembra que já tínhamos definido esse nome na Configuração do Projeto do Servidor?
Linha 5 Sugere a URL na qual o IdentityServer está ativo e em execução. É importante EXECUTAR o IdentityServer primeiro e, em seguida, o projeto WebAPI seguido por um cliente, se houver. (Adicionaremos um projeto Web cliente mais adiante neste artigo)

Finalmente, no método Config, adicione o seguinte. <b>Certifique-se de que a ordem em que os Middleware são definidos é a mesma,</b>

📄 Startup.cs

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Agora vá para o WeatherController padrão e adicione um atributo Authorize ao controlador. Dessa forma, garantimos nosso WebAPI Endpoint.

📄 Controllers/WeatherForecastController.cs

```csharp
[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherForecastController : ControllerBase
```

## Buscando o Token

Abra o `POSTMAN` e envie uma solicitação `GET` para o ponto de extremidade da previsão do tempo. O ideal é que você esteja recebendo um erro não autorizado 401.

![alt](./imgs/011.webp)

Envie uma solicitação GET para o ponto de extremidade do token IdentityServer com parâmetros válidos. Isso obtém um Access Token. Lembra que fizemos isso antes? Agora, usaremos esse token para acessar o Controlador de API Seguro.

## Acessando a API com o Access Token

Novamente, envie uma solicitação `GET` para o ponto de extremidade weatherforecast, mas desta vez, com um cabeçalho de autorização adicional. No `POSTMAN`, alterne para a guia Autorização e selecione o Token de Portador no menu suspenso e cole o Token de acesso que você recebeu do IdentityServer4. Agora clique em Enviar. <b>200 OK</b>

```console
GET: http://localhost:5020/weatherforecast
	Authorization
		Bearer Token
			Token	eyJhbGciOiJSUzI1NiIsImtpZCI6IkRDNjhCODQ2Mzk4M0NGNkJFQkJGMTI4MzAwNjRGNzRGIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3MDE2MzQxMDAsImV4cCI6MTcwMTYzNzcwMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDEwIiwiYXVkIjoibXlBcGkiLCJjbGllbnRfaWQiOiJjd20uY2xpZW50IiwianRpIjoiNDFFOUE3RDE1RjM0QzE0NjJDNTY2RjA3OUMxQTZDMTkiLCJpYXQiOjE3MDE2MzQxMDAsInNjb3BlIjpbIm15QXBpLnJlYWQiXX0.azDZTwzPHezxT3KlNXdtJVxvt0thIXFNi2_QdXqzNtmlRJuITFs27xb5fV5Mmc1bsWpFJ2ou6aRTEXX_2-AqDyU9w64iE4drY7j6N8QaD77CDE_-S7cn1iQnflWXUsfcUnsD9IHro57aouSbKpnwsq9oePbJaSBOeyrvXCVrAwYG-ZJ4IUrCCV9Q-ndHuSeBWXSEXJdu8eQfY7J_f3vY-ze8L80dmBtEjNmccE65wpFkb6iPUf73MyxN949akv4_PJDufjeRzm7Z3RlE0I-VDDykQieS_pq-GCNX2hK1kb0qnUC6eZ22lj8Gd6N55qNXnU7FUFfmTQgFDZnVfJ1zsw
```

![alt](./imgs/012.webp)

Assim, nossa API é protegida usando o IdentityServer4. Agora, vamos parar de usar o POSTMAN como nosso cliente. Em vez disso, vamos apresentar um Projeto Web como o Cliente que tentará acessar nosso Controlador de API seguro. Este será o caso de uso mais comum do IdentityServer4. Vamos ver como conseguir isso.

Criando um cliente Web para acessar a API segura
Em primeiro lugar, crie um novo projeto em nossa solução e nomeie-o WebClient. <b>Usaremos um projeto MVC sem autenticação.</b>

```sh
# ASP.NET Core web Application (mvc)
dotnet new webapp -n WebClient -f net3.1
```

📄 Properties/launchSettings.json

```json
{
  "profiles": {
    "WebClient": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5030",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

![alt](./imgs/013.webp)

Primeiro, instale o pacote a seguir.

```powershell
Install-Package IdentityModel -Version 4.4.0
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design - Version 3.1.4
```

```sh
dotnet add package IdentityModel --version 4.4.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 3.1.4
```

Em seguida, precisamos de um serviço que possa conversar com o IdentityServer4 e solicitar um token de acesso com o qual o Projeto MVC possa acessar os dados da API. Entendeu?

No projeto WebClient. adicione uma nova pasta e nomeie-a como Serviços. aqui vamos adicionar a interface TokenService e implementação. Observe que o parâmetro de entrada será uma cadeia de caracteres de conteúdo de escopo.

📄 Services/ITokenService.cs

```csharp
public interface ITokenService
{
    Task<TokenResponse> GetToken(string scope);
}
```

📄 Services/TokenService.cs

```csharp
public class TokenService : ITokenService
{
    private DiscoveryDocumentResponse _discDocument {get;set;}
    public TokenService()
    {
        using(var client = new HttpClient())
        {
            _discDocument = client.GetDiscoveryDocumentAsync("http://localhost:5010/.well-known/openid-configuration").Result;
        }
    }
    public async Task<TokenResponse> GetToken(string scope)
    {
        using (var client = new HttpClient())
        {
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discDocument.TokenEndpoint,
                ClientId = "cwm.client",
                Scope = scope,
                ClientSecret = "secret"
            });
            if(tokenResponse.IsError)
            {
                throw new Exception("Token Error");
            }
            return tokenResponse;
        }
    }
}
```

Linha 3, Aqui está a classe DiscoveryDocumentReponse que vem com o pacote que instalamos anteriormente.
Linha 4 a 10, no construtor, usamos o HTTPClient para obter os dados do documento do ponto de extremidade de configuração OpenID do IdentityServer. Observe que estamos codificando as URLs aqui. <b>Idealmente, teremos que defini-los em appsettings.json e usar o padrão IOptions para recuperá-los em tempo de execução.</b>

Lembra que adicionamos algum cliente à nossa configuração do IdentityServer4? Vamos usar esses dados aqui. Linha 17 a 20, definimos o endereço, clientId, Escopo e Segredo do Cliente.

Agora, estamos esperando dados meteorológicos de nossa API. Assim, vamos criar uma nova classe Model para acomodar os dados. No projeto WebClient. adicione uma nova classe na pasta Models e nomeie-a como WeatherModel. Adicione o seguinte trecho.

📄 Models/WeatherModel.cs

```csharp
public class WeatherModel
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; set; }
}
```

Em seguida, no HomeController adicione um novo Método que basicamente conversará com a API Secured e obterá dados dela. No alto nível, o que essa ação do controlador fará é o seguinte:

1. Use o Serviço de Token, fale com o IdentityServer4 e recupere um token de acesso válido.
2. Defina o token de acesso para o cabeçalho JWT do HttpClient.
3. Use o Cliente Http e fale com a API Segura para obter os dados meteorológicos. Como estamos adicionando o JWT Token, não devemos ter nenhum problema em autenticar o WebClient para usar a WebAPI, certo?

Adicione o seguinte método de ação.

📄 Controllers/HomeController.cs

```csharp
public async Task<IActionResult> Weather()
{
    var data = new List<WeatherModel>();
    var token = await _tokenService.GetToken("myApi.read");
    using (var client = new HttpClient())
    {
        client.SetBearerToken(token.AccessToken);
        var result = await client.GetAsync("https://localhost:44367/weatherforecast");
        if(result.IsSuccessStatusCode)
        {
            var model = await result.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject<List<WeatherModel>>(model);
            return View(data);
        }
        else
        {
            throw new Exception("Failed to get Data from API");
        }
    }
}
```

Em seguida, adicione um novo modo de exibição para o Método Meteorológico. Será uma exibição simples onde haverá uma tabela que exibe uma lista de dados do WeatherModel. Esses dados serão passados para a Exibição pelo Controlador.

📄 Views/Home/Weather.cshtml

```cshtml
@model List<WeatherModel>
@{
    ViewData["Title"] = "Weather";
}
<h1>Weather</h1>
<table class="table table-striped">
    @foreach (var weather in Model)
    {
        <tr>
            <td>@weather.Date</td>
            <td>@weather.Summary</td>
            <td>@weather.TemperatureC</td>
            <td>@weather.TemperatureF</td>
        </tr>
    }
</table>
```

Finalmente, no .cs de inicialização do projeto WebClient, adicione o seguinte no método ConfigureServices para registrar o TokenService no contêiner de DI do ASP.NET Core.

```csharp
services.AddSingleton<ITokenService, TokenService>();
```

Isso é sobre tudo o que você tem que fazer para autorizar seu cliente. Agora, compile e execute todos os 3 projetos na seguinte ordem -> IdentityServer, WebAPI e, finalmente, o WebClient. No navegador do WebClient, navegue até ./home/weather. Se tudo correr bem, você verá os dados reais da nossa WebAPI Segura.

![alt](./imgs/014.webp)

Ok, então como você verifica se seu cliente está realmente autorizado? Simples, abra uma nova guia no Postman e envie uma solicitação GET para o WebAPI Endpoint. Agora, se o seu cliente está realmente autorizado, isso significa que o POSTMAN deve gritar com você com um erro 401. Assim, é bastante evidente que o IdentityServer está protegendo nosso API Endpoint e autorizando nosso Aplicativo WebClient a consumir a WebAPI. Incrível, sim?

Vamos encerrar o artigo por enquanto. Na próxima parte desta série, abordaremos tópicos mais avançados e a implementação do IdentitySevrer4. <b>Abordaremos conceitos como Adicionando a interface do usuário do IdentityServer4 ao projeto do servidor, Protegendo o projeto do cliente com o IdentityServer4, Removendo armazenamentos na memória e substituindo-os por banco de dados real, Adicionando ASP.NET identidade principal para gerenciar usuários com mais eficiência e muito mais.</b> Atualizarei o link para o próximo artigo aqui assim que estiver disponível. Fique atento.
