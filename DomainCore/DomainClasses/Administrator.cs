using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DomainCore.BaseDomainClasses;

namespace DomainCore.DomainClasses
{
  public class Administrator : DatedEntity
  {
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Password { get; set; }

    public virtual ICollection<LoginSession> LoginSessions { get; set; }
  }
}
