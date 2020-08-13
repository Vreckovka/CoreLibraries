using System;
using System.ComponentModel.DataAnnotations.Schema;
using DomainCore.BaseDomainClasses;

namespace DomainCore.DomainClasses
{
  public class LoginSession : DatedEntity
  {
    public Guid Token { get; set; }


    [ForeignKey("Administrator")]
    public int AdministratorId { get; set; }


    public Administrator Administrator { get; set; }
  }
}
