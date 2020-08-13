using Microsoft.AspNetCore.Builder;

namespace APICore.Middlewares
{
  public static class MiddlewareExtentions
  {
    public static void UseCustomExceptionMiddlewere(this IApplicationBuilder app)
    {
      app.UseMiddleware<ExceptionMiddleware>();
    }
  }
}