using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public class AppUser: IdentityUser
    {
        public int AddressId { get; set; }
        
        public Address Address { get; set; }
    }
}
