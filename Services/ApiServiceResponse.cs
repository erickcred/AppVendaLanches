namespace AppLanches.Services
{
  public class ApiServiceResponse<T>
  {
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
  }
}
