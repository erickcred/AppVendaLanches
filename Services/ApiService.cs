using AppLanches.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppLanches.Services;

public class ApiService
{
  private readonly HttpClient _httpClient;
  private readonly string _baseUrl = AppConfig.BaseUrl;
  private readonly ILogger<ApiService> _logger;
  JsonSerializerOptions _serializerOptions;

  public ApiService(
    HttpClient httpClient,
    ILogger<ApiService> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
    _serializerOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };
  }

  public async Task<ApiServiceResponse<bool>> RegistrarUsuario(string nome, string email, string telefone, string password)
  {
    try
    {
      var register = new Register
      {
        Nome = nome,
        Email = email,
        Telefone = telefone,
        Senha = password
      };

      var json = JsonSerializer.Serialize(register, _serializerOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var response = await PostRequest("api/Usuarios/Cadastrar", content);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
        return new ApiServiceResponse<bool>
        {
          ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
        };
      }
      return new ApiServiceResponse<bool> { Data = true };
    }
    catch (Exception ex)
    {
      _logger.LogError($"Erro ao registrar usuário: {ex.Message}");
      return new ApiServiceResponse<bool> { ErrorMessage = ex.Message };
    }
  }

  public async Task<ApiServiceResponse<bool>> Login(string email, string password)
  {
    try
    {
      var login = new Login
      {
        Nome = email,
        Email = email,
        Senha = password
      };

      var json = JsonSerializer.Serialize(login, _serializerOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      

      var response = await PostRequest("api/Usuarios/Login", content);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
        return new ApiServiceResponse<bool>
        {
          ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
        };
      }

      var jsonResult = await response.Content.ReadAsStringAsync();
      var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

      Preferences.Set("accessToken", result!.AccessToken);
      Preferences.Set("usuarioId", (int)result.UsuarioId!);
      Preferences.Set("usuarioNome", result!.UsuarioNome);

      return new ApiServiceResponse<bool> { Data = true };
    }
    catch (Exception ex)
    {
      _logger.LogError($"Erro no login: {ex.Message}");
      return new ApiServiceResponse<bool> { ErrorMessage = ex.Message };
    }
  }

  public async Task<(List<Categoria>? Categorias, string? ErrorMessage)> GetCategorias()
  {
    return await GetAsync<List<Categoria>>("api/categoria");
  }

  public async Task<(List<Produto>? Produtos, string? Errormessage)> GetProdutos(string tipoProduto, string categoriaId)
  {
    string endpoint = $"api/Produto?tipoProduto={tipoProduto}&categoriaId={categoriaId}";
    return await GetAsync<List<Produto>>(endpoint);
  }

  private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent httpContent)
  {
    var enderecoUrl = _baseUrl + uri;
    try
    {
      var result = await _httpClient.PostAsync(enderecoUrl, httpContent);
      return result;
    }
    catch (Exception ex)
    {
      _logger.LogError($"Erro ao enviar requisição POST para {uri}: {ex.Message}");
      return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }
  }

  private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
  {
    string errorMessage = string.Empty;
    try
    {
      AddAuthorizationHeader();

      var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

      if (response.IsSuccessStatusCode)
      {
        var responseString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
        return (data ?? Activator.CreateInstance<T>(), null);
      }
      else
      {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
          errorMessage = "Unauthorized";
          _logger.LogError(errorMessage);
          return (default, errorMessage);
        }
      }

      string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
      _logger.LogError(generalErrorMessage);
      return (default, generalErrorMessage);
    }
    catch (HttpRequestException ex)
    {
      errorMessage = $"Erro de requisição HTTP: {ex.Message}";
      _logger.LogError(ex, ex.Message);
      return (default, errorMessage);
    }
    catch (JsonException ex)
    {
      errorMessage = $"Erro de desserialização JSON: {ex.Message}";
      _logger.LogError(ex, ex.Message);
      return (default, errorMessage);
    }
    catch (Exception ex)
    {
      errorMessage = $"Erro inesperado: {ex.Message}";
      _logger.LogError(ex, ex.Message);
      return (default, errorMessage);
    }
  }

  private void AddAuthorizationHeader()
  {
    var token = Preferences.Get("accessToken", string.Empty);
    if (!string.IsNullOrWhiteSpace(token))
    {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
  }
}
