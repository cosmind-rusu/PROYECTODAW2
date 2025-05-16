using System;

namespace back.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public bool IsExpense { get; set; }
    }
}
