using Domain.Entities.common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.user
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
    }
}
