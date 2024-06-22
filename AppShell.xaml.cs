using AppLanches.Pages.Internas;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches
{
  public partial class AppShell : Shell
  {
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public AppShell(
      ApiService apiService,
      IValidator validator)
    {
      InitializeComponent();
      _apiService = apiService?? throw new ArgumentException(nameof(apiService));
      _validator = validator;

      ConfigShell();
    }

    private void ConfigShell()
    {
      var homePage = new HomePage(_apiService, _validator);
      var carrinhoPage = new CarrinhoPage();
      var favoritosPage = new FavoritosPage();
      var perfilPage = new PerfilPage();

      Items.Add(new TabBar
      {
        Items =
        {
          new ShellContent { Title = "", Icon = "home", Content = homePage },
          new ShellContent { Title = "Carrinho", Icon = "cart", Content = carrinhoPage },
          new ShellContent { Title = "Favoritos", Icon = "heart", Content = favoritosPage },
          new ShellContent { Title = "Perfil", Icon = "person", Content = perfilPage }
        }
      });
    }
  }
}
