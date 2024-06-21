using AppLanches.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppLanches.Services;

public class ApiService
{
  private readonly HttpClient _httpClient;
  private readonly string _baseUrl = "https://vvb3jv62-7019.brs.devtunnels.ms/";
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
}
