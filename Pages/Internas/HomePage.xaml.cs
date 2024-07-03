using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages.Internas;

public partial class HomePage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
	private bool _loginPageDisplay = false;


  public HomePage(
		ApiService apiService,
		IValidator validator)
	{
		InitializeComponent();
		_apiService = apiService ?? throw new ArgumentException(nameof(apiService));
		_validator = validator;

		lblSaudacao.Text += Preferences.Get("usuarioNome", string.Empty);
	}

	// Metodo que é iniciando quando a pagina está sendo carregada,
	// Responsavel por carregar os dados de exibição na pagina
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await GetListaCategorias();
		await GetMaisVendidos();
		await GetPopulares();
	}

  #region SelectChange
  private void cvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var currentSelection = e.CurrentSelection.FirstOrDefault() as Categoria;

		if (currentSelection is null) return;
		Navigation.PushAsync(
			new ListaProdutosPage(currentSelection.Nome!, currentSelection.Id, _apiService, _validator));

		((CollectionView)sender).SelectedItem = null;
	}
  #endregion

  #region Retornos DataGrids
  private async Task<IEnumerable<Categoria>> GetListaCategorias()
	{
    try
		{
			var (categorias, errorMessage) = await _apiService.GetCategorias();

			if (errorMessage != null && errorMessage!.Equals("Unauthorized") && !_loginPageDisplay)
			{
				await DisplayLoginPage();
				return Enumerable.Empty<Categoria>();
			}

			if (categorias is null)
			{
				await DisplayAlert("Error", errorMessage ?? "Não foi possível obter as categorias.", "OK");
				return Enumerable.Empty<Categoria>();
			}
			cvCategorias.ItemsSource = categorias;
			return categorias;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex}", "OK");
			return Enumerable.Empty<Categoria>();
		}
	}

	private async Task<IEnumerable<Produto>> GetMaisVendidos()
	{
    try
    {
			var (produtos, errorMessage) = await _apiService.GetProdutos("mais vendido", string.Empty);

			if (errorMessage is not null && errorMessage!.Equals("Unauthorized") && !_loginPageDisplay)
			{
				await DisplayLoginPage();
				return Enumerable.Empty<Produto>();
			}

			if (produtos is null)
			{
				await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter produtos mais vendidos!", "OK");
				return Enumerable.Empty<Produto>();
			}

			cvMaisVendidos.ItemsSource = produtos;
			return produtos;
    }
    catch (Exception ex)
    {
			await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex}", "OK");
			return Enumerable.Empty<Produto>();
    }
  }

	private async Task<IEnumerable<Produto>> GetPopulares()
	{
		try
		{
			var (produtos, errorMessage) = await _apiService.GetProdutos("popular", string.Empty);

			if (errorMessage is not null && errorMessage!.Equals("Unauthorized") && !_loginPageDisplay)
			{
				await DisplayLoginPage();
				return Enumerable.Empty<Produto>();
			}

			if (produtos is null)
			{
				await DisplayAlert("Error", errorMessage ?? $"Não foi possive obter produtos mais populares!", "OK");
				return Enumerable.Empty<Produto>();
			}

			cvPopulares.ItemsSource = produtos;
			return produtos;
		}
		catch (Exception ex)
		{
      await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex}", "OK");
      return Enumerable.Empty<Produto>();
    }
	}
	#endregion

	private async Task DisplayLoginPage()
	{
		_loginPageDisplay = true;
		await Navigation.PushAsync(new LoginPage(_apiService, _validator));
	}
}