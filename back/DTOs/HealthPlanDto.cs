using System;

namespace back.DTOs
{
    public class HealthPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre del plan
        public int TreatmentId { get; set; } // Tratamiento cubierto por el plan
        public string? TreatmentName { get; set; } // Nombre del tratamiento para UI
        public string Description { get; set; } = string.Empty; // Descripción del plan
        public decimal Cost { get; set; } // Costo del plan
        public int DurationMonths { get; set; } = 12; // Duración en meses
        public int VisitsIncluded { get; set; } = 0; // Visitas incluidas
        public bool IncludesEmergencies { get; set; } = false; // Cobertura de emergencias
        public decimal DiscountPercentage { get; set; } = 0; // Porcentaje de descuento
        public string CoverageDetails { get; set; } = string.Empty; // Detalles de cobertura
        public DateTime StartDate { get; set; } // Inicio
        public DateTime EndDate { get; set; } // Fin
        public bool IsActive { get; set; } // Activo o no
    }
}
