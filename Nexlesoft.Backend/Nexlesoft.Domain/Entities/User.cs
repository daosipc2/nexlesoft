using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexlesoft.Domain.Entities
{
    //public class User : IdentityUser<int>
    public class User 
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        //[NotMapped]
        //public override int AccessFailedCount { get; set; }

        //[NotMapped]
        //public override string? ConcurrencyStamp { get; set; }
        public ICollection<Token> Tokens { get; set; }
    }
}
