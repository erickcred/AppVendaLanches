namespace AppLanches.Pages.Internas;

public partial class HomePage : ContentPage
{
	private string CaminhoImagem { get; set; }
	private string Nome { get; set; }


  public HomePage()
	{
		InitializeComponent();
		lblSaudacao.Text += Preferences.Get("usuarioNome", string.Empty);
	}
}