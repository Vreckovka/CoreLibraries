using System;
using System.Net;
using System.Threading.Tasks;
using APICore.Logging;
using Microsoft.AspNetCore.Http;

namespace APICore.Middlewares
{
  public class ExceptionMiddleware
  {
    private readonly ILogger logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next, ILogger logger)
    {
      this._next = next ?? throw new ArgumentNullException(nameof(next));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        logger.Log(ex);
        await HandleExcepion(context);
      }
    }

    public async Task HandleExcepion(HttpContext context)
    {
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      context.Response.ContentType = "application/json";
    }
  }
}

