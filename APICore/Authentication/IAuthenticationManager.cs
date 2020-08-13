namespace APICore.Authentication
{
  public interface IAuthenticationManager
  {
    bool IsTokenValid(string token);
  }
}