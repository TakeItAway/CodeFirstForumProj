using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CF.Data.Models
{
    public enum ELanguage { Ru, En }
    public enum ETheme { Light, Dark }

    public class ApplicationUser : IdentityUser
    {
        public bool IsBlocked { get; set; }
    }
}
