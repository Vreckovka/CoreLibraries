using System.ComponentModel.DataAnnotations;

namespace DomainCore.BaseDomainClasses
{
  public abstract class DomainEntity
  {
    [Key]
    public int Id { get; set; }

  }
}