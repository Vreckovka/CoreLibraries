using System;

namespace DomainCore.BaseDomainClasses
{
  public abstract class DatedEntity : DomainEntity
  {
    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }
  }
}
