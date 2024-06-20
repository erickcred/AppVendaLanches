using System.Text.RegularExpressions;

namespace AppLanches.Validations;

public class Validator : IValidator
{
  public string NomeErro { get; set; } = string.Empty;
  public string EmailErro { get; set; } = string.Empty;
  public string TelefoneErro { get; set; } = string.Empty;
  public string SenhaErro { get; set; } = string.Empty;

  private const string NomeVazioErroMsg = "Por favor, informe o seu nome!";
  private const string NomeInvalidoErroMsg = "Por favor, informe um nome válido!";
  private const string EmailVazioErroMsg = "Por favor, informe o seu email!";
  private const string EmailInvalidoErroMsg = "Por favor, informe um email válido!";
  private const string TelefoneVazioErroMsg = "Por favor, informe o seu telefone!";
  private const string TelefoneInvalidoErroMsg = "Por favor, informe um telefone válido!";
  private const string SenhaVazioErroMsg = "Por favor, informe sua senha!";
  private const string SenhaInvalidaErroMsg = "A senha deve conter pelo menos 8 caracteres, incluindo letras, numeros e/ou caracteres especiais";

  public Task<bool> Validar(string nome, string email, string telefone, string senha)
  {
    var isNomeValido = ValidarNome(nome);
    var isEmailValido = ValidarEmail(email);
    var isTelefoneValido = ValidarTelefone(telefone);
    var isSenhaValida = ValidarSenha(senha);

    return Task.FromResult(isNomeValido && isEmailValido && isTelefoneValido && isSenhaValida);
  }

  private bool ValidarNome(string nome)
  {
    if (string.IsNullOrWhiteSpace(nome))
    {
      NomeErro = NomeVazioErroMsg;
      return false;
    }

    if (nome.Length < 3)
    {
      NomeErro = NomeInvalidoErroMsg;
      return false;
    }
    NomeErro = string.Empty;
    return true;
  }

  private bool ValidarEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      EmailErro = EmailVazioErroMsg;
      return false;
    }

    if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\.\-]+)((\.(\w){2,3})+)$"))
    {
      EmailErro = EmailInvalidoErroMsg;
      return false;
    }
    EmailErro = string.Empty;
    return true;
  }

  private bool ValidarTelefone(string telefone)
  {
    if (string.IsNullOrWhiteSpace(telefone))
    {
      TelefoneErro = TelefoneVazioErroMsg;
      return false;
    }

    if (telefone.Length < 12)
    {
      TelefoneErro = TelefoneInvalidoErroMsg;
      return false;
    }
    TelefoneErro = string.Empty;
    return true;
  }

  private bool ValidarSenha(string senha)
  {
    if (string.IsNullOrWhiteSpace(senha))
    {
      SenhaErro = SenhaVazioErroMsg;
      return false;
    }

    if (senha.Length < 8 || !Regex.IsMatch(senha, @"[a-zA-Z]") || !Regex.IsMatch(senha, @"\d"))
    {
      SenhaErro = SenhaInvalidaErroMsg;
      return false;
    }
    SenhaErro = string.Empty;
    return true;
  }

}
