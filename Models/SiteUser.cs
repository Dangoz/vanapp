using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
  public class SiteUser : IdentityUser<int>
  {
    public int Age { get; set; }
    public string Gender { get; set; }
    public string City { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }

    public ICollection<Photo>? Photos { get; set; }

    public ICollection<Message> MessagesSent { get; set; }

    public ICollection<Message> MessagesReceived { get; set; }

  }
}