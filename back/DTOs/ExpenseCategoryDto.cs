using System;

namespace back.DTOs
{
    // Renombrado conceptualmente a TreatmentDto
    public class ExpenseCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal StandardCost { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string RequiredEquipment { get; set; } = string.Empty;
        public string Procedure { get; set; } = string.Empty;
        public string Aftercare { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
