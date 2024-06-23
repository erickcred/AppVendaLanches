using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages.Internas;

public partial class ListaProdutosPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
	private int _categoriaid;
	private string CategoriaNome = "categoria";

	public ListaProdutosPage(string categoriaNome, int categoriaId, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
		_apiService = apiService;
		_validator = validator;
    CategoriaNome = categoriaNome;
    _categoriaid = categoriaId;

	}

  protected async override void OnAppearing()
  {
		await GetListaProdutos(CategoriaNome, _categoriaid.ToString());
  }

  private async Task<IEnumerable<Produto>> GetListaProdutos(string tipoProduto, string categoriaId)
	{
		try
		{
			var (produtos, errorMessage) = await _apiService.GetProdutos("categoria", categoriaId);

			if (errorMessage is not null)
			{
        //await DisplayLoginPage();
        return Enumerable.Empty<Produto>();
      }

			if (produtos is null)
			{
				await DisplayAlert("Erro", $"Lista de produtos da categoria: {CategoriaNome} não pode ser carregado", "OK");
				return Enumerable.Empty<Produto>();
			}

      cvProdutosPorCategoria.ItemsSource = produtos;
			return produtos;
		}
		catch (Exception ex)
		{
      await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex}", "OK");
      return Enumerable.Empty<Produto>();
    }
	}
}