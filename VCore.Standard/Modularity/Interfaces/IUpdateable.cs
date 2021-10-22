namespace VCore.Standard.Modularity.Interfaces
{
  public interface IUpdateable<TEntity> 
  {
    void Update(TEntity other);
  }
}