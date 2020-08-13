using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using APICore.Factories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APICore.Repositories
{

  public interface IRepository<TEntity> 
  {
    IQueryable<TEntity> Entities { get;  }

    void Insert(TEntity entity);
    void Update(TEntity entity);
    Task<int> UpdateAsync(TEntity entity);
    void Delete(TEntity entity);
    TEntity GetById(int id);

    Task<int> Patch<TModel>(TModel dbEntity, JsonPatchDocument<TModel> patchDoc, ModelStateDictionary modelState)
      where TModel : class;

  }

  public class Repository<TContext,TEntity>  : IRepository<TEntity> 
    where TEntity : class 
    where TContext : DbContext
  {

    #region Constructors
    
    public Repository(IContextFactory<TContext> contextFactory)
    {
      Context = contextFactory.CreateContext();
    }

    #endregion

    #region Properties

    #region Context

    private TContext Context { get; } 

    #endregion

    #endregion


    public IQueryable<TEntity> Entities
    {
      get { return Context.Set<TEntity>(); }
    }

    public TEntity GetById(int id)
    {
      return Context.Set<TEntity>().Find(id);
    }

    public IEnumerable<TEntity> List()
    {
      return Context.Set<TEntity>().AsEnumerable();
    }

    public void Insert(TEntity entity)
    {
      Context.Set<TEntity>().Add(entity);
      Context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
      Context.Entry(entity).State = EntityState.Modified;
      Context.SaveChanges();
    }

    public Task<int> UpdateAsync(TEntity entity)
    {
      Context.Entry(entity).State = EntityState.Modified;
      return Context.SaveChangesAsync();
    }

    public void Delete(TEntity entity)
    {
      Context.Set<TEntity>().Remove(entity);
      Context.SaveChanges();
    }

    public Task<int> Patch<TModel>(TModel dbEntity, JsonPatchDocument<TModel> patchDoc, ModelStateDictionary modelState)
    where TModel : class
    {
      if (dbEntity != null)
      {
        patchDoc.ApplyTo(dbEntity, modelState);
      }
      else
      {
        throw new Exception("Entity does not exists");
      }

      if (!modelState.IsValid)
      {
        throw new Exception("Invalid model state");
      }

      return Context.SaveChangesAsync();
    }
  }
}