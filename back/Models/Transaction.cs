using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public bool IsExpense { get; set; }
        public string UserId { get; set; } = null!;
        public IdentityUser? User { get; set; }
        public Category? Category { get; set; }
    }
}
