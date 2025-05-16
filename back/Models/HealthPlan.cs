using System;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class HealthPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre del plan de salud o seguro
        public int TreatmentId { get; set; } // Tratamiento cubierto por el plan
        public Treatment? Treatment { get; set; } // Relación con el tratamiento
        public string Description { get; set; } = string.Empty; // Descripción detallada
        public decimal Cost { get; set; } // Costo del plan para el cliente
        public int DurationMonths { get; set; } = 12; // Duración en meses
        public int VisitsIncluded { get; set; } = 0; // Número de visitas incluidas
        public bool IncludesEmergencies { get; set; } = false; // Si cubre emergencias
        public decimal DiscountPercentage { get; set; } = 0; // Porcentaje de descuento en tratamientos
        public string CoverageDetails { get; set; } = string.Empty; // Detalles de cobertura
        public DateTime StartDate { get; set; } // Fecha de inicio
        public DateTime EndDate { get; set; } // Fecha de finalización
        public bool IsActive { get; set; } // Si está activo
        public string UserId { get; set; } = null!; // ID del veterinario que creó el plan
        public IdentityUser? User { get; set; } // Usuario veterinario
    }
}
