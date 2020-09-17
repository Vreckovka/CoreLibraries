using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainCore.BaseDomainClasses
{
  public abstract class DomainEntity
  {
    [Key, Column(Order = 0)]
    public int Id { get; set; }

  }
}