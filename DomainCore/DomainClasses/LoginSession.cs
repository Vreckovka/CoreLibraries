using System;
using System.ComponentModel.DataAnnotations.Schema;
using DomainCore.BaseDomainClasses;

namespace DomainCore.DomainClasses
{
  public class LoginSession : DatedEntity
  {
    public Guid Token { get; set; }


    [ForeignKey("User")]
    public int UserId { get; set; }


    public User User { get; set; }
  }
}
