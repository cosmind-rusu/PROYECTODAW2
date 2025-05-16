using System;

namespace back.DTOs
{
    public class TreatmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre del tratamiento
        public string Description { get; set; } = null!; // Descripción del tratamiento
        public bool IsExpense { get; set; } // Mantener para compatibilidad
        public decimal DefaultCost { get; set; } = 0; // Costo estándar
        public string? MedicalRequirements { get; set; } // Requisitos médicos
        public string? PostTreatmentCare { get; set; } // Cuidados posteriores
        public string? TypicalMedication { get; set; } // Medicamentos comunes
        public int EstimatedTimeMinutes { get; set; } = 30; // Duración estimada
        public string? IconName { get; set; } // Mantener para UI
        public string? ColorCode { get; set; } // Mantener para UI
        public bool IsActive { get; set; } // Si está disponible
        public DateTime CreatedDate { get; set; }
    }
}
