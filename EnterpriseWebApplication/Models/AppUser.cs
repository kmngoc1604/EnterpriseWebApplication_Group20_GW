using EnterpriseWebApplication.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseWebApplication.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        //public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<NIdea> Ideas { get; set; }
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
