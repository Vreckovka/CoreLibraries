namespace APICore.Authentication
{
  public class AuthenticationManager : IAuthenticationManager
  {
    public bool IsTokenValid(string token)
    {
      if (token == "TOKEN")
      {
        return true;
      }

      return false;
    }
  }
}