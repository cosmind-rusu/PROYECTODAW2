using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = null!;
        public IdentityUser? User { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
