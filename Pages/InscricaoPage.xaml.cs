using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class InscricaoPage : ContentPage
{
  private readonly ApiService _apiService;
  private readonly IValidator _validator;

  public InscricaoPage(ApiService apiService, IValidator validator)
  {
    InitializeComponent();
    _apiService = apiService;
    _validator = validator;
  }

  private async void btnResgistro_Clicked(object sender, EventArgs e)
  {
    if (await _validator.Validar(txtNome.Text, txtEmail.Text, txtTelefone.Text, txtPassword.Text))
    {
      var response = await _apiService.RegistrarUsuario(
        txtNome.Text,
        txtEmail.Text,
        txtTelefone.Text,
        txtPassword.Text);

      if (!response.HasError)
      {
        await DisplayAlert("Aviso", "Sua conta foi criada com sucesso!!", "OK");
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
      }
      else
      {
        await DisplayAlert("Erro", "Algo deu errado!", "Cancelar");
      }
    }
    else
    {
      string mensagemErro = string.Empty;
      mensagemErro += _validator.NomeErro is not null ? $"\n- {_validator.NomeErro}" : string.Empty;
      mensagemErro += _validator.EmailErro is not null ? $"\n- {_validator.EmailErro}" : string.Empty;
      mensagemErro += _validator.TelefoneErro is not null ? $"\n- {_validator.TelefoneErro}" : string.Empty;
      mensagemErro += _validator.SenhaErro is not null ? $"\n- {_validator.SenhaErro}" : string.Empty;
      await DisplayAlert("Erro", mensagemErro, "OK");
    }

  }

  private async void TapLogin_Tapped(object sender, TappedEventArgs e)
  {
    await Navigation.PushAsync(new LoginPage(_apiService, _validator));
  }
}