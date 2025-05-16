using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre del tratamiento o procedimiento
        public string Description { get; set; } = null!; // Descripción detallada
        public bool IsExpense { get; set; } = true; // Mantener para compatibilidad
        public decimal DefaultCost { get; set; } = 0; // Costo estándar del tratamiento
        public string? MedicalRequirements { get; set; } // Requisitos médicos
        public string? PostTreatmentCare { get; set; } // Cuidados posteriores al tratamiento
        public string? TypicalMedication { get; set; } // Medicamentos típicos para este tratamiento
        public int EstimatedTimeMinutes { get; set; } = 30; // Duración estimada
        public string? IconName { get; set; } // Ícono representativo (mantener para UI)
        public string? ColorCode { get; set; } // Color representativo (mantener para UI)
        public bool IsActive { get; set; } // Si el tratamiento está disponible actualmente
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = null!; // ID del veterinario que creó el tratamiento
        public IdentityUser? User { get; set; }
        public ICollection<VeterinaryConsultation>? VeterinaryConsultations { get; set; } // Consultas donde se aplicó este tratamiento
        public ICollection<HealthPlan>? HealthPlans { get; set; } // Planes de salud que cubren este tratamiento
    }
}
