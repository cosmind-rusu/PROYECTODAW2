using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class AnimalSpecies
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre de la especie (perro, gato, ave, etc.)
        public string Description { get; set; } = null!; // Descripción general de la especie
        public bool IsActive { get; set; } // Si está activa en el sistema
        public string CommonIssues { get; set; } = string.Empty; // Problemas comunes de salud para esta especie
        public string CareInstructions { get; set; } = string.Empty; // Instrucciones generales de cuidado
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = null!; // ID del veterinario o persona que registró la especie
        public IdentityUser? User { get; set; }
        public ICollection<VeterinaryConsultation>? VeterinaryConsultations { get; set; } // Consultas para esta especie
    }
}
