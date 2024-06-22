using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class LoginPage : ContentPage
{
	private readonly ApiService _apiService;
  private readonly IValidator _validator;

	public LoginPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
		_apiService = apiService;
    _validator = validator;
	}

  private async void btnEntrar_Clicked(object sender, EventArgs e)
  {
    if (string.IsNullOrWhiteSpace(txtEmail.Text))
    {
      await DisplayAlert("Erro", "Informe o Email!", "Cancelar");
      return;
    }

    if (string.IsNullOrWhiteSpace(txtPassword.Text))
    {
      await DisplayAlert("Erro", "Informe a Senha!", "Cancelar");
      return;
    }

    var response = await _apiService.Login(txtEmail.Text, txtPassword.Text);
    if (!response.HasError)
    {
      Application.Current!.MainPage = new AppShell(_apiService, _validator);
    }
    else
      await DisplayAlert($"Erro", "Algo deu errado!\n-{response}", "Cancelar");
  }

  private async void TapRegister_Tapped(object sender, TappedEventArgs e)
  {
    await Navigation.PushAsync(new InscricaoPage(_apiService, _validator));
  }
}