using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using APICore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APICore.Controllers
{

  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class BaseController<TEntity> : ControllerBase where TEntity : class
  {
    #region Constructors

    public BaseController(IRepository<TEntity> repository)
    {
      this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #endregion

    #region Properties

    protected IRepository<TEntity> Repository { get; }

    #endregion

    #region Get

    [AllowAnonymous]
    [HttpGet]
    public virtual IEnumerable<TEntity> Get()
    {
      return Repository.Entities.ToList();
    }

    #endregion

    #region Post

    [HttpPost]
    public HttpResponseMessage Post([FromBody] TEntity entity)
    {
      Repository.Insert(entity);

      return new HttpResponseMessage(HttpStatusCode.Created);
    }

    #endregion

    #region Put

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, TEntity entity)
    {
      await Repository.UpdateAsync(entity);

      return Ok();
    }

    #endregion

    #region Patch

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<TEntity> patchDoc)
    {
      if (patchDoc != null)
      {
        var entity = Repository.GetById(id);
        var result = await Repository.Patch(entity, patchDoc, ModelState);

        if (result == 1 || result == 0)
          return Ok();
      }

      return BadRequest(ModelState);
    }

    #endregion

    #region Delete

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var dbEntity = Repository.GetById(id);

      Repository.Delete(dbEntity);

      return Ok();
    }

    #endregion
  }
}



